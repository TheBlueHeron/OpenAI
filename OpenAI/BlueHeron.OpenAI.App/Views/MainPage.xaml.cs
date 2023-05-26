namespace BlueHeron.OpenAI.Views;

/// <summary>
/// The main application page.
/// </summary>
public partial class MainPage : TabbedPage
{
    #region Construction

    /// <summary>
    /// Initializes the main page.
    /// </summary>
    /// <param name="viewModel">The <see cref="OpenAIViewModel"/> to use</param>
    public MainPage(OpenAIViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    #endregion

    #region Events

    /// <summary>
    /// Sets focus to the Question <see cref="Entry"/>.
    /// </summary>
    private void ChatPageLoaded(object sender, EventArgs e)
    {
        txtQuestion.Focus();
    }

    /// <summary>
    /// Executes the <see cref="OpenAIViewModel.AnswerQuestionCommand"/>.
    /// </summary>
    private void QuestionCompleted(object sender, EventArgs e)
    {
        ((OpenAIViewModel)BindingContext).AnswerQuestionCommand.Execute(null);
    }

    /// <summary>
    /// ...
    /// </summary>
    private void ToDoPageLoaded(object sender, EventArgs e)
    {
        // do stuff
    }

    #endregion
}