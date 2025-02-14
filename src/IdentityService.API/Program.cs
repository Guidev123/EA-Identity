using IdentityService.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddEnviromentConfig();
builder.AddCorsConfig();
builder.AddDbContextConfig();
builder.AddDependencyInjection();
builder.AddIdentityConfig();
builder.AddMessageBusConfiguration();

var app = builder.Build();
app.UseCustomMiddlewares();
app.UseSecurity();

app.Run();