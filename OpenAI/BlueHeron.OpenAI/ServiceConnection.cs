using System.Diagnostics;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace BlueHeron.OpenAI;

/// <summary>
/// Handles the coneection to an <see cref="OpenAIService"/>.
/// </summary>
public class ServiceConnection
{
    #region Objects and variables

    private readonly OpenAIService mService;

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public OpenAIService Service => mService;

    #endregion

    #region Construction

    public ServiceConnection()
    {
        mService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Environment.GetEnvironmentVariable("OPENAI_KEY"),
            Organization = Environment.GetEnvironmentVariable("OPENAI_ORG"),
            DefaultModelId = Models.Davinci
        });

    }

    #endregion

    #region Public methods and functions

    public async IAsyncEnumerable<string> Answer(string question)
    {
        var completionResult = mService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a helpful assistant."),
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
                if (completion.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                Debug.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
            }
        }
    }

    #endregion
}
