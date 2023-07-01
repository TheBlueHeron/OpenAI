using System.ComponentModel;

namespace BlueHeron.OpenAI.Views;

/// <summary>
/// The main application page.
/// </summary>
public partial class MainPage : TabbedPage
{
    #region Objects and variables

    private readonly OpenAIViewModel mViewModel;

    #endregion

    #region Construction

    /// <summary>
    /// Initializes the main page.
    /// </summary>
    /// <param name="viewModel">The <see cref="OpenAIViewModel"/> to use</param>
    public MainPage(OpenAIViewModel viewModel)
    {
        InitializeComponent();
        mViewModel = viewModel;
        BindingContext = mViewModel;
        mViewModel.PropertyChanged += OnAlertChanged;
    }

    /// <summary>
    /// Displays an alert message if needed.
    /// </summary>
    private void OnAlertChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is OpenAIViewModel vm && e.PropertyName == nameof(OpenAIViewModel.Alert) && !string.IsNullOrEmpty(vm.Alert))
        {
            DisplayAlert("Warning", vm.Alert, "Dismiss", FlowDirection.MatchParent);
        }
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
        mViewModel.AnswerQuestionCommand.Execute(null);
    }

    /// <summary>
    /// ...
    /// </summary>
    private void ToDoPageLoaded(object sender, EventArgs e)
    {
        // do stuff
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    protected async override void OnDisappearing()
    {
        _ = await mViewModel.Quit(); // despite careful disposing an error is generated on close, which is not captured by AppDomain.Current.UnhandledException
        base.OnDisappearing();
    }

    #endregion
}