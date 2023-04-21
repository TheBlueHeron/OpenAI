using System.Diagnostics;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace BlueHeron.OpenAI;

/// <summary>
/// Handles the connection to an <see cref="OpenAIService"/>.
/// The <see cref="ServiceConnector"/> expects two environment variables to be present on the local machine:
/// 'OPENAI_KEY': must hold a valid OpenAI API key. See: https://platform.openai.com/account/api-keys
/// 'OPENAI_ORG': must hold a registered organisation id. See: https://platform.openai.com/account/org-settings
/// 
/// Default model: 'DaVinci'.
/// </summary>
public class ServiceConnector
{
    #region Objects and variables

    private const string _ERRUNKNOWN = "Unknown Error";
    private const string _UNKNOWN = "?";

    public const string KEY_API = "OPENAI_KEY";
    public const string KEY_ORG = "OPENAI_ORG";
    public const string MSG_ASSISTANT = "You are a helpful assistant.";

    private readonly OpenAIService mService;

    #endregion

    #region Properties

    /// <summary>
    /// The Betalgo <see cref="OpenAIService"/> to use for connecting to OpenAI.
    /// </summary>
    public OpenAIService Service => mService;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="ServiceConnector"/>.
    /// </summary>
    public ServiceConnector()
    {
        mService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Environment.GetEnvironmentVariable(KEY_API),
            Organization = Environment.GetEnvironmentVariable(KEY_ORG),
            DefaultModelId = Models.Davinci
        });
    }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Posts the given question to the chat completion API using the <see cref="Models.ChatGpt3_5Turbo"/> model, and returns the answer as an <see cref="IAsyncEnumerable{string}"/>.
    /// </summary>
    /// <param name="question">The question to ask</param>
    /// <returns>The generated answer</returns>
    public async IAsyncEnumerable<string> Answer(string question)
    {
        var completionResult = mService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(MSG_ASSISTANT),
                ChatMessage.FromUser(question)
            },
            Model = Models.ChatGpt3_5Turbo
        });

        await foreach (var completion in completionResult)
        {
            if (completion.Successful)
            {
                yield return completion.Choices.FirstOrDefault()?.Message.Content;
            }
            else
            {
                Debug.WriteLine(completion.Error == null ? _ERRUNKNOWN : $" *** {completion.Error.Code}: {completion.Error.Message} *** ");
                yield return _UNKNOWN;
            }
        }
    }

    #endregion
}
