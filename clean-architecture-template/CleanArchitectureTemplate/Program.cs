using Application;
using CleanArchitectureTemplate;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

// Build the app.
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
