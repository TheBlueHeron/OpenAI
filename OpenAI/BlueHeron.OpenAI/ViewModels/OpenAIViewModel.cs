﻿using System.Globalization;
using BlueHeron.OpenAI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlueHeron.OpenAI.ViewModels;

/// <summary>
/// An <see cref="ObservableObject"/> that exposes bindable properties and <see cref="IRelayCommand"/>s for <see cref="OpenAIService"/> and <see cref="ISpeechToText"/> functionality.
/// </summary>
public partial class OpenAIViewModel : ObservableObject
{
    #region Objects and variables

    private const int _ANSWERUPDATEDELAY = 25; // enhance the effect of 'live writing' of the answer
    private const string _CHATSFILE = "chats.json";
    private const string _DEFAULTCULTURE = "en-us";
    private const string _DOT = ".";
    private const string _MIC = "No microphone access!";
    private const string _QUOTE = "\"";
    private const char _SPC = ' ';

    private readonly OpenAIService mConnector;
    private readonly HashSet<string> mSentenceEndings = new() { _DOT, "?", "!" };
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
    /// The latest answer received from the <see cref="OpenAIService"/>.
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
    private string _culture = _DEFAULTCULTURE;

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
    /// The latest question posted to the <see cref="OpenAIService"/>.
    /// </summary>
    [ObservableProperty()]
    private string _question = string.Empty;

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
    /// <param name="connector">The <see cref="OpenAIService"/> to use</param>
    /// <param name="speech">The <see cref="ISpeechToText"/> to use</param>
    public OpenAIViewModel(OpenAIService connector, ISpeechToText speech)
    {
        var path = Path.Combine(FileSystem.Current.AppDataDirectory, _CHATSFILE);

        if (File.Exists(path))
        {
            try
            {
                _chats = ChatCollection.FromJson(File.ReadAllText(path));
            }
            catch { } // ignore
        }
        _chats ??= new()
        {
            new Chat() { IsActive = true, Title = Chat.DefaultName() }
        };
        ActiveChat = _chats.First(c => c.IsActive);
        mConnector = connector;
        mSpeech = speech;
        mSpeech.StateChanged += OnStateChanged;
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Activates the given <see cref="Chat"/>.
    /// </summary>
    /// <param name="chat">The <see cref="Chat"/> to activate</param>
    public void ActivateChat(Chat chat)
    {
        if (chat != null)
        {
            if (ActiveChat != null)
            {
                ActiveChat.IsActive = false;
            }
            chat.IsActive = true;
            if (ActiveChat != chat)
            {
                ActiveChat = chat;
            }
        }
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    public async Task<bool> Quit()
    {
        mTokenSource?.Dispose();
        try
        {
            File.WriteAllText(Path.Combine(FileSystem.Current.AppDataDirectory, _CHATSFILE), Chats.ToJson());
        }
        catch { } // ignore

        return mSpeech is null || await mSpeech.Quit();
    }

    #endregion

    #region Commands

    /// <summary>
    /// The 'AddChat' command, that adds a new chat to the <see cref="Chats"/> collection and activates it.
    /// </summary>
    [RelayCommand]
    private void AddChat()
    {
        var newChat = new Chat() { IsActive = true, Title = Chat.DefaultName() };
        Chats.Add(newChat);
        ActivateChat(newChat);
    }

    /// <summary>
    /// The 'AnswerQuestion' command that calls <see cref="OpenAIService.Update(Chat)"/> and asynchronously and repeatedly updates the <see cref="Answer"/> property as it is received as a stream of string tokens.
    /// </summary>
    [RelayCommand]
    private async void AnswerQuestion()
    {
        if (ActiveChat.Messages.Count == 0)
        {
            ActiveChat.Messages.Add(new ChatMessage(ChatMessage.MSG_ASSISTANT, MessageType.System, DateTime.UtcNow, false));
        }
        if (!mSentenceEndings.Contains(Question.Last().ToString()))
        {
            Question += _DOT; // prevent GPT from completing the input
        }
        ActiveChat.Messages.Add(new ChatMessage(Question, MessageType.Question, DateTime.UtcNow, false));
        ClearQuestion();

        await ParseChatResponse(mConnector.Update(ActiveChat));        
    }

    /// <summary>
    /// Clears the currently active chat.
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
    /// The 'DeleteChat' command, that deletes the active chat from the <see cref="Chats"/> collection.
    /// The first <see cref="Chat"/> in the collection will become active.
    /// If the collection has become empty, a new <see cref="Chat"/> is added and activated.
    /// </summary>
    [RelayCommand]
    private void DeleteChat()
    {
        Chats.Remove(ActiveChat);
        if (!Chats.Any())
        {
            var newChat = new Chat() { IsActive = true, Title = Chat.DefaultName() };
            Chats.Add(newChat);
        }        
        ActivateChat(Chats.First());
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

    /// <summary>
    /// Parses the response stream into sentences - speaking them if needed - and adds a <see cref="ChatMessage"/> of type <see cref="MessageType.Answer"/> to the <see cref="ActiveChat"/>.
    /// </summary>
    /// <param name="response">The <see cref="IAsyncEnumerable{string}"/> returned by the <see cref="OpenAIService"/></param>
    /// <returns>A <see cref="Task"/></returns>
    private async Task ParseChatResponse(IAsyncEnumerable<string> response)
    {
        var currentSentence = string.Empty;
        var isQuote = false;
        bool isCurrentNumeric;
        bool isPreviousNumeric;
        var currentToken = string.Empty;
        var isFirst = true;
        bool mustAddSpace;
        var curIndex = -1;

        Answer = new ChatMessage(string.Empty, MessageType.Answer, DateTime.UtcNow, false);
        ActiveChat.Messages.Add(Answer);

        await foreach (var t in response)
        {
            if (!string.IsNullOrEmpty(t))
            {
                isCurrentNumeric = int.TryParse(t, out _);
                isPreviousNumeric = !isFirst && (currentToken == _DOT || int.TryParse(currentToken, out _));

                if (!isCurrentNumeric && currentToken == _DOT) // previous token was a number followed by a dot, therefore if the current token is not a number the dot is a line ending and not a decimal separator.
                {
                    Answer.Sentences.Add(currentSentence);
                    SpeakSentence(currentSentence);
                    currentSentence = string.Empty;
                    curIndex = -1;
                    isFirst = true;
                }

                mustAddSpace = isCurrentNumeric && !isPreviousNumeric && !isFirst; // numbers within a sentence are parsed without preceding space character
                currentToken = mustAddSpace ? _SPC + t : t;
                currentSentence += currentToken;
                curIndex += t.Length + (mustAddSpace ? 1 : 0);
                isFirst = false;

                await UpdateAnswer(currentToken);

                if (t == _QUOTE) // treated quoted sentences as part of the containing sentence (i.e. ignore line endings)
                {
                    isQuote = !isQuote;
                    continue;
                }

                if (!isQuote && !isPreviousNumeric && mSentenceEndings.Contains(t)) // detect end of sentence versus possible decimal separator
                {
                    Answer.Sentences.Add(currentSentence);
                    SpeakSentence(currentSentence);
                    currentSentence = string.Empty;
                    curIndex = -1;
                    isFirst = true;
                }
            }
        }
        if (!string.IsNullOrEmpty(currentSentence)) // no trailing dot, or the sentence ended with a number followed by a dot.
        {
            Answer.Sentences.Add(currentSentence);
            SpeakSentence(currentSentence);
        }
        Answer.TimeStampUTC = DateTime.UtcNow;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Updates the UI when the state of the speech recognizer has changed.
    /// </summary>
    /// <param name="sender">The <see cref="ISpeechToText"/> implementation</param>
    /// <param name="e">The <see cref="SpeechRecognizerStateChangedEventArgs"/></param>
    private void OnStateChanged(object sender, SpeechRecognizerStateChangedEventArgs e)
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
    private async Task UpdateAnswer(string t)
    {
        await Task.Run(() =>
        {
            Answer.Content += t;
            Thread.Sleep(_ANSWERUPDATEDELAY);
        });
    }

    #endregion
}