using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// An <see cref="ObservableCollection{ChatMessage}"/> that disallows adding <see cref="ChatMessage"/>s directly.
/// </summary>
public class ChatMessageCollection : ObservableCollection<ChatMessage>
{
    /// <summary>
    /// Shadowed in order to disallow adding messages directly.
    /// Use <see cref="Chat.AddQuestion(string, bool)"/> instead.
    /// </summary>
    /// <param name="chatMessage">The <see cref="ChatMessage"/></param>
    /// <exception cref="InvalidOperationException">Don't add items directly. Use <see cref="Chat.AddQuestion(string, bool)"/> instead.</exception>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Shadowed to disallow adding items directly")]
    public new void Add(ChatMessage chatMessage) => throw new InvalidOperationException("Don't add items directly. Use Chat.AddQuestion(...) instead.");

    /// <summary>
    /// Adds a <see cref="ChatMessage"/> to the collection.
    /// </summary>
    /// <param name="chatMessage">The <see cref="ChatMessage"/></param>
    internal void AddMessage(ChatMessage chatMessage)
    {
        base.Add(chatMessage);
    }
}