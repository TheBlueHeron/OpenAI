namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// Base class for popups.
/// </summary>
public partial class PopupView : ContentPage
{
    #region Objects and variables

    /// <summary>
    /// <see cref="TaskCompletionSource{Object}"/> to handle asynchronous display of the popup.
    /// </summary>
    internal TaskCompletionSource<object>? ReturnResultTask;

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="ForegroundColor"/> property.
    /// </summary>
    public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(propertyName: nameof(ForegroundColor), returnType: typeof(Color), declaringType: typeof(PopupView), defaultValue: Colors.Transparent, defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="HorizontalOptions"/> property.
    /// </summary>
    public static readonly BindableProperty HorizontalOptionsProperty = BindableProperty.Create(propertyName: nameof(HorizontalOptions), returnType: typeof(LayoutOptions), declaringType: typeof(PopupView), defaultValue: LayoutOptions.Center, defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="Margin"/> property.
    /// </summary>
    public static readonly BindableProperty MarginProperty = BindableProperty.Create(propertyName: nameof(Margin), returnType: typeof(Thickness), declaringType: typeof(PopupView), defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="VerticalOptions"/> property.
    /// </summary>
    public static readonly BindableProperty VerticalOptionsProperty = BindableProperty.Create(propertyName: nameof(VerticalOptions), returnType: typeof(LayoutOptions), declaringType: typeof(PopupView), defaultValue: LayoutOptions.Center, defaultBindingMode: BindingMode.OneWay);

    #endregion

    #region Properties

    /// <summary>
    /// The foreground color of the modal view.
    /// </summary>
    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal alignment and expansion.
    /// </summary>
    public LayoutOptions HorizontalOptions
    {
        get => (LayoutOptions)GetValue(HorizontalOptionsProperty);
        set => SetValue(HorizontalOptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin between the modal view and the app border.
    /// </summary>
    public Thickness Margin
    {
        get => (Thickness)GetValue(MarginProperty);
        set => SetValue(MarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment and expansion.
    /// </summary>
    public LayoutOptions VerticalOptions
    {
        get => (LayoutOptions)GetValue(VerticalOptionsProperty);
        set => SetValue(VerticalOptionsProperty, value);
    }

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="PopupView"/>.
    /// </summary>
    public PopupView()
    {
        InitializeComponent();
        BackgroundColor = Color.FromArgb("#80000000");
        ForegroundColor = Application.Current?.RequestedTheme == AppTheme.Light ? Colors.White : Color.FromArgb("#ff282828");
    }

    #endregion

}