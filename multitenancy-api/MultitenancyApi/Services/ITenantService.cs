namespace MultitenancyApi.Services
{
    public interface ITenantService
    {
        Tenant GetCurrentTenant();
    }
}