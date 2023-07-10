using BlueHeron.OpenAI.Models;
using BlueHeron.OpenAI.Interfaces;

namespace BlueHeron.OpenAI;

/// <summary>
/// <see cref="IQuestionHandler" /> that returns the user input without applying any transformation.
/// </summary>
public class DefaultAnswerHandler : IAnswerHandler
{
    #region Properties

    /// <summary>
    /// The <see cref="ChatContext"/> to use.
    /// </summary>
    public ChatContext Context { get; set; }

    #endregion

    #region Construction

    /// <summary>
    /// Needed in deserialization.
    /// </summary>
    public DefaultAnswerHandler() { }

    /// <summary>
    /// Creates a new <see cref="DefaultAnswerHandler"/>, using the given <see cref="ChatContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="ChatContext"/> to use</param>
    public DefaultAnswerHandler(ChatContext context)
    {
        Context = context;
    }

    #endregion

    /// <summary>
    /// No transformation is applied.
    /// </summary>
    /// <param name="answer">The answer returned from the OpenAI API</param>
    /// <param name="actualResponse">The OpenAI API output as a string. Null in this case.</param>
    /// <returns>An <see cref="IAsyncEnumerable{String}"/></returns>
    public IAsyncEnumerable<string> Transform(IAsyncEnumerable<string> answer, out string actualResponse)
    {
        actualResponse = null;
        return answer;
    }
}