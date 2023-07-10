using BlueHeron.OpenAI.Interfaces;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// A <see cref="List{ChatContext}"/> that always contains a <see cref="ChatContext.Default"/>.
/// </summary>
public class ChatContextCollection : List<ChatContext>
{
    #region Construction

    /// <summary>
    /// Creates a new <see cref="ChatContextCollection"/>.
    /// </summary>
    public ChatContextCollection()
    {
        Add(ChatContext.Default);

        // TODO: retrieve available contexts from server
        var ctx = new ChatContext("BIS", "Insert instructions for BIS data here...");
        ctx.QuestionHandler = IQuestionHandler.Default(ctx);
        ctx.AnswerHandler = IAnswerHandler.Default(ctx);
        Add(ctx);
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Shadowed to prevent the <see cref="ChatContext.Default"/> from being removed.
    /// </summary>
    /// <param name="context">The <see cref="ChatContext"/> to remove</param>
    public new bool Remove(ChatContext context)
    {
        if (object.ReferenceEquals(context, ChatContext.Default))
        {
            return false;
        }
        return base.Remove(context);
    }

    #endregion
}