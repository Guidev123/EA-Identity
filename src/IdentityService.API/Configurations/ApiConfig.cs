using IdentityService.API.Data;
using IdentityService.API.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.Tokens.AspNet;
using SharedLib.Tokens.Configuration;
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
        builder.Services.AddSwaggerConfig();
    }

    public static void AddIdentityConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddDefaultTokenProviders();

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(2);
        });
        builder.Services.AddJwtConfiguration(builder.Configuration);

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

        app.UseAuthConfiguration();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseJwksDiscovery();
    }
}
