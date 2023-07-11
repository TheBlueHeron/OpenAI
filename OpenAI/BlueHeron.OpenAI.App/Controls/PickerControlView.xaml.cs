using MauiPopup;
using MauiPopup.Views;
using System.Collections;

namespace BlueHeron.OpenAI.Controls;

/// <summary>
/// Modal popup that allows for the selection of a databound item.
/// </summary>
public partial class PickerControlView : BasePopupPage
{
    #region Construction

    /// <summary>
    /// Creates a new <see cref="PickerControlView"/>.
    /// </summary>
    /// <param name="itemSource">The items to display</param>
    /// <param name="itemTemplate">The <see cref="DataTemplate"/> to use for displaying the items</param>
    /// <param name="title">The title of the popup</param>
    /// <param name="pickerControlHeight">The requested height of the popup</param>
    public PickerControlView(IEnumerable itemSource, DataTemplate itemTemplate, string title, double pickerControlHeight)
    {
        InitializeComponent();

        lblTitle.Text = title;
        clPickerView.ItemsSource = itemSource;
        clPickerView.ItemTemplate = itemTemplate;
        grv.HeightRequest = pickerControlHeight;
    }

    #endregion

    #region Events

    /// <summary>
    /// Closes the popup, returning the selected item as a result.
    /// </summary>
    /// <param name="sender">The <see cref="CollectionView"/> used to display the items</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/></param>
    private async void PickerView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Any())
        {
            await PopupAction.ClosePopup(e.CurrentSelection[0]);
        }        
    }

    #endregion
}