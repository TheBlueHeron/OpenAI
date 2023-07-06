using BlueHeron.OpenAI.ViewModels;

namespace BlueHeron.OpenAI;

/// <summary>
/// Extension methods and functions.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers objects needed to use BlueHeron.OpenAI:
    /// 1. The <see cref="OpenAIService"/> (as a singleton object)
    /// 2. The <see cref="SpeechToTextImplementation"/> (as a singleton object)
    /// 3. The <see cref="OpenAIViewModel"/> (as a singleton object)
    /// </summary>
    /// <param name="builder">This <see cref="MauiAppBuilder"/></param>
    /// <returns>This <see cref="MauiAppBuilder"/></returns>
    public static MauiAppBuilder UseOpenAI(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<OpenAIService>()
        .AddSingleton(typeof(ISpeechToText), new SpeechToTextImplementation())
        .AddSingleton<OpenAIViewModel>();
        return builder;
    }
}