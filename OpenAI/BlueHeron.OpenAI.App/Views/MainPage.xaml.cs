using System.ComponentModel;
using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Views;

/// <summary>
/// The main application page.
/// </summary>
public partial class MainPage : ContentPage
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
        stack.SizeChanged += OnStackHeightChanged;
    }

    /// <summary>
    /// Ensures that the last message is fully visible when added or modified.
    /// </summary>
    private async void OnStackHeightChanged(object? sender, EventArgs e)
    {
        await svw.ScrollToAsync(stack, ScrollToPosition.End, false);
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
    /// Activates the selected <see cref="Chat"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Picker"/></param>
    /// <param name="e"><see cref="EventArgs"/>, unused</param>
    private void ChatSelected(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            mViewModel.ActivateChat((Chat)((Picker)sender).SelectedItem);
        }
    }

    /// <summary>
    /// Sets focus to the Question <see cref="Entry"/>.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Application.Current?.Dispatcher.DispatchDelayed(new TimeSpan(0, 0, 0, 0, 500), () =>
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                GetParentWindow().Width += 0.5; // found no better way to fix the ScrollView inside Grid row of type * layout problem
            }
            txtQuestion.Focus();
        });
    }

    /// <summary>
    /// Executes the <see cref="OpenAIViewModel.AnswerQuestionCommand"/>.
    /// </summary>
    private void QuestionCompleted(object sender, EventArgs e)
    {
        mViewModel.AnswerQuestionCommand.Execute(null);
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    protected async override void OnDisappearing()
    {
        _ = await mViewModel.Quit(); // despite careful disposing an error is generated on close, which is not captured by AppDomain.Current.UnhandledException
        base.OnDisappearing();
    }

    private void SettingsClicked(object sender, EventArgs e)
    {
        //
    }

    #endregion
}