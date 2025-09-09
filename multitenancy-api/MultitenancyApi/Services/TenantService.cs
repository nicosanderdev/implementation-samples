using Microsoft.Extensions.Options;

namespace MultitenancyApi.Services
{
    public class TenantSettings
    {
        public List<Tenant> Tenants { get; set; } = new();
    }

    public class TenantService : ITenantService
    {
        private readonly TenantSettings _tenantSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Tenant? _currentTenant;

        public TenantService(IOptions<TenantSettings> tenantSettings, IHttpContextAccessor httpContextAccessor)
        {
            _tenantSettings = tenantSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public Tenant GetCurrentTenant()
        {
            if (_currentTenant != null)
            {
                return _currentTenant;
            }

            var httpContext = _httpContextAccessor.HttpContext ?? throw new Exception("No active HttpContext.");

            if (!httpContext.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("X-Api-Key header is missing.");
            }

            _currentTenant = _tenantSettings.Tenants.FirstOrDefault(t => t.ApiKey == apiKey.ToString());

            if (_currentTenant == null)
            {
                throw new Exception("Invalid API Key.");
            }

            return _currentTenant;
        }
    }
}