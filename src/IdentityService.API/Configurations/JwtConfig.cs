﻿using IdentityService.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SharedLib.Tokens.Extensions;

namespace IdentityService.API.Configurations;

public static class JwtConfig
{
    public static void AddJwtConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    
    {
        var appSettingsSection = configuration.GetSection(nameof(JwksSettings));
        services.Configure<JwksSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<JwksSettings>() ?? new();
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.SetJwksOptions(new JwkOptions(appSettings.JwksEndpoint));
            });
    }
}
