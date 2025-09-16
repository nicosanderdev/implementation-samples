namespace StripeSample.Options;

public class StripeOptions
{
    public const string Stripe = "Stripe";

    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}