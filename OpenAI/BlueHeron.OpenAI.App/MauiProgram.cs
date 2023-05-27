using Microsoft.Extensions.Logging;
using Microsoft.Maui.Foldable;

namespace BlueHeron.OpenAI;

/// <summary>
/// Configures the <see cref="MauiApp"/>.
/// </summary>
public static partial class MauiProgram
{
    /// <summary>
    /// Configures the <see cref="MauiApp"/>.
    /// </summary>
    /// <returns>The <see cref="MauiApp"/></returns>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
               .UseFoldable()
               .UseMauiCommunityToolkit()
               .UseMauiCommunityToolkitMarkup()
               .UseMauiCommunityToolkitMediaElement()
               .UseOpenAI()
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                   fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                   fonts.AddFont("Segoe.ttf", "Segoe MDL2 Assets");
               });

        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}