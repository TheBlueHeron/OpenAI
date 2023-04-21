using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueHeron.OpenAI.ViewModels;

/// <summary>
/// Base class for all viewmodels.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    /// <summary>
    /// Creates a new <see cref="BaseViewModel"/>.
    /// </summary>
    public BaseViewModel() { }

    /// <summary>
    /// The title of the page, consuming this viewmodel.
    /// </summary>
    [ObservableProperty]
    private string _title = string.Empty;
}