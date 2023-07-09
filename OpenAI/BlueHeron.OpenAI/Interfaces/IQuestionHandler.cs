using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Interfaces;

/// <summary>
/// Interface definition for objects that transform user questions into prompts that are tailored to a specific <see cref="ChatContext"/>.
/// </summary>
public interface IQuestionHandler
{
    #region Objects and variables

    /// <summary>
    /// The <see cref="DefaultQuestionHandler"/>.
    /// </summary>
    private static readonly IQuestionHandler mDefault = new DefaultQuestionHandler();

    #endregion

    #region Properties

    /// <summary>
    /// Returns the <see cref="DefaultQuestionHandler"/>.
    /// </summary>
    public static IQuestionHandler Default => mDefault;

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Transforms the given user input into a prompt that is tailored to the current <see cref="ChatContext"/>.
    /// </summary>
    /// <param name="userInput">The user input</param>
    /// <returns>The prompt that will be posted to the OpenAI API</returns>
    public string Transform(string userInput);

    #endregion
}