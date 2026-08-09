using Altairis.Services.Mailing.Templating;
using Microsoft.Extensions.DependencyInjection;

namespace Altairis.Services.Mailing;

public static class AltairisServicesMailingTemplatingRegistrationExtensions {

    public static IServiceCollection AddResourceTemplatedMailerService(this IServiceCollection services, ResourceTemplatedMailerServiceOptions options) {
        services.AddSingleton(options);
        services.AddSingleton(typeof(ITemplatedMailerService), typeof(ResourceTemplatedMailerService));
        return services;
    }

}
