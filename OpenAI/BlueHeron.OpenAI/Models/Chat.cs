using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI.Chat;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// Container for a collection of <see cref="ChatMessage"/> objects.
/// </summary>
public partial class Chat : ObservableObject
{
    #region Properties

    /// <summary>
    /// An <see cref="ObservableCollection{ChatMessage}"/>.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ChatMessage> _chatMessages = new();

    /// <summary>
    /// Gets or sets a boolean, determining whether this chat is active.
    /// </summary>
    [ObservableProperty()]
    private bool _isActive;

    /// <summary>
    /// The date time the last message was posted, as UTC time.
    /// </summary>
    private DateTime? TimeStampUTC => ChatMessages.LastOrDefault()?.TimeStampUTC;

    /// <summary>
    /// The name or title of this chat.
    /// </summary>
    [ObservableProperty()]
    private string _title;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new Chat.
    /// </summary>
    public Chat()
    {
        _chatMessages.Add(new ChatMessage(ChatMessage.MSG_ASSISTANT, MessageType.System, DateTime.UtcNow, false));
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Returns this chat as an <see cref="IList{Message}"/>.
    /// </summary>
    /// <returns>An <see cref="IList{Message}"/></returns>
    public IList<Message> AsOpenAIChat()
    {
        return ChatMessages.Select(c => c.AsOpenAIMessage()).ToList();
    }

    /// <summary>
    /// Overridden to return the <see cref="Title"/>.
    /// </summary>
    public override string ToString() => Title;

    #endregion
}