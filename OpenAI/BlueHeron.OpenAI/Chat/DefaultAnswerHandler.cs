using BlueHeron.OpenAI.Interfaces;

namespace BlueHeron.OpenAI;

/// <summary>
/// <see cref="IQuestionHandler" /> that returns the user input without applying any transformation.
/// </summary>
public class DefaultAnswerHandler : IAnswerHandler
{
    /// <summary>
    /// No transformation is applied.
    /// </summary>
    /// <param name="answer">The answer returned from the OpenAI API</param>
    /// <returns>An <see cref="IAsyncEnumerable{String}"/></returns>
    public IAsyncEnumerable<string> Transform(IAsyncEnumerable<string> answer) => answer;
}