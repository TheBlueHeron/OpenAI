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
    /// <param name="viewModel">The <see cref="ServiceConnectorViewModel"/> to use</param>
    public MainPage(ServiceConnectorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    #endregion

    #region Events

    /// <summary>
    /// Sets focus to the Question <see cref="Entry"/>.
    /// </summary>
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        txtQuestion.Focus();
    }

    /// <summary>
    /// Executes the <see cref="ServiceConnectorViewModel.AnswerQuestionCommand"/>.
    /// </summary>
    private void QuestionCompleted(object sender, EventArgs e)
    {
        ((ServiceConnectorViewModel)BindingContext).AnswerQuestionCommand.Execute(null);
    }

    #endregion
}