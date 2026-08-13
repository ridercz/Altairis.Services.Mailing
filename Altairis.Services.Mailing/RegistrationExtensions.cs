using Microsoft.Extensions.DependencyInjection;

namespace Altairis.Services.Mailing;

public static class SystemNetMailRegistrationExtensions {

    public static IServiceCollection AddPickupFolderMailerService(this IServiceCollection services, PickupFolderMailerServiceOptions options) {
        services.AddSingleton<IMailerService>(new PickupFolderMailerService(options));
        return services;
    }

    public static IServiceCollection AddPickupFolderMailerService(this IServiceCollection services, string pickupFolderName) {
        ArgumentNullException.ThrowIfNull(pickupFolderName);
        if (string.IsNullOrWhiteSpace(pickupFolderName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(pickupFolderName));

        var options = new PickupFolderMailerServiceOptions { PickupFolderName = pickupFolderName };
        services.AddSingleton<IMailerService>(new PickupFolderMailerService(options));
        return services;
    }

    public static IServiceCollection AddSmtpServerMailerService(this IServiceCollection services, SmtpServerMailerServiceOptions options) {
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton<IMailerService>(new SmtpServerMailerService(options));
        return services;
    }

}
