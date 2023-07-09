using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI.Chat;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// Container for a collection of <see cref="ChatMessage"/> objects that are handled by a specific <see cref="ChatContext"/>.
/// </summary>
public partial class Chat : ObservableObject
{
    #region Objects and variables

    private const string fmtDateTime = "yyyy-MM-dd HH:mm";

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="ChatContext"/> to use.
    /// </summary>
    [ObservableProperty]
    private ChatContext _context;

    /// <summary>
    /// A <see cref="ChatMessageCollection"/>.
    /// </summary>
    [ObservableProperty]
    private ChatMessageCollection _messages = new();

    /// <summary>
    /// Gets or sets a boolean, determining whether this chat is active.
    /// </summary>
    [ObservableProperty]
    private bool _isActive;

    /// <summary>
    /// The name or title of this chat.
    /// </summary>
    [ObservableProperty]
    private string _title;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new Chat.
    /// </summary>
    /// <param name="context">The <see cref="ChatContext"/> to use</param>
    public Chat(ChatContext context)
    {
        _context = context;
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Transforms and adds the given question the the <see cref="Messages"/> collection.
    /// </summary>
    /// <param name="question">The user input</param>
    /// <param name="isSpoken"></param>
    public void AddQuestion(string question, bool isSpoken)
    {
        Messages.AddMessage(new ChatMessage(Context.QuestionHandler.Transform(question), question, ChatMessageType.Question, DateTime.UtcNow, isSpoken));
    }

    /// <summary>
    /// Returns a default name for a new <see cref="Chat"/>.
    /// </summary>
    /// <returns>A string like 'New chat - 2023-07-01 12:00'.</returns>
    public static string DefaultName() => $"{ChatContext.Default.Name} - {DateTime.UtcNow.ToString(fmtDateTime)}";

    /// <summary>
    /// Returns this chat as an <see cref="IList{Message}"/>.
    /// </summary>
    /// <returns>An <see cref="IList{Message}"/></returns>
    public IList<Message> AsOpenAIChat()
    {
        return Messages.Select(c => c.AsOpenAIMessage()).ToList();
    }

    /// <summary>
    /// Overridden to return the <see cref="Title"/>.
    /// </summary>
    public override string ToString() => Title;

    #endregion
}