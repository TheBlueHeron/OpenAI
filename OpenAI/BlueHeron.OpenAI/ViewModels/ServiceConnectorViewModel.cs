using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BlueHeron.OpenAI.ViewModels;

/// <summary>
/// A <see cref="BaseViewModel"/> implementation that exposes bindable properties and <see cref="IRelayCommand"/>s for the <see cref="ServiceConnector"/>.
/// </summary>
public partial class ServiceConnectorViewModel : BaseViewModel
{
    #region Objects and variables

    private const string _NEWLINE = "\r\n\r\n";
    private const string _RESPONSESTART = ">  ";

    private readonly ServiceConnector mConnector;

    #endregion

    #region Properties

    /// <summary>
    /// The latest answer received from the <see cref="ServiceConnector"/>.
    /// </summary>
    [ObservableProperty()]
    private string _answer = string.Empty;

    /// <summary>
    /// The latest question posted to the <see cref="ServiceConnector"/>.
    /// </summary>
    [ObservableProperty()]
    private string _question = string.Empty;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="ServiceConnectorViewModel"/>
    /// </summary>
    public ServiceConnectorViewModel(ServiceConnector connector)
    {
        mConnector = connector;
        Title = "Chat";
    }

    #endregion

    #region Commands

    /// <summary>
    /// Clears the chat and starts a new one.
    /// </summary>
    [RelayCommand]
    private void ClearChat()
    {
        Question = string.Empty;
        Answer = string.Empty;
        mConnector.ClearChat();
    }

    /// <summary>
    /// Clears the question.
    /// </summary>
    [RelayCommand]
    private void ClearQuestion()
    {
        Question = string.Empty;
    }

    /// <summary>
    /// The default 'AnswerQuestion' command that calls <see cref="ServiceConnector.Answer(string)"/> and asynchronously and repeatedly updates the <see cref="Answer"/> property as it is received as a stream of string tokens.
    /// </summary>
    [RelayCommand]
    private async void AnswerQuestion()
    {
        Answer += _RESPONSESTART;
        await foreach (var t in mConnector.Answer(Question))
        {
            _ = await UpdateAnswer(t);
        }
        Answer += _NEWLINE;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Asynchronously updates the <see cref="Answer"/> property.
    /// </summary>
    /// <param name="t">The next token</param>
    /// <returns><see langword="true"/></returns>
    private async Task<bool> UpdateAnswer(string t)
    {
        await Task.Run(() =>
        {
            Answer += t;
        });
        return true;
    }

    #endregion
}