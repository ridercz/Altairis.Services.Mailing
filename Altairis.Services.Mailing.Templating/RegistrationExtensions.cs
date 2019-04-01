using Altairis.Services.Mailing;
using Altairis.Services.Mailing.Templating;
using System;

namespace Microsoft.Extensions.DependencyInjection {
    public static class AltairisServicesMailingTemplatingRegistrationExtensions {

        public static IServiceCollection AddResourceTemplatedMailerService(this IServiceCollection services, ResourceTemplatedMailerServiceOptions options) {
            services.AddSingleton(options);
            services.AddSingleton(typeof(ITemplatedMailerService), typeof(ResourceTemplatedMailerService));
            return services;
        }



    }
}
