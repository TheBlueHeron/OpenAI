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
    /// The contents of the message.
    /// </summary>
    [ObservableProperty]
    private string _content;

    /// <summary>
    /// Gets or sets a boolean, determining whether this message was or must be spoken.
    /// </summary>
    [ObservableProperty]
    private bool _isSpoken;

    /// <summary>
    /// The <see cref="OpenAI.MessageType"/> of the message.
    /// </summary>
    [ObservableProperty]
    private MessageType _messageType;

    /// <summary>
    /// The <see cref="Content"/>, separated into sentences.
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
    private DateTime _timeStamp;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new ChatMessage.
    /// </summary>
    /// <param name="content">The contents of the message</param>
    /// <param name="messageType">The <see cref="OpenAI.MessageType"/> of the message</param>
    /// <param name="timeStampUTC">The date time this message was posted, as UTC time</param>
    /// <param name="isSpoken">A boolean, determining whether this message was or must be spoken</param>
    public ChatMessage(string content, MessageType messageType, DateTime timeStampUTC, bool isSpoken)
    {
        _content = content;
        _messageType = messageType;
        _timeStampUTC = timeStampUTC;
        _timeStamp = _timeStampUTC.ToLocalTime();
        _isSpoken = isSpoken;
        _sentences = new List<string>();
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Returns this message as a <see cref="Message"/>.
    /// </summary>
    /// <returns>A <see cref="Message"/></returns>
    public Message AsOpenAIMessage()
    {
        return MessageType == MessageType.Question ? 
            new Message(Role.User, Content) :
            MessageType == MessageType.Answer ?
                new Message(Role.Assistant, Content) :
                new Message(Role.System, Content);
    }

    /// <summary>
    /// Overridden to return a debug-friendly representation of this message.
    /// </summary>
    public override string ToString()
    {
        return $"{TimeStampUTC} - {MessageType} - {Content}";
    }

    #endregion
}