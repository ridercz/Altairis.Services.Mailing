﻿using Altairis.Services.Mailing;
using Altairis.Services.Mailing.Rfc2822;
using System;

namespace Microsoft.Extensions.DependencyInjection {
    public static class AltairisServicesMailingRfc2822RegistrationExtensions {

        public static IServiceCollection AddPickupFolderMailerService(this IServiceCollection services, PickupFolderMailerServiceOptions options) {
            services.AddSingleton<IMailerService>(new PickupFolderMailerService(options));
            return services;
        }

        public static IServiceCollection AddPickupFolderMailerService(this IServiceCollection services, string pickupFolderName) {
            if (pickupFolderName == null) throw new ArgumentNullException(nameof(pickupFolderName));
            if (string.IsNullOrWhiteSpace(pickupFolderName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(pickupFolderName));

            var options = new PickupFolderMailerServiceOptions { PickupFolderName = pickupFolderName };
            services.AddSingleton<IMailerService>(new PickupFolderMailerService(options));
            return services;
        }

        public static IServiceCollection AddSmtpServerMailerService(this IServiceCollection services, SmtpServerMailerServiceOptions options) {
            services.AddSingleton<IMailerService>(new SmtpServerMailerService(options));
            return services;
        }

    }
}
