using System.ComponentModel;
using BlueHeron.OpenAI.Controls;
using BlueHeron.OpenAI.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace BlueHeron.OpenAI.Views;

/// <summary>
/// The main application page.
/// </summary>
public partial class MainPage : ContentPage
{
    #region Objects and variables

    private const string _COPIED = "Message copied!";

    private readonly OpenAIViewModel mViewModel;

    private readonly SnackbarOptions successOptions = new()
    {
        BackgroundColor = Colors.Green,
        TextColor = Colors.White,
        CornerRadius = new CornerRadius(8),
        Font = Microsoft.Maui.Font.SystemFontOfSize(14),
        CharacterSpacing = 0.5
    };

    #endregion

    #region Construction

    /// <summary>
    /// Initializes the main page.
    /// </summary>
    /// <param name="viewModel">The <see cref="OpenAIViewModel"/> to use</param>
    public MainPage(OpenAIViewModel viewModel)
    {
        mViewModel = viewModel;
        BindingContext = mViewModel;
        InitializeComponent(); // above lines must come first as the ViewModel is needed for data bindings in this line
        mViewModel.PropertyChanged += OnAlertChanged;
        stack.SizeChanged += OnStackHeightChanged;
    }

    #endregion

    #region Events

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

    /// <summary>
    /// Sets focus to the Question <see cref="Entry"/>.
    /// </summary>
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await mViewModel.GetAvailableContextsAsync();
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
    /// Activates the selected <see cref="Chat"/>.
    /// </summary>
    private void OnChatSelected(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            mViewModel.ActivateChat((Chat)((Picker)sender).SelectedItem);
        }
    }

    /// <summary>
    /// Copies the selected <see cref="ChatMessage"/>'s <see cref="ChatMessage.DisplayedContent"/> to the clipboard and notifies the user.
    /// </summary>
    private async void OnCopyMessageClicked(object sender, EventArgs e)
    {
        var mnuItem = (MenuFlyoutItem)sender;

        if (mnuItem.BindingContext is ChatMessage msg)
        {
            await Clipboard.Default.SetTextAsync(msg.DisplayedContent);
            await ((VisualElement)((MenuFlyout)mnuItem.Parent).Parent).DisplaySnackbar(_COPIED, duration:TimeSpan.FromSeconds(3), visualOptions: successOptions);
        }
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    protected async override void OnDisappearing()
    {
        _ = await mViewModel.Quit(); // despite careful disposing an error is generated on close, which is not captured by AppDomain.Current.UnhandledException

        base.OnDisappearing();
    }

    /// <summary>
    /// Creates a new <see cref="Chat"/>, using the selected <see cref="ChatContext"/>.
    /// </summary>
    /// <param name="sender">The selected <see cref="ChatContext"/></param>
    private void OnNewChatSelected(object sender, PopupCompletedEventArgs e)
    {
        var rst = e.Result;
        if (rst != null)
        {
            mViewModel.AddChatCommand.Execute((ChatContext)rst);
        }
    }

    /// <summary>
    /// Executes the <see cref="OpenAIViewModel.AnswerQuestionCommand"/>.
    /// </summary>
    private void OnQuestionCompleted(object sender, EventArgs e)
    {
        mViewModel.AnswerQuestionCommand.Execute(false);
    }

    /// <summary>
    /// Handles settings changes.
    /// </summary>
    private void OnSettingsChanged(object sender, PopupCompletedEventArgs e)
    {
        var rst = e.Result;
        if (rst != null)
        {
            // save settings
        }
    }

    /// <summary>
    /// Ensures that the last message is fully visible when added or modified.
    /// </summary>
    private async void OnStackHeightChanged(object? sender, EventArgs e)
    {
        await svw.ScrollToAsync(stack, ScrollToPosition.End, false);
    }

    #endregion

}