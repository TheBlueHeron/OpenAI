namespace BlueHeron.OpenAI;

/// <summary>
/// The app.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the application and sets the <see cref="MainPage"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/> used for dependency injection</param>
    public App(IServiceProvider services)
    {
        InitializeComponent();
        MainPage = services.GetService<MainPage>();
    }

    /// <summary>
    /// Creates the main application window and sets the title and dimensions properties.
    /// </summary>
    /// <param name="activationState">The current <see cref="IActivationState"/></param>
    /// <returns>A <see cref="Window"/></returns>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        
        window.Title = "BlueHeron OpenAI Playground";
        window.MinimumWidth = 360;
        window.MinimumHeight = 400;
        window.Width = window.MinimumWidth;
        window.Height = window.MinimumHeight;

        return window;
    }
}