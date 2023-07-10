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

    /// <summary>
    /// The default <see cref="ChatContext"/>.
    /// </summary>
    private static ChatContext mDefault;

    private const string _DEFAULTNAME = "Assistant";

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="IAnswerHandler"/> to use.
    /// </summary>
    [ObservableProperty]
    private IAnswerHandler _answerHandler;

    /// <summary>
    /// The content of the first <see cref="ChatMessageType.System"/> message in the <see cref="Chat"/>.
    /// This provides the context when answering subsequent questions.
    /// </summary>
    [ObservableProperty]
    private string _context;

    /// <summary>
    /// Returns a default <see cref="ChatContext"/>.
    /// </summary>
    public static ChatContext Default
    {
        get
        {
            if (mDefault == null)
            {
                mDefault = new ChatContext();
                mDefault.AnswerHandler = IAnswerHandler.Default(mDefault);
                mDefault.QuestionHandler = IQuestionHandler.Default(mDefault);
            }
            return mDefault;
        }
    }

    /// <summary>
    /// The name of this context.
    /// </summary>
    [ObservableProperty]
    private string _name;

    /// <summary>
    /// Dictionary of custom parameters needed by this context.
    /// </summary>
    [ObservableProperty]
    private Dictionary<string, string> _parameters = new();

    /// <summary>
    /// The <see cref="IQuestionHandler"/> to use.
    /// </summary>
    [ObservableProperty]
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