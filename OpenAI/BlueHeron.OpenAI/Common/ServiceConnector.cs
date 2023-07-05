using System.Reflection;
using BlueHeron.OpenAI.Models;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace BlueHeron.OpenAI;

/// <summary>
/// Handles the connection to an <see cref="OpenAIClient"/>.
/// The <see cref="ServiceConnector"/> expects two environment variables to be present on the local machine:
/// 'OPENAI_KEY': must hold a valid OpenAI API key. See: https://platform.openai.com/account/api-keys
/// 'OPENAI_ORG': must hold a registered organisation id. See: https://platform.openai.com/account/org-settings
/// </summary>
public class ServiceConnector
{
    #region Objects and variables

    private const string _UNKNOWN = "?";

    public const string KEY_API = "OPENAI_KEY";
    public const string KEY_ORG = "OPENAI_ORG";

    private readonly OpenAIClient mClient;

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="OpenAIClient"/> to use for connecting to OpenAI.
    /// </summary>
    public OpenAIClient Client => mClient;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="ServiceConnector"/>.
    /// </summary>
    public ServiceConnector()
    {
        //mClient = new OpenAIClient(new OpenAIAuthentication(
        //    "sk-aaaaabbbbbcccccddddd", // Environment.GetEnvironmentVariable(KEY_API),
        //    "org-eeeeefffffggggghhhhh" // Environment.GetEnvironmentVariable(KEY_ORG)
        //));
        mClient = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))); // assumes the presence of a file named '.openai' in the output directory. See: https://github.com/RageAgainstThePixel/OpenAI-DotNet#load-key-from-configuration-file
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