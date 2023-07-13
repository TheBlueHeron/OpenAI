
namespace BlueHeron.OpenAI.Controls;

#nullable disable

/// <summary>
/// Object that handles <see cref="PopupView"/> display using the navigation stack.
/// </summary>
public static class Popup
{
    /// <summary>
    /// Pushes the given <see cref="PopupView"/> onto the modal navigation stack.
    /// </summary>
    /// <typeparam name="T">The type of the object displayed</typeparam>
    /// <param name="popup">The <see cref="PopupView"/> to display</param>
    /// <returns>A <see cref="Task{T}"/></returns>
    public static async Task<T> Display<T>(PopupView popup)
    {
        try
        {
            if (Application.Current?.MainPage != null)
            {
                popup.ReturnResultTask = new();
                await Application.Current.MainPage.Navigation.PushModalAsync(popup);
            }
            return (T)await popup.ReturnResultTask.Task;
        }
        catch { return default; } // canceled
    }

    /// <summary>
    /// Closes the last <see cref="PopupView"/> on the modal stack
    /// </summary>
    /// <param name="returnValue">The result of the popup</param>
    /// <returns>A <see cref="Task"/></returns>
    public static async Task Close(object returnValue = null)
    {
        var stack = Application.Current.MainPage.Navigation.ModalStack.Count;

        if (stack > 0)
        {
            try
            {
                var currentPopup = Application.Current.MainPage.Navigation.ModalStack[stack - 1] as PopupView;

                if (returnValue == null)
                {
                    currentPopup?.ReturnResultTask?.SetCanceled();
                }
                else
                {
                    currentPopup?.ReturnResultTask?.SetResult(returnValue);
                }
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch { }
        }
    }
}