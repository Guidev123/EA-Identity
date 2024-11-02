using IdentityService.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.AddEnviromentMiddleware();
builder.AddDbContextMiddleware();
builder.AddDependenciesMiddleware();
builder.AddIdentityMiddleware();

var app = builder.Build();
app.UseSecurity();
app.Run();