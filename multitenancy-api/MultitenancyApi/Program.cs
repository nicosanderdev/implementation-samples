using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using MultitenancyApi.Data;
using MultitenancyApi.Repositories;
using MultitenancyApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "MultiTenantApi",
        Version = "v1"
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key de autenticación para el Tenant. Escribe tu key aquí.",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] { }
        }
    });
});


builder.Services.AddHttpContextAccessor();
builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection("TenantSettings"));
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
{
    var tenantService = serviceProvider.GetRequiredService<ITenantService>();
    var tenant = tenantService.GetCurrentTenant();
    var connectionString = tenant.ConnectionString;
    
    if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseNpgsql(connectionString);
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("StartupMigration");

    try
    {
        var tenantSettings = services.GetRequiredService<IOptions<TenantSettings>>().Value;
        foreach (var tenant in tenantSettings.Tenants)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseNpgsql(tenant.ConnectionString);

                using var db = new TenantDbContext(optionsBuilder.Options);
                db.Database.Migrate();
                logger.LogInformation("Applied migrations for tenant {Tenant}", tenant.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply migrations for tenant {Tenant}", tenant.Name);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply migrations at startup");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();