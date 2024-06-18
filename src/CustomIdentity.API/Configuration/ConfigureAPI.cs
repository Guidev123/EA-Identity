using CustomIdentity.API.Data;
using CustomIdentity.API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomIdentity.API.Configuration
{
    public static class ConfigureAPI
    {
        public static void AddApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(opt =>
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddJwtConfiguration(configuration);
            services.AddSwaggerConfig();

            // IDENTITY CONFIG
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddErrorDescriber<IdentityCustomMessages>()
                .AddDefaultTokenProviders();
        }

        public static void CustomMiddlewares(this IApplicationBuilder app, IWebHostEnvironment environment)
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
