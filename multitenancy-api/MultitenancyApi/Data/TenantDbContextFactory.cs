using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MultitenancyApi.Services;

namespace MultitenancyApi.Data
{
    public class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")) &&
                File.Exists(Path.Combine(basePath, "MultitenancyApi", "appsettings.json")))
            {
                basePath = Path.Combine(basePath, "MultitenancyApi");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var tenantSettings = new TenantSettings();
            config.GetSection("TenantSettings").Bind(tenantSettings);

            var connectionString = tenantSettings.Tenants.Count > 0
                ? tenantSettings.Tenants[0].ConnectionString
                : throw new Exception("No tenants found in configuration to create design-time DbContext.");

            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new TenantDbContext(optionsBuilder.Options);
        }
    }
}
