using IdentityService.API.Configurations;
using SharedLib.Tokens.AspNet;

var builder = WebApplication.CreateBuilder(args);
builder.AddEnviromentConfig();
builder.AddDbContextConfig();
builder.AddDependencies();
builder.AddIdentityConfig();
builder.AddMessageBusConfiguration();

var app = builder.Build();
app.UseSecurity();
app.UseJwksDiscovery();

app.Run();