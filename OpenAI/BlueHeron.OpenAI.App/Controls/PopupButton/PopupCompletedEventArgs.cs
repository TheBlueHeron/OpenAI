namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// <see cref="EventArgs"/> for the <see cref="PopupButton.PopupCompleted"/> event.
/// </summary>
public class PopupCompletedEventArgs : EventArgs
{
    private readonly object? mObject;

    /// <summary>
    /// The result object returned by the popup.
    /// </summary>
    public object? Result => mObject;

    /// <summary>
    /// Creates a new <see cref="PopupCompletedEventArgs"/>.
    /// </summary>
    /// <param name="result">The result object returned by the popup</param>
    public PopupCompletedEventArgs(object? result)
    {
        mObject = result;
    }
}