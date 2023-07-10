using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Interfaces;

/// <summary>
/// Interface definition for objects that transform user questions into prompts that are tailored to a specific <see cref="ChatContext"/>.
/// </summary>
public interface IQuestionHandler
{
    #region Properties

    /// <summary>
    /// The <see cref="ChatContext"/> to use.
    /// </summary>
    public ChatContext Context { get; set; }

    /// <summary>
    /// Returns the <see cref="DefaultQuestionHandler"/> for the given <see cref="ChatContext"/>.
    /// </summary>
    public static IQuestionHandler Default(ChatContext context) => new DefaultQuestionHandler(context);

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Transforms the given user input into a prompt that is tailored to the configured <see cref="Context"/>.
    /// </summary>
    /// <param name="userInput">The user input</param>
    /// <returns>The prompt that will be posted to the OpenAI API</returns>
    public string Transform(string userInput);

    #endregion
}