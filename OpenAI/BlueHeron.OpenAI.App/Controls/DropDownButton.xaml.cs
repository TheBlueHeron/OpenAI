using System.Collections;
using MauiPopup;

namespace BlueHeron.OpenAI.Controls;

#nullable disable

/// <summary>
/// Button that shows a popup from which a single item can be selected.
/// </summary>
public partial class PopupButton : Button
{
    #region Objects and variables

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="Glyph"/> property.
    /// </summary>
    public static readonly BindableProperty GlyphProperty = BindableProperty.Create(propertyName: nameof(Glyph), returnType: typeof(string), declaringType: typeof(PopupButton), defaultBindingMode: BindingMode.OneWay, defaultValue: "&#xE710;");

    /// <summary>
    /// <see cref="BindableProperty"/> for the <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(propertyName: nameof(ItemSource), returnType: typeof(IEnumerable), declaringType: typeof(PopupButton), defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// Event is fired when the user has selected an item.
    /// </summary>
    public event EventHandler<EventArgs> ItemSelected;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the glyph to display on the button.
    /// </summary>
    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of items that can be selected.
    /// </summary>
    public IEnumerable ItemSource
    {
        get => (IEnumerable)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    /// <summary>
    /// The <see cref="DataTemplate"/> to use for displaying the selectable items.
    /// </summary>
    public DataTemplate ItemTemplate { get; set; }

    /// <summary>
    /// Requested height of the popup.
    /// </summary>
    public double PickerHeightRequest { get; set; } = 200;

    /// <summary>
    /// The title of the popup.
    /// </summary>
    public string PopupTitle { get; set; } = "Select item";

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="PopupButton"/>.
    /// </summary>
    public PopupButton()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    /// <summary>
    /// Displays the popup and fires the <see cref="ItemSelected"/> event after selecting an item.
    /// </summary>
    private async void DropDownButtonClicked(object sender, EventArgs e)
    {
        var response = await PopupAction.DisplayPopup<object>(new PickerControlView(ItemSource, ItemTemplate, PopupTitle, PickerHeightRequest));

        if (response != null)
        {
            ItemSelected?.Invoke(response, e);
        }
    }

    #endregion
}