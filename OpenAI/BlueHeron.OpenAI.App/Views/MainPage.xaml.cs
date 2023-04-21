namespace BlueHeron.OpenAI.Views;

public partial class MainPage : ContentPage
{
    public MainPage(ServiceConnectorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
