using System.Globalization;

namespace BlueHeron.OpenAI;

/// <summary>
/// Interface definition for objects that are able to convert speech to text.
/// </summary>
/// <seealso>https://github.com/jfversluis/MauiSpeechToTextSample</seealso>
public interface ISpeechToText
{
    /// <summary>
    /// Requests the appropriate permissions needed for the task.
    /// </summary>
    /// <returns>Boolean, <see langword="true"/> if the appropriate permissions were acquired.</returns>
    Task<bool> RequestPermissions();

    /// <summary>
    /// Asynchronously listens to speech input and converts it to text.
    /// </summary>
    /// <param name="culture">The current <see cref="CultureInfo"/></param>
    /// <param name="recognitionResult">The result of the oepration as an <see cref="IProgress{String}"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The converted speech</returns>
    Task<string> Listen(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken);

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    Task<bool> Quit();

    /// <summary>
    /// Event is raised when the state of the inner speech recognizer has changed.
    /// </summary>
    public event EventHandler<SpeechRecognizerStateChangedEventArgs> StateChanged;
}