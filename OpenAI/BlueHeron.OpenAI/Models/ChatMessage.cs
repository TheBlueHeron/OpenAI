using System.ComponentModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI.Chat;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// Container for details on a particular message in a <see cref="Chat"/>.
/// </summary>
public partial class ChatMessage : ObservableObject
{
    #region Objects and variables

    public const string MSG_ASSISTANT = "You are a helpful assistant.";

    #endregion

    #region Properties

    /// <summary>
    /// The actual contents of the message, i.e. the contents that are posted to the OpenAI API.
    /// </summary>
    [ObservableProperty]
    private string _actualContent;

    /// <summary>
    /// The displayed contents of the message.
    /// </summary>
    [ObservableProperty]
    private string _displayedContent;

    /// <summary>
    /// Gets or sets a boolean, determining whether this message was or must be spoken.
    /// </summary>
    [ObservableProperty]
    private bool _isSpoken;

    /// <summary>
    /// Gets or sets a boolean, determining whether this message is currently being updated.
    /// </summary>
    [ObservableProperty]
    [property: JsonIgnore]
    private bool _isUpdating;

    /// <summary>
    /// The <see cref="OpenAI.ChatMessageType"/> of the message.
    /// </summary>
    [ObservableProperty]
    private ChatMessageType _messageType;

    /// <summary>
    /// The <see cref="DisplayedContent"/>, separated into sentences.
    /// </summary>
    [ObservableProperty()]
    private List<string> _sentences;

    /// <summary>
    /// The date time this message was posted, as UTC time.
    /// </summary>
    [ObservableProperty]
    private DateTime _timeStampUTC;

    /// <summary>
    /// The date time this message was posted, as local time.
    /// </summary>
    [ObservableProperty]
    [property: JsonIgnore] // will be populated in constructor when deserializing
    private DateTime _timeStamp;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new ChatMessage.
    /// </summary>
    /// <param name="actualContent">The actual contents of the message, i.e. the contents that are posted to the OpenAI API</param>
    /// <param name="displayedContent">The displayed contents of the message</param>
    /// <param name="messageType">The <see cref="OpenAI.ChatMessageType"/> of the message</param>
    /// <param name="timeStampUTC">The date time this message was posted, as UTC time</param>
    /// <param name="isSpoken">A boolean, determining whether this message was or must be spoken</param>
    public ChatMessage(string actualContent, string displayedContent, ChatMessageType messageType, DateTime timeStampUTC, bool isSpoken)
    {
        _actualContent = actualContent;
        _displayedContent = displayedContent;
        _messageType = messageType;
        TimeStampUTC = timeStampUTC; // induce OnPropertyChanged call to set TimeStamp to local time
        _isSpoken = isSpoken;
        _sentences = new List<string>();
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Returns this <see cref="ChatMessage"/> as a <see cref="Message"/>, based on its <see cref="ChatMessage.ActualContent"/>.
    /// </summary>
    /// <returns>A <see cref="Message"/></returns>
    public Message AsOpenAIMessage()
    {
        return MessageType == ChatMessageType.Question ? 
            new Message(Role.User, ActualContent) :
            MessageType == ChatMessageType.Answer ?
                new Message(Role.Assistant, ActualContent) :
                new Message(Role.System, ActualContent);
    }

    /// <summary>
    /// Overridden to return a debug-friendly representation of this message.
    /// </summary>
    public override string ToString()
    {
        return $"{TimeStampUTC} - {MessageType} - {DisplayedContent}";
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Sets <see cref="TimeStamp"/> to local time when <see cref="TimeStampUTC"/> has changed.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/></param>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(TimeStampUTC))
        {
            TimeStamp = TimeStampUTC.ToLocalTime();
        }
    }

    #endregion
}