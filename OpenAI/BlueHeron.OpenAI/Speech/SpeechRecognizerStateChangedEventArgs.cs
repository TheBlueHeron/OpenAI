
namespace BlueHeron.OpenAI;

/// <summary>
/// <see cref="EventArgs"/> for the <see cref="ISpeechToText.StateChanged"/> event.
/// </summary>
public class SpeechRecognizerStateChangedEventArgs : EventArgs
{
    #region Properties

    /// <summary>
    /// Gets a value determining whether the recognizer is listening.
    /// </summary>
    public bool IsListening { get; }

    /// <summary>
    /// Gets a value determining whether the recognizer is ready to start listening.
    /// </summary>
    public bool IsReadyToListen { get; }

    /// <summary>
    /// Gets the current state of the recognizer.
    /// </summary>
    public string State { get; }

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="SpeechRecognizerStateChangedEventArgs"/> object.
    /// </summary>
    /// <param name="isListening">Boolean, determining whether the recognizer is listening</param>
    /// <param name="isReadyToListen">Boolean, determining whether the recognizer is ready to start listening</param>
    /// <param name="state">The current state of the recognizer</param>
    public SpeechRecognizerStateChangedEventArgs(bool isListening, bool isReadyToListen, string state)
    {
        IsListening = isListening;
        IsReadyToListen = isReadyToListen;
        State = state;
    }

    #endregion
}