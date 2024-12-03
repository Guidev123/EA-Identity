using EA.CommonLib.MessageBus;
using IdentityService.API.Data;
using IdentityService.API.Interfaces;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API.Middlewares
{
    public static class ApiMiddlewares
    {
        public static void AddDbContextMiddleware(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AuthenticationDbContext>(opt =>
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
        public static void AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
        }
        public static void AddDependenciesMiddleware(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
            builder.Services.AddTransient<ISecurityService, SecurityService>();
        }

        public static void AddIdentityMiddleware(this WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AuthenticationDbContext>()
                    .AddDefaultTokenProviders();
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
        }
    }

}
