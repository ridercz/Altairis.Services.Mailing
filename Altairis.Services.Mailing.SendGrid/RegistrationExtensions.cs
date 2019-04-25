using Altairis.Services.Mailing;
using Altairis.Services.Mailing.SendGrid;
using System;

namespace Microsoft.Extensions.DependencyInjection {
    public static class AltairisServicesMailingSendGridRegistrationExtensions {

        public static IServiceCollection AddSendGridMailerService(this IServiceCollection services, SendGridMailerServiceOptions options) {
            services.AddSingleton<IMailerService>(new SendGridMailerService(options));
            return services;
        }

        public static IServiceCollection AddSendGridMailerService(this IServiceCollection services, string apiKey) {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(apiKey));

            var options = new SendGridMailerServiceOptions { ApiKey = apiKey };
            services.AddSingleton<IMailerService>(new SendGridMailerService(options));
            return services;
        }

    }
}
