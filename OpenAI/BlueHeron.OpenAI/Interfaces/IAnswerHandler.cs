using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Interfaces;

/// <summary>
/// Interface definition for objects that transform OpenAI API answers into results that are tailored to a specific <see cref="ChatContext"/>.
/// </summary>
public interface IAnswerHandler
{
    #region Properties

    /// <summary>
    /// The <see cref="ChatContext"/> to use.
    /// </summary>
    public ChatContext Context { get; set; }

    /// <summary>
    /// Returns the <see cref="DefaultAnswerHandler"/> for the given <see cref="ChatContext"/>.
    /// </summary>
    public static IAnswerHandler Default(ChatContext context) => new DefaultAnswerHandler(context);

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Transforms the given answer sequence into a sequence that is tailored to the configured <see cref="Context"/>.
    /// </summary>
    /// <param name="answer">The answer returned from the OpenAI API</param>
    /// <param name="actualResponse">The OpenAI API output as a string. Leave empty to use the transformed content as actual content.</param>
    /// <returns>The sequence that will be returned to the user</returns>
    public IAsyncEnumerable<string> Transform(IAsyncEnumerable<string> answer, out string actualResponse);

    #endregion
}