using BlueHeron.OpenAI.ViewModels;

namespace BlueHeron.OpenAI;

/// <summary>
/// Extension methods and functions.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers services needed to use BlueHeron.OpenAI.
    /// </summary>
    /// <param name="services">This <see cref="IServiceCollection"/></param>
    /// <returns>This <see cref="IServiceCollection"/></returns>
    public static IServiceCollection UseOpenAI(this IServiceCollection services) =>
        services.AddSingleton<ServiceConnector>()
        .AddSingleton(typeof(ISpeechToText), new SpeechToTextImplementation())
        .AddSingleton<OpenAIViewModel>();
}