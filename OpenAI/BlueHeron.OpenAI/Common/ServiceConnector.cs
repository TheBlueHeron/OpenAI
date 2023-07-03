using System.Diagnostics;
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
    public ServiceConnector() // TODO: REMOVE VALUES!!!
    {
        mClient = new OpenAIClient(new OpenAIAuthentication(
            "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", // Environment.GetEnvironmentVariable(KEY_API),
            "org-Ssz4keu7TIM75Edx4JnRQdo5" // Environment.GetEnvironmentVariable(KEY_ORG)
        ));
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

        await foreach (var result in mClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
        {
            foreach (var choice in result.Choices.Where(choice => !string.IsNullOrWhiteSpace(choice.Delta?.Content)))
            {
                if (choice.FinishReason == null)
                {
                    yield return choice.Delta.Content;
                }
                else
                {
                    Debug.WriteLine(choice.FinishReason);
                    yield return _UNKNOWN;
                }
            }
        }
    }

    #endregion
}