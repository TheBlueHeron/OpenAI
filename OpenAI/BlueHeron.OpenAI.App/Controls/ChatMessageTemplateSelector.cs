using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// <see cref="DataTemplateSelector"/> for <see cref="ChatMessage"/> objects.
/// </summary>
public class ChatMessageTemplateSelector : DataTemplateSelector
{
    #region Properties

    /// <summary>
    /// <see cref="DataTemplate"/> for a <see cref="ChatMessage"/> of type <see cref="MessageType.Answer"/>.
    /// </summary>
    public required DataTemplate AnswerTemplate { get; set; }

    /// <summary>
    /// <see cref="DataTemplate"/> for a <see cref="ChatMessage"/> of type <see cref="MessageType.Question"/>.
    /// </summary>
    public required DataTemplate QuestionTemplate { get; set; }

    /// <summary>
    /// <see cref="DataTemplate"/> for a <see cref="ChatMessage"/> of type <see cref="MessageType.System"/>.
    /// </summary>
    public required DataTemplate SystemTemplate { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Returns the appropriate <see cref="DataTemplate"/> for the given <see cref="ChatMessage"/>.
    /// </summary>
    /// <param name="item">The <see cref="ChatMessage"/></param>
    /// <param name="container">The <see cref="BindableLayout"/> displaying the template</param>
    /// <returns>The appropriate <see cref="DataTemplate"/> for the current <see cref="ChatMessage.MessageType"/></returns>
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return ((ChatMessage)item).MessageType switch
        {
            MessageType.Answer => AnswerTemplate,
            MessageType.Question => QuestionTemplate,
            _ => SystemTemplate,
        };
    }

    #endregion
}