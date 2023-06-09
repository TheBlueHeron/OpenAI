﻿using System.Globalization;
using BlueHeron.OpenAI.Interfaces;
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
    /// The <see cref="ChatContextCollection"/> with <see cref="ChatContext"/>s, available for the current user.
    /// </summary>
    [ObservableProperty()]
    private ChatContextCollection _availableContexts = new();

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

    [ObservableProperty()]
    private AppOptions _options;

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
    /// Creates a new <see cref="OpenAIViewModel"/> and deserializes stored chats, if present.
    /// </summary>
    /// <param name="connector">The <see cref="OpenAIService"/> to use</param>
    /// <param name="speech">The <see cref="ISpeechToText"/> to use</param>
    public OpenAIViewModel(OpenAIService connector, ISpeechToText speech)
    {
        _chats = LocalStore.Load<ChatCollection>(nameof(Chats));
        _chats ??= new() { new Chat(ChatContext.Default) { IsActive = true } };
        _options = LocalStore.Load<AppOptions>(nameof(Options));
        _options ??= new();
        ActiveChat = _chats.First(c => c.IsActive);
        mConnector = connector;
        mSpeech = speech;
        mSpeech.StateChanged += OnSpeechStateChanged;
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
    /// Gets the available <see cref="ChatContext"/>s for the current user.
    /// </summary>
    /// <returns>A <see cref="Task"/></returns>
    public async Task GetAvailableContextsAsync() // TODO: asynchronously retrieve available contexts from server
    {
        await Task.Run(() => {
            AvailableContexts.Clear();
            AvailableContexts.Add(ChatContext.Default);

            var ctx = new ChatContext("BIS", "Insert instructions for BIS data here...");
            ctx.QuestionHandler = IQuestionHandler.Default(ctx);
            ctx.AnswerHandler = IAnswerHandler.Default(ctx);
            AvailableContexts.Add(ctx);
        });
    }

    /// <summary>
    /// Stores the <see cref="Chats"/> and cleans up resources.
    /// </summary>
    public async Task<bool> Quit()
    {
        mTokenSource?.Dispose();
        LocalStore.Save(nameof(Chats), Chats);
        LocalStore.Save(nameof(Options), Options);

        return mSpeech is null || await mSpeech.Quit();
    }

    #endregion

    #region Commands

    /// <summary>
    /// The 'AddChat' command, that adds a new chat to the <see cref="Chats"/> collection and activates it.
    /// </summary>
    /// <param name="context">The <see cref="ChatContext"/> to use</param>
    [RelayCommand]
    private void AddChat(ChatContext context)
    {
        var newChat = new Chat(context) { IsActive = true };
        Chats.Add(newChat);
        ActivateChat(newChat);
    }

    /// <summary>
    /// The 'AnswerQuestion' command that calls <see cref="OpenAIService.Update(Chat)"/> and asynchronously and repeatedly updates the <see cref="Answer"/> property as it is received as a stream of string tokens.
    /// </summary>
    /// <param name="isSpoken">The question was raised through speech</param>
    [RelayCommand]
    private async Task AnswerQuestion(bool isSpoken)
    {
        if (ActiveChat.Messages.Count == 0)
        {
            ActiveChat.Messages.AddMessage(new ChatMessage(ActiveChat.Context.Context, ActiveChat.Context.Context, ChatMessageType.System, DateTime.UtcNow, false));
        }
        if (!mSentenceEndings.Contains(Question.Last().ToString()))
        {
            Question += _DOT; // prevent GPT from completing the input
        }
        ActiveChat.AddQuestion(Question, isSpoken);
        ClearQuestion();

        await ParseChatResponse(mConnector.Update(ActiveChat), isSpoken);
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
            var newChat = new Chat(ChatContext.Default) { IsActive = true };
            Chats.Add(newChat);
        }        
        ActivateChat(Chats.First());
    }

    /// <summary>
    /// Starts listening for speech input and generates the <see cref="Question"/> from it.
    /// </summary>
    [RelayCommand]
    private async Task Listen()
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
    /// Parses the response stream into sentences - speaking them if needed - and adds a <see cref="ChatMessage"/> of type <see cref="ChatMessageType.Answer"/> to the <see cref="ActiveChat"/>.
    /// </summary>
    /// <param name="response">The <see cref="IAsyncEnumerable{String}"/> returned by the <see cref="OpenAIService"/></param>
    /// <param name="isSpoken">The question was raised through speech</param>
    /// <returns>A <see cref="Task"/></returns>
    private async Task ParseChatResponse(IAsyncEnumerable<string> response, bool isSpoken)
    {
        var currentSentence = string.Empty;
        var isQuote = false;
        bool isCurrentNumeric;
        bool isPreviousNumeric;
        var currentToken = string.Empty;
        var isFirst = true;
        bool mustAddSpace;
        var curIndex = -1;
        string actualContent;
        var handleSentence = new Action(() => 
         {
             Answer.Sentences.Add(currentSentence);
            if (isSpoken)
            {
                SpeakSentence(currentSentence);
            }
            currentSentence = string.Empty;
            curIndex = -1;
            isFirst = true;
        });

        Answer = new(string.Empty, string.Empty, ChatMessageType.Answer, DateTime.UtcNow, isSpoken) { IsUpdating = true };
        ActiveChat.Messages.AddMessage(Answer);

        await foreach (var t in ActiveChat.Context.AnswerHandler.Transform(response, out actualContent))
        {
            if (!string.IsNullOrEmpty(t))
            {
                isCurrentNumeric = int.TryParse(t, out _);
                isPreviousNumeric = !isFirst && (currentToken == _DOT || int.TryParse(currentToken, out _));

                if (!isCurrentNumeric && currentToken == _DOT) // previous token was a number followed by a dot, therefore if the current token is not a number the dot is a line ending and not a decimal separator.
                {
                    handleSentence();
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
                    handleSentence();
                }
            }
        }
        if (!string.IsNullOrEmpty(currentSentence)) // no trailing dot, or the sentence ended with a number followed by a dot.
        {
            handleSentence();
        }
        Answer.ActualContent = actualContent?? Answer.DisplayedContent;
        Answer.TimeStampUTC = DateTime.UtcNow;
        Answer.IsUpdating = false;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Updates the UI when the state of the speech recognizer has changed.
    /// </summary>
    /// <param name="sender">The <see cref="ISpeechToText"/> implementation</param>
    /// <param name="e">The <see cref="SpeechRecognizerStateChangedEventArgs"/></param>
    private void OnSpeechStateChanged(object sender, SpeechRecognizerStateChangedEventArgs e)
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
            Answer.DisplayedContent += t;
            Thread.Sleep(_ANSWERUPDATEDELAY);
        });
    }

    #endregion
}