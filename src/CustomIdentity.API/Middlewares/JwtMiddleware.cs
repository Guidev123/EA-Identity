using CustomIdentity.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CustomIdentity.API.Middlewares
{
    public static class JwtMiddleware
    {
        public static void AddJwtConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>() ?? new();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true; 
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    // VALIDATIONS 
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor
                };
            });
        }
    }
}
