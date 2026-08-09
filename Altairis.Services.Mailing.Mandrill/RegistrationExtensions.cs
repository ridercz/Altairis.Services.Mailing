using Altairis.Services.Mailing.Mandrill;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Altairis.Services.Mailing;

public static class AltairisServicesMailingMandrillRegistrationExtensions {

    public static IServiceCollection AddMandrillMailerService(this IServiceCollection services, MandrillMailerServiceOptions options) {
        services.AddSingleton<IMailerService>(new MandrillMailerService(options));
        return services;
    }

    public static IServiceCollection AddMandrillMailerService(this IServiceCollection services, string apiKey) {
        ArgumentNullException.ThrowIfNull(apiKey);
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(apiKey));

        var options = new MandrillMailerServiceOptions { ApiKey = apiKey };
        services.AddSingleton<IMailerService>(new MandrillMailerService(options));
        return services;
    }

}
