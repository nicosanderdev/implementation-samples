using Stripe;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StripeSample.Model;
using StripeSample.Options;

namespace StripeSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly StripeOptions _stripeOptions;
    private readonly ILogger<StripeController> _logger;
    private readonly PaymentIntentService _paymentIntentService;

    public StripeController(IOptions<StripeOptions> stripeOptions, ILogger<StripeController> logger,
        PaymentIntentService paymentIntentService)
    {
        _stripeOptions = stripeOptions.Value;
        _logger = logger;
        _paymentIntentService = paymentIntentService;
    }

    [HttpPost("create-payment-intent")]
    public ActionResult<CreatePaymentIntentResponse> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        try
        {
            var createOptions = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var requestOptions = new RequestOptions
            {
                ApiKey = _stripeOptions.SecretKey
            };

            var paymentIntent = _paymentIntentService.Create(createOptions, requestOptions);

            return Ok(new CreatePaymentIntentResponse { ClientSecret = paymentIntent.ClientSecret });
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe error while creating payment intent.");
            return BadRequest(new { error = new { message = e.StripeError.Message } });
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeOptions.WebhookSecret
            );

            _logger.LogInformation("Stripe event received: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var succeededPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation("PaymentIntent was successful for {Amount}!", succeededPaymentIntent.Amount);
                    // TODO: fulfill the order, grant access, update your DB, etc.
                    break;

                case "payment_intent.payment_failed":
                    var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogWarning("PaymentIntent failed for {PaymentIntentId}", failedPaymentIntent?.Id);
                    // TODO: notify the user
                    break;

                default:
                    _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe webhook error.");
            return BadRequest();
        }
    }
}