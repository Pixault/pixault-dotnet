using Microsoft.Extensions.DependencyInjection;

namespace Pixault.Client;

/// <summary>
/// DI registration extensions for Pixault client services.
/// </summary>
public static class PixaultServiceExtensions
{
    /// <summary>
    /// Adds Pixault image services to the DI container.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services.AddPixault(options =>
    /// {
    ///     options.BaseUrl = "https://img.pixault.io";
    ///     options.DefaultProject = "barber";
    ///     options.ApiKey = builder.Configuration["Pixault:ApiKey"];
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddPixault(
        this IServiceCollection services, Action<PixaultOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<PixaultImageService>();

        services.AddHttpClient<PixaultUploadClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PixaultOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            if (!string.IsNullOrEmpty(options.ApiKey))
                client.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        });

        services.AddHttpClient<PixaultAdminClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PixaultOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            if (!string.IsNullOrEmpty(options.ApiKey))
                client.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        });

        return services;
    }
}
