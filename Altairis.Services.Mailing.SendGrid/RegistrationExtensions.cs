using Altairis.Services.Mailing.SendGrid;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Altairis.Services.Mailing;

public static class AltairisServicesMailingSendGridRegistrationExtensions {

    public static IServiceCollection AddSendGridMailerService(this IServiceCollection services, SendGridMailerServiceOptions options) {
        services.AddSingleton<IMailerService>(new SendGridMailerService(options));
        return services;
    }

    public static IServiceCollection AddSendGridMailerService(this IServiceCollection services, string apiKey) {
        ArgumentNullException.ThrowIfNull(apiKey);
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(apiKey));

        var options = new SendGridMailerServiceOptions { ApiKey = apiKey };
        services.AddSingleton<IMailerService>(new SendGridMailerService(options));
        return services;
    }

}
