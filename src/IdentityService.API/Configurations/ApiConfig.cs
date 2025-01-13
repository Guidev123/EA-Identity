using IdentityService.API.Data;
using IdentityService.API.Extensions;
using IdentityService.API.Middlewares;
using IdentityService.API.Services;
using IdentityService.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.MessageBus;
using SharedLib.Tokens.AspNet;
using SharedLib.Tokens.Core;
using SharedLib.Tokens.Core.Jwa;
using SharedLib.Tokens.EntityFramework;

namespace IdentityService.API.Configurations;

public static class ApiConfig
{
    public static void AddDbContextConfig(this WebApplicationBuilder builder) =>
        builder.Services.AddDbContext<AuthenticationDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    public static void AddEnviromentConfig(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddJwtConfiguration(builder.Configuration);
        builder.Services.AddSwaggerConfig();
    }

    public static void AddMessageBusConfiguration(this WebApplicationBuilder builder) =>
        builder.Services.AddMessageBus(builder.Configuration.GetMessageQueueConnection("MessageBus"));

    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
        builder.Services.AddTransient<ITokenGeneratorService, TokenGeneratorService>();
        builder.Services.AddTransient<GlobalExceptionMiddleware>();
        var appSettingsSection = builder.Configuration.GetSection(nameof(AppTokenSettings));
        builder.Services.Configure<AppTokenSettings>(appSettingsSection);
    }

    public static void AddIdentityConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddDefaultTokenProviders();

        builder.Services.AddJwksManager(x => x.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.EcdsaSha256))
            .PersistKeysToDatabaseStore<AuthenticationDbContext>()
            .UseJwtValidation();
    }

    public static void UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static void UseSecurity(this IApplicationBuilder app)
    {
        app.UseSwaggerConfig();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("Total");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseJwksDiscovery();
    }
}
