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
    private SpeechRecognitionEngine speechRecognitionEngine;
    private SpeechRecognizer speechRecognizer;
    private string recognitionText;

    public Task<string> Listen(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
    {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            return ListenOnline(culture, recognitionResult, cancellationToken);
        }

        return ListenOffline(culture, recognitionResult, cancellationToken);
    }

    private async Task<string> ListenOnline(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
    {
        recognitionText = string.Empty;
        speechRecognizer = new SpeechRecognizer(new Language(culture.IetfLanguageTag));
        await speechRecognizer.CompileConstraintsAsync();

        var taskResult = new TaskCompletionSource<string>();
        speechRecognizer.ContinuousRecognitionSession.ResultGenerated += (s, e) =>
        {
            recognitionText += e.Result.Text;
            recognitionResult?.Report(e.Result.Text);
        };
        speechRecognizer.ContinuousRecognitionSession.Completed += (s, e) =>
        {
            switch (e.Status)
            {
                case SpeechRecognitionResultStatus.Success:
                    taskResult.TrySetResult(recognitionText);
                    break;
                case SpeechRecognitionResultStatus.UserCanceled:
                    taskResult.TrySetCanceled();
                    break;
                default:
                    taskResult.TrySetException(new Exception(e.Status.ToString()));
                    break;
            }
        };
        if (speechRecognizer.State == SpeechRecognizerState.Idle)
        {
            await speechRecognizer.ContinuousRecognitionSession.StartAsync();
            await using (cancellationToken.Register(async () =>
            {
                await StopRecording();
                taskResult.TrySetCanceled();
            }))
            {
                return await taskResult.Task;
            }
        }
        else
        {
            taskResult.TrySetCanceled();
            return await taskResult.Task;
        }
    }

    private async Task<string> ListenOffline(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
    {
        speechRecognitionEngine = new SpeechRecognitionEngine(culture);
        speechRecognitionEngine.LoadGrammarAsync(new DictationGrammar());
        speechRecognitionEngine.SpeechRecognized += (s, e) =>
        {
            recognitionResult?.Report(e.Result.Text);
        };
        speechRecognitionEngine.SetInputToDefaultAudioDevice();
        speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
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

    private async Task StopRecording()
    {
        try
        {
            await speechRecognizer?.ContinuousRecognitionSession.StopAsync();
        }
        catch
        {
            // ignored. Recording may be already stopped
        }
    }

    private void StopOfflineRecording()
    {
        speechRecognitionEngine?.RecognizeAsyncCancel();
    }

    public async ValueTask DisposeAsync()
    {
        await StopRecording();
        StopOfflineRecording();
        speechRecognitionEngine?.Dispose();
        speechRecognizer?.Dispose();
    }

    public Task<bool> RequestPermissions()
    {
        return Task.FromResult(true);
    }
}