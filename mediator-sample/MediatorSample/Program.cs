using System.Reflection;
using MediatorSample.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMediator(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();