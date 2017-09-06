﻿using Altairis.Services.Mailing;
using Altairis.Services.Mailing.Rfc2822;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
    public static class RegistrationExtensions {

        public static IServiceCollection AddPickupFolderMailService(this IServiceCollection services, PickupFolderMailServiceOptions options) {
            services.AddSingleton<IMailerService>(new PickupFolderMailService(options));
            return services;
        }

        public static IServiceCollection AddPickupFolderMailService(this IServiceCollection services, string pickupFolderName) {
            if (pickupFolderName == null) throw new ArgumentNullException(nameof(pickupFolderName));
            if (string.IsNullOrWhiteSpace(pickupFolderName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(pickupFolderName));

            var options = new PickupFolderMailServiceOptions { PickupFolderName = pickupFolderName };
            services.AddSingleton<IMailerService>(new PickupFolderMailService(options));
            return services;
        }

        public static IServiceCollection AddSmtpServerMailService(this IServiceCollection services, SmtpServerMailServiceOptions options) {
            services.AddSingleton<IMailerService>(new SmtpServerMailService(options));
            return services;
        }

    }
}