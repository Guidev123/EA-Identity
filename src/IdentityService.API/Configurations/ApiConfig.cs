﻿using IdentityService.API.Data;
using IdentityService.API.Middlewares;
using KeyPairJWT.AspNet;
using KeyPairJWT.Configuration;
using KeyPairJWT.Core;
using KeyPairJWT.Core.Jwa;
using KeyPairJWT.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    public static void AddCorsConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Total", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
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
