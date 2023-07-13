using System.Collections;

namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// Modal popup that allows for the selection of a databound item.
/// </summary>
public partial class ItemSelectionView : PopupView
{
    #region Objects and variables

    /// <summary>
    /// Bindable property for <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ItemSelectionView), null);

    /// <summary>
    /// Bindable property for <see cref="ItemTemplate"/> property.
    /// </summary>
    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemSelectionView), null);

    /// <summary>
    /// Bindable property for <see cref="PopupHeightRequest"/> property.
    /// </summary>
    public static readonly BindableProperty PopupHeightRequestProperty = BindableProperty.Create(nameof(PopupHeightRequest), typeof(double), typeof(ItemSelectionView), 200.0);

    /// <summary>
    /// Bindable property for <see cref="PopupTitle"/> property.
    /// </summary>
    public static readonly BindableProperty PopupTitleProperty = BindableProperty.Create(nameof(PopupTitle), typeof(string), typeof(ItemSelectionView), "Select item");

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the collection of items that can be selected.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// The <see cref="DataTemplate"/> to use for displaying the selectable items.
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
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
    /// Creates a new <see cref="ItemSelectionView"/>.
    /// </summary>
    public ItemSelectionView()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    /// <summary>
    /// Closes the popup, returning null as a result.
    /// </summary>
    private async void SelectionCanceled(object sender, EventArgs e)
    {
        await Popup.Close(null);
    }

    /// <summary>
    /// Closes the popup, returning the selected item as a result.
    /// </summary>
    private async void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Any())
        {
            await Popup.Close(e.CurrentSelection[0]);
        }        
    }

    #endregion
}