using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Interfaces;

/// <summary>
/// Interface definition for objects that transform OpenAI API answers into results that are tailored to a specific <see cref="ChatContext"/>.
/// </summary>
public interface IAnswerHandler
{
    #region Objects and variables

    /// <summary>
    /// The <see cref="DefaultAnswerHandler"/>.
    /// </summary>
    private static readonly IAnswerHandler mDefault = new DefaultAnswerHandler();

    #endregion

    #region Properties

    /// <summary>
    /// Returns the <see cref="DefaultAnswerHandler"/>.
    /// </summary>
    public static IAnswerHandler Default => mDefault;

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Transforms the given answer sequence into a sequence that is tailored to the current <see cref="ChatContext"/>.
    /// </summary>
    /// <param name="answer">The answer returned from the OpenAI API</param>
    /// <returns>The sequence that will be returned to the user</returns>
    public IAsyncEnumerable<string> Transform(IAsyncEnumerable<string> answer);

    #endregion
}