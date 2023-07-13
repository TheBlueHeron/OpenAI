
namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// Modal popup that allows for editing app options.
/// </summary>
public partial class OptionsView : PopupView
{
    #region Objects and variables

    /// <summary>
    /// Bindable property for <see cref="Options"/> property.
    /// </summary>
    public static readonly BindableProperty OptionsProperty = BindableProperty.Create(nameof(Options), typeof(object), typeof(OptionsView), null);

    /// <summary>
    /// Bindable property for <see cref="PopupHeightRequest"/> property.
    /// </summary>
    public static readonly BindableProperty PopupHeightRequestProperty = BindableProperty.Create(nameof(PopupHeightRequest), typeof(double), typeof(ItemSelectionView), 400.0);

    /// <summary>
    /// Bindable property for <see cref="PopupTitle"/> property.
    /// </summary>
    public static readonly BindableProperty PopupTitleProperty = BindableProperty.Create(nameof(PopupTitle), typeof(string), typeof(ItemSelectionView), "Edit options");

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="Options"/> to edit.
    /// </summary>
    public object Options
    {
        get => (object)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    /// <summary>
    /// Requested height of the popup.
    /// </summary>
    public double PopupHeightRequest
    {
        get => (double)GetValue(PopupHeightRequestProperty);
        set => SetValue(PopupHeightRequestProperty, value);
    }

    /// <summary>
    /// The title of the popup.
    /// </summary>
    public string PopupTitle
    {
        get => (string)GetValue(PopupTitleProperty);
        set => SetValue(PopupTitleProperty, value);
    }

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="OptionsView"/>.
    /// </summary>
    public OptionsView()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    /// <summary>
    /// Closes the popup, returning null as a result.
    /// </summary>
    private async void EditCanceled(object sender, EventArgs e)
    {
        await Popup.Close(null);
    }

    /// <summary>
    /// Closes the popup, returning the modified options as a result.
    /// </summary>
    private async void EditCompleted(object sender, EventArgs e)
    {
        await Popup.Close(Options);        
    }

    #endregion
}