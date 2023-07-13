namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// Button that shows a popup.
/// </summary>
public class PopupButton : Button
{
    #region Objects and variables

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="PopupView"/> property.
    /// </summary>
    public static readonly BindableProperty PopupViewProperty = BindableProperty.Create(nameof(PopupView), typeof(PopupView), typeof(PopupButton));

    /// <summary>
    /// Event is raised after the popup display has completed.
    /// </summary>
    public event EventHandler<PopupCompletedEventArgs>? PopupCompleted;

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="PopupView"/> to show.
    /// </summary>
    public PopupView? PopupView
    {
        get => (PopupView?)GetValue(PopupViewProperty);
        set => SetValue(PopupViewProperty, value);
    }

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="PopupButton"/>.
    /// </summary>
    public PopupButton()
    {
        Clicked += DropDownButtonClicked;
    }

    #endregion

    #region Events

    /// <summary>
    /// Displays the popup and raises the <see cref="PopupCompleted"/> event.
    /// </summary>
    private async void DropDownButtonClicked(object? sender, EventArgs e)
    {
        if (PopupView != null)
        {
            var rst = await Popup.Display<object>(PopupView);
            PopupCompleted?.Invoke(this, new PopupCompletedEventArgs(rst));
        }
    }

    #endregion
}