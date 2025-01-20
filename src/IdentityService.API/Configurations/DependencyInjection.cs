using IdentityService.API.Extensions;
using IdentityService.API.Middlewares;
using IdentityService.API.Services;
using SendGrid.Extensions.DependencyInjection;
using SharedLib.MessageBus;
using System.Reflection;

namespace IdentityService.API.Configurations;

public static class DependencyInjection
{
    public static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.AddHandlers();
        builder.AddMessageBusConfiguration();
        builder.AddModelSettings();
        builder.AddTokenService();
        builder.AddEmailServices();
        builder.AddMiddlewares();
    }

    public static void AddHandlers(this WebApplicationBuilder builder) =>
        builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    
    public static void AddModelSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AppTokenSettings>(builder.Configuration.GetSection(nameof(AppTokenSettings)));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));
    }

    public static void AddEmailServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSendGrid(x =>
        {
            x.ApiKey = builder.Configuration.GetValue<string>("EmailSettings:ApiKey");
        });
        builder.Services.AddScoped<IEmailService, EmailService>();
    }

    public static void AddTokenService(this WebApplicationBuilder builder) =>
        builder.Services.AddTransient<ITokenService, TokenService>();

    public static void AddMiddlewares(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<GlobalExceptionMiddleware>();
    }

    public static void AddMessageBusConfiguration(this WebApplicationBuilder builder) =>
        builder.Services.AddMessageBus(builder.Configuration.GetMessageQueueConnection("MessageBus"));
}
