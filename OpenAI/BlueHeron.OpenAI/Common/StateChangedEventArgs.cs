
namespace BlueHeron.OpenAI;

/// <summary>
/// <see cref="EventArgs"/> for the <see cref="ISpeechToText.StateChanged"/> event.
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    #region Properties

    /// <summary>
    /// Gets a value determining whether the recognizer is listening.
    /// </summary>
    public bool IsListening { get; }

    /// <summary>
    /// Gets a value determining whether the recognizer is ready to start listening.
    /// </summary>
    public bool IsReasyForListen { get; }

    /// <summary>
    /// Gets the current state of the recognizer.
    /// </summary>
    public string State { get; }

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="StateChangedEventArgs"/> object.
    /// </summary>
    /// <param name="isListening">Boolean, determining whether the recognizer is listening</param>
    /// <param name="isReadyForListen">Boolean, determining whether the recognizer is ready to start listening</param>
    /// <param name="state">The current state of the recognizer</param>
    public StateChangedEventArgs(bool isListening, bool isReadyForListen, string state)
    {
        IsListening = isListening;
        IsReasyForListen = isReadyForListen;
        State = state;
    }

    #endregion
}