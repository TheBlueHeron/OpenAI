using BlueHeron.OpenAI.Models;
using BlueHeron.OpenAI.Interfaces;

namespace BlueHeron.OpenAI;

/// <summary>
/// <see cref="IQuestionHandler" /> that returns the user input without applying any transformation.
/// </summary>
public class DefaultQuestionHandler : IQuestionHandler
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
    public DefaultQuestionHandler() { }

    /// <summary>
    /// Creates a new <see cref="DefaultQuestionHandler"/>, using the given <see cref="ChatContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="ChatContext"/> to use</param>
    public DefaultQuestionHandler(ChatContext context) { Context = context; }

    #endregion

    /// <summary>
    /// No transformation is applied.
    /// </summary>
    /// <param name="userInput">The user input</param>
    /// <returns>The <paramref name="userInput"/></returns>
    public string Transform(string userInput) => userInput;
}