using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI;

/// <summary>
/// Enumeration of possible types of a <see cref="ChatMessage"/>.
/// </summary>
public enum ChatMessageType
{
    /// <summary>
    /// The message is a question.
    /// </summary>
    Question,
    /// <summary>
    /// The message is an answer.
    /// </summary>
    Answer,
    /// <summary>
    /// The message is a system message.
    /// </summary>
    System,
    /// <summary>
    /// The message is a function call.
    /// </summary>
    Function
}