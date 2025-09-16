using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using StripeSample.Model;
using StripeSample.Options;

namespace StripeSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly StripeOptions _stripeOptions;
    private readonly ILogger<StripeController> _logger;

    public StripeController(IOptions<StripeOptions> stripeOptions, ILogger<StripeController> logger)
    {
        _stripeOptions = stripeOptions.Value;
        _logger = logger;
    }
    
    [HttpPost("create-payment-intent")]
    public ActionResult<CreatePaymentIntentResponse> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        // This is a crucial step. The Stripe.net library uses a static singleton
        // for configuration. This sets the API key for the current thread.
        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;

        try
        {
            var options = new PaymentIntentCreateOptions
            {
                // Amount is in the smallest currency unit (e.g., cents)
                Amount = request.Amount,
                Currency = "usd", // Or any other currency
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Ok(new CreatePaymentIntentResponse { ClientSecret = paymentIntent.ClientSecret });
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe error while creating payment intent.");
            return BadRequest(new { error = new { message = e.StripeError.Message } });
        }
    }
}