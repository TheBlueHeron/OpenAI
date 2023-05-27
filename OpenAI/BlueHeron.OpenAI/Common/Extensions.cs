using System.ComponentModel;
using System.Reflection;
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
    /// <param name="builder">This <see cref="MauiAppBuilder"/></param>
    /// <returns>This <see cref="MauiAppBuilder"/></returns>
    public static MauiAppBuilder UseOpenAI(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ServiceConnector>()
        .AddSingleton(typeof(ISpeechToText), new SpeechToTextImplementation())
        .AddSingleton<OpenAIViewModel>();
        return builder;
    }

}