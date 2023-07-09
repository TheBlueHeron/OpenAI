using System.Text.Json.Serialization;
using BlueHeron.OpenAI.Interfaces;
using BlueHeron.OpenAI.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// Object that provides context for <see cref="Chat"/>s, by injecting context information in to the first <see cref="ChatMessage"/> of the <see cref="Chat"/>.
/// </summary>
[JsonConverter(typeof(ChatContextConverter))]
public partial class ChatContext : ObservableObject
{
    #region Objects and variables

    private const string _DEFAULTNAME = "Assistant";

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="IAnswerHandler"/> to use.
    /// </summary>
    [ObservableProperty]
    [property: JsonIgnore()]
    private IAnswerHandler _answerHandler;

    /// <summary>
    /// Returns a default <see cref="ChatContext"/>.
    /// </summary>
    [JsonIgnore()]
    public static ChatContext Default => new() { AnswerHandler = IAnswerHandler.Default, QuestionHandler = IQuestionHandler.Default };

    /// <summary>
    /// The name of this context.
    /// </summary>
    [ObservableProperty]
    private string _name;

    /// <summary>
    /// The content of the first <see cref="ChatMessageType.System"/> message in the <see cref="Chat"/>.
    /// This provides the context when answering subsequent questions.
    /// </summary>
    [ObservableProperty]
    private string _context;

    /// <summary>
    /// The <see cref="IQuestionHandler"/> to use.
    /// </summary>
    [ObservableProperty]
    [property: JsonIgnore()]
    private IQuestionHandler _questionHandler;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="ChatContext"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ChatContext"/></param>
    /// <param name="context">The content of the first <see cref="ChatMessageType.System"/> message in the <see cref="Chat"/></param>
    public ChatContext(string name = _DEFAULTNAME, string context = ChatMessage.MSG_ASSISTANT)
    {
        Name = name;
        Context = context;
    }

    #endregion
}