using BlueHeron.OpenAI.Interfaces;

namespace BlueHeron.OpenAI;

/// <summary>
/// <see cref="IQuestionHandler" /> that returns the user input without applying any transformation.
/// </summary>
public class DefaultQuestionHandler : IQuestionHandler
{
    /// <summary>
    /// No transformation is applied.
    /// </summary>
    /// <param name="userInput">The user input</param>
    /// <returns>The <paramref name="userInput"/></returns>
    public string Transform(string userInput) => userInput;
}