namespace MultitenancyApi.Services
{
    public class Tenant
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string ConnectionString { get; set; }
    }
}