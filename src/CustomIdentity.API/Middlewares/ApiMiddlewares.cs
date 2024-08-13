using CustomIdentity.API.Data;
using CustomIdentity.API.Extensions;
using CustomIdentity.API.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CustomIdentity.API.Middlewares
{
    public static class ApiMiddlewares
    {
        public static void AddDbContextMiddleware(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AuthDbContext>(opt =>
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }
        public static void AddEnviromentMiddleware(this WebApplicationBuilder builder)
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
        public static void AddDependenciesMiddleware(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IJasonWebToken, JasonWebToken>();
        }

        public static void AddIdentityMiddleware(this WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddErrorDescriber<IdentityCustomMessages>()
                    .AddDefaultTokenProviders();
        }

        public static void UseSecurity(this IApplicationBuilder app, IWebHostEnvironment environment)
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
        }
    }

}
