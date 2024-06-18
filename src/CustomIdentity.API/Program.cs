//========================================== Environment Configure ===============================================/
using CustomIdentity.API.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();
//================================================ END ========================================================/


builder.Services.AddApiConfig(builder.Configuration);

var app = builder.Build();
app.CustomMiddlewares(app.Environment);
app.Run();