﻿using System.Globalization;
using BlueHeron.OpenAI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlueHeron.OpenAI.ViewModels;

/// <summary>
/// An <see cref="ObservableObject"/> that exposes bindable properties and <see cref="IRelayCommand"/>s for <see cref="ServiceConnector"/> and <see cref="ISpeechToText"/> functionality.
/// </summary>
public partial class OpenAIViewModel : ObservableObject
{
    #region Objects and variables

    private const string _MIC = "No microphone access!";
    private const string _SPC = " ";

    private readonly ChatCollection mChats;
    private readonly ServiceConnector mConnector;
    private readonly ISpeechToText mSpeech;
    private CancellationTokenSource mTokenSource;

    #endregion

    #region Properties

    /// <summary>
    /// The currently active <see cref="Chat"/>.
    /// </summary>
    [ObservableProperty()]
    private Chat _activeChat;

    /// <summary>
    /// The latest alert message generated by an operation.
    /// </summary>
    [ObservableProperty()]
    private string _alert = string.Empty;

    /// <summary>
    /// The latest answer received from the <see cref="ServiceConnector"/>.
    /// </summary>
    [ObservableProperty()]
    private ChatMessage _answer;

    /// <summary>
    /// Gets the <see cref="ChatCollection"/>.
    /// </summary>
    [ObservableProperty()]
    private ChatCollection _chats;

    /// <summary>
    /// The culture to use when converting speech to text.
    /// </summary>
    [ObservableProperty()]
    private string _culture = "en-us";

    /// <summary>
    /// Gets a boolean, determining whether the speech recognizer is currently listening.
    /// </summary>
    [ObservableProperty()]
    private bool _isListening = false;

    /// <summary>
    /// Gets a boolean, determining whether the speech recognizer is ready to start listening.
    /// </summary>
    [ObservableProperty()]
    private bool _isReadyToListen = true;

    /// <summary>
    /// The latest question posted to the <see cref="ServiceConnector"/>.
    /// </summary>
    [ObservableProperty()]
    private string _question = string.Empty;

    /// <summary>
    /// The latest answer received from the <see cref="ServiceConnector"/>, separated into sentences.
    /// </summary>
    [ObservableProperty()]
    private List<string> _sentences;

    /// <summary>
    /// The current state of the <see cref="ISpeechToText"/> implementation.
    /// </summary>
    [ObservableProperty()]
    private string _state = string.Empty;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="OpenAIViewModel"/>
    /// </summary>
    /// <param name="connector">The <see cref="ServiceConnector"/> to use</param>
    /// <param name="speech">The <see cref="ISpeechToText"/> to use</param>
    public OpenAIViewModel(ServiceConnector connector, ISpeechToText speech)
    {
        mChats = new()
        {
            new Chat() { IsActive = true, Title = Chat.DefaultName() }
        };
        ActiveChat = mChats.First();
        mConnector = connector;
        mSpeech = speech;
        mSpeech.StateChanged += OnStateChanged;
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    public async Task<bool> Quit()
    {
        mTokenSource?.Dispose();
        return mSpeech is null || await mSpeech.Quit();
    }

    #endregion

    #region Commands

    /// <summary>
    /// The 'AnswerQuestion' command that calls <see cref="ServiceConnector.Update(Chat)"/> and asynchronously and repeatedly updates the <see cref="Answer"/> property as it is received as a stream of string tokens.
    /// </summary>
    [RelayCommand]
    private async void AnswerQuestion()
    {
        var currentSentence = string.Empty;

        Sentences = new List<string>();
        if (ActiveChat.ChatMessages.Count == 0)
        {
            ActiveChat.ChatMessages.Add(new ChatMessage(ChatMessage.MSG_ASSISTANT, MessageType.System, DateTime.UtcNow, false));
        }
        ActiveChat.ChatMessages.Add(new ChatMessage(Question, MessageType.Question, DateTime.UtcNow, false));
        ClearQuestion();

        var response = mConnector.Update(ActiveChat);
        Answer = new ChatMessage(string.Empty, MessageType.Answer, DateTime.UtcNow, false);
        ActiveChat.ChatMessages.Add(Answer);
        await foreach (var t in response)
        {
            if (!string.IsNullOrEmpty(t))
            {
                _ = await UpdateAnswer(t);
                currentSentence += t;
                if (t == ".")
                {
                    Sentences.Add(currentSentence);
                    SpeakSentence(currentSentence);
                    currentSentence = string.Empty;
                }
            }
        }
        if (!string.IsNullOrEmpty(currentSentence)) // no trailing dot
        {
            Sentences.Add(currentSentence);
            SpeakSentence(currentSentence);
        }
        Answer.TimeStampUTC = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears the chat and starts a new one.
    /// </summary>
    [RelayCommand]
    private void ClearChat()
    {
        ClearQuestion();
        Answer = null;
    }

    /// <summary>
    /// Clears the question.
    /// </summary>
    [RelayCommand]
    private void ClearQuestion()
    {
        Question = string.Empty;
        ListenCancel();
    }

    /// <summary>
    /// Starts listening for speech input and generates the <see cref="Question"/> from it.
    /// </summary>
    [RelayCommand]
    private async void Listen()
    {
        var isAuthorized = await mSpeech.RequestPermissions();

        if (isAuthorized)
        {
            mTokenSource = new CancellationTokenSource();
            IsListening = true;
            IsReadyToListen = false;
            try
            {
                Question = await mSpeech.Listen(CultureInfo.GetCultureInfo(Culture), new Progress<string>(partialText =>
                    {
                        if (DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            Question = partialText;
                        }
                        else
                        {
                            Question += partialText + _SPC;
                        }
                    }), mTokenSource.Token);
            }
            catch (Exception ex)
            {
                Alert = ex.Message;
                IsListening = false;
                IsReadyToListen = true;
            }
        }
        else
        {
            Alert = _MIC;
            IsListening = false;
            IsReadyToListen = false;
        }
    }

    /// <summary>
    /// Stops listening to speech input.
    /// </summary>
    [RelayCommand]
    private void ListenCancel()
    {
        mTokenSource?.Cancel();
        IsListening = false;
        IsReadyToListen = true;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Updates the UI when the speech recognizer state has changed.
    /// </summary>
    /// <param name="sender">The <see cref="ISpeechToText"/> implementation</param>
    /// <param name="e">The <see cref="StateChangedEventArgs"/></param>
    private void OnStateChanged(object sender, StateChangedEventArgs e)
    {
        IsListening = e.IsListening;
        IsReadyToListen = e.IsReadyToListen;
        State = e.State;
    }

    /// <summary>
    /// Asynchronously speaks the given sentence.
    /// </summary>
    /// <param name="sentence">The sentence to speak</param>
    private static void SpeakSentence(string sentence)
    {
        Task.Run(async () =>
        {
            await TextToSpeech.Default.SpeakAsync(sentence);
        });
    }

    /// <summary>
    /// Asynchronously updates the <see cref="Answer"/> property.
    /// </summary>
    /// <param name="t">The next token</param>
    /// <returns><see langword="true"/></returns>
    private async Task<bool> UpdateAnswer(string t)
    {
        await Task.Run(() =>
        {
            Answer.Content += t;
            Thread.Sleep(25);
        });
        return true;
    }

    #endregion
}