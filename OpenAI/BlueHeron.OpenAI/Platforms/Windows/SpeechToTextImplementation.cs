using System.Globalization;
using System.Speech.Recognition;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using SpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;

namespace BlueHeron.OpenAI;

/// <summary>
/// System speech recognition must be enabled: https://support.unity.com/hc/en-us/articles/212792423-Why-I-am-getting-a-StartAsync-error-running-my-app-that-uses-Windows-Speech-
/// Also: the application needs the following capabilities: runFullTrust, internetClient and microphone.
/// 
/// </summary>
public partial class SpeechToTextImplementation : ISpeechToText
{
    #region Objects and variables

    private SpeechRecognitionEngine mSpeechRecognitionEngine;
    private SpeechRecognizer mSpeechRecognizer;
    private string mRecognizedText;

    public event EventHandler<StateChangedEventArgs> StateChanged;

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Starts listening to speech input.
    /// </summary>
    /// <param name="culture">The <see cref="CultureInfo"/> to use</param>
    /// <param name="recognitionResult">The <see cref="IProgress{String}"/> holding the result</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>A string holding the recognized speech as flat text</returns>
    public Task<string> Listen(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
    {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            return ListenOnline(culture, recognitionResult, cancellationToken);
        }

        return ListenOffline(culture, recognitionResult, cancellationToken);
    }

    /// <summary>
    /// Clans up resources.
    /// </summary>
    public async Task<bool> Quit()
    {
        if (mSpeechRecognitionEngine != null)
        {
            StopOfflineRecording();
            mSpeechRecognitionEngine.Dispose();
            mSpeechRecognitionEngine = null;
        }
        if (mSpeechRecognizer != null)
        {
            await StopRecording();
            
            mSpeechRecognizer?.Dispose();
            mSpeechRecognizer = null;
        }
        return true;
    }

    /// <summary>
    /// Returns always <see langword="true"/> (for now).
    /// </summary>
    /// <returns>Boolean, <see langword="true"/> if the appropriate permissions were acquired</returns>
    public Task<bool> RequestPermissions()
    {
        return Task.FromResult(true);
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Starts listening using online continuous recogition.
    /// </summary>
    /// <param name="culture">The <see cref="CultureInfo"/> to use</param>
    /// <param name="recognitionResult">The <see cref="IProgress{String}"/> holding the result</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>A string holding the recognized speech as flat text</returns>
    private async Task<string> ListenOnline(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
    {
        var taskResult = new TaskCompletionSource<string>();

        if (mSpeechRecognizer == null)
        {
            mSpeechRecognizer = new SpeechRecognizer(new Language(culture.IetfLanguageTag));
            mSpeechRecognizer.Timeouts.BabbleTimeout = new TimeSpan(0, 0, 1);
            mSpeechRecognizer.Timeouts.EndSilenceTimeout = new TimeSpan(0, 0, 3);
            mSpeechRecognizer.Timeouts.InitialSilenceTimeout = new TimeSpan(0, 0, 5);
            mSpeechRecognizer.Constraints.Clear();

            await mSpeechRecognizer.CompileConstraintsAsync();

            mSpeechRecognizer.StateChanged += (s, e) =>
            {
                var isListening = true;
                var isReady = true;

                switch (e.State)
                {
                    case SpeechRecognizerState.Idle:
                        isListening = false;
                        isReady = true;
                        break;
                    case SpeechRecognizerState.Capturing:
                    case SpeechRecognizerState.Processing:
                    case SpeechRecognizerState.SoundStarted:
                    case SpeechRecognizerState.SoundEnded:
                    case SpeechRecognizerState.SpeechDetected:
                        isListening = true;
                        isReady = false;
                        break;
                    default: // Paused
                        isListening = false;
                        isReady = false;
                        break;
                }
                StateChanged?.Invoke(this, new StateChangedEventArgs(isListening, isReady, e.State.ToString()));
            };
            mSpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += (s, e) =>
            {
                mRecognizedText += e.Result.Text;
                recognitionResult?.Report(e.Result.Text);
            };
            mSpeechRecognizer.ContinuousRecognitionSession.Completed += async (s, e) =>
            {
                switch (e.Status)
                {
                    case SpeechRecognitionResultStatus.Success:
                        taskResult.TrySetResult(mRecognizedText);
                        break;
                    case SpeechRecognitionResultStatus.UserCanceled:
                        await Quit();
                        taskResult.TrySetCanceled();
                        break;
                    case SpeechRecognitionResultStatus.TimeoutExceeded:
                        await Quit();
                        taskResult.TrySetResult(mRecognizedText);
                        break;
                    default:
                        if (mSpeechRecognizer.State != SpeechRecognizerState.Idle)
                        {
                            await Quit();
                            taskResult.TrySetException(new Exception(e.Status.ToString()));
                        }
                        break;
                }
            };
        }

        mRecognizedText = string.Empty;
        mSpeechRecognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = new TimeSpan(0, 0, 5);
        await mSpeechRecognizer.ContinuousRecognitionSession.StartAsync();
        await using (cancellationToken.Register(async () =>
        {
            await Quit();
            taskResult.TrySetCanceled(cancellationToken);
        }))
        {
            return await taskResult.Task;
        }
    }

    /// <summary>
    /// Starts listening using an offline <see cref="Grammar"/>.
    /// </summary>
    /// <param name="culture">The <see cref="CultureInfo"/> to use</param>
    /// <param name="recognitionResult">The <see cref="IProgress{String}"/> holding the result</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>A string holding the recognized speech as flat text</returns>
    private async Task<string> ListenOffline(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
    {
        mSpeechRecognitionEngine = new SpeechRecognitionEngine(culture);
        mSpeechRecognitionEngine.LoadGrammarAsync(new DictationGrammar());
        mSpeechRecognitionEngine.SpeechRecognized += (s, e) => recognitionResult?.Report(e.Result.Text);
        mSpeechRecognitionEngine.SetInputToDefaultAudioDevice();
        mSpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        var taskResult = new TaskCompletionSource<string>();
        await using (cancellationToken.Register(() =>
        {
            StopOfflineRecording();
            taskResult.TrySetCanceled();
        }))
        {
            return await taskResult.Task;
        }
    }

    /// <summary>
    /// Stops the continuous listening.
    /// </summary>
    private async Task StopRecording()
    {
        try
        {
            if (mSpeechRecognizer != null && mSpeechRecognizer.State != SpeechRecognizerState.Idle)
            {
                await mSpeechRecognizer.ContinuousRecognitionSession?.StopAsync();
            }
        }
        catch
        {
            // ignored. Recording may be already stopped
        }
    }

    /// <summary>
    /// Stops listening.
    /// </summary>
    private void StopOfflineRecording()
    {
        mSpeechRecognitionEngine?.RecognizeAsyncCancel();
    }

    #endregion
}