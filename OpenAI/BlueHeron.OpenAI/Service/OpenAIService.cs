using System.Reflection;
using BlueHeron.OpenAI.Models;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace BlueHeron.OpenAI;

/// <summary>
/// Handles the connection to an <see cref="OpenAIClient"/>.
/// The <see cref="OpenAIService"/> expects  a json file named '.openai' to be present in the same directory as the application executable.
/// For details on the expected file contents, see: https://github.com/RageAgainstThePixel/OpenAI-DotNet#load-key-from-configuration-file
/// </summary>
public class OpenAIService
{
    #region Objects and variables

    private readonly OpenAIClient mClient;

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="OpenAIClient"/>, used to communicate with the OpenAI API.
    /// </summary>
    public OpenAIClient Client => mClient;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="OpenAIService"/>.
    /// </summary>
    public OpenAIService()
    {
        mClient = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Posts the given question to the chat completion API using the <see cref="Model.GPT3_5_Turbo"/> model, and returns the answer as an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <param name="chat">The updated <see cref="Chat"/></param>
    /// <returns>The generated answer</returns>
    public async IAsyncEnumerable<string> Update(Chat chat)
    {
        var messages = chat.AsOpenAIChat();
        var chatRequest = new ChatRequest(messages);

        var iterator = mClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest).GetAsyncEnumerator();
        var more = true;
        var msgError = string.Empty;

        while (more)
        {
            try
            {
                more = await iterator.MoveNextAsync();
            }
            catch (Exception e)
            {
                more = false;
                msgError = e.Message;
            }

            if (more)
            {
                IEnumerable<Choice> choices = null;

                try
                {
                    choices = iterator.Current.Choices.Where(choice => !string.IsNullOrWhiteSpace(choice.Delta?.Content));
                }
                catch (Exception ex)
                {
                    choices = Array.Empty<Choice>();
                    more = false;
                    msgError = ex.Message;
                }
                foreach (var choice in choices)
                {
                    if (choice.FinishReason == null)
                    {
                        yield return choice.Delta.Content;
                    }
                    else
                    {
                        yield return choice.FinishReason; // _UNKNOWN;
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(msgError))
        {
            yield return msgError;
        }
    }

    #endregion
}