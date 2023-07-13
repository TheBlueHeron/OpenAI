using System.Text.Json;
using BlueHeron.OpenAI.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueHeron.OpenAI;

/// <summary>
/// Container for application settings and preferences.
/// </summary>
public partial class AppOptions : ObservableObject, IJsonSerializable
{
    #region Objects and variables



    #endregion

    #region Properties

    /// <summary>
    /// The language code of the language to use in the UI and in speech.
    /// </summary>
    [ObservableProperty]
    private string _lcid = "en-US";

    /// <summary>
    /// The requested <see cref="AppTheme"/> to use in the UI.
    /// </summary>
    [ObservableProperty]
    private AppTheme _theme = AppTheme.Unspecified;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="AppOptions"/> object.
    /// </summary>
    public AppOptions() { }

    #endregion

    #region Public methods and functions

    public void SetTheme(AppTheme theme)
    {
        Application.Current.UserAppTheme = theme;
    }

    /// <summary>
    /// Serializes this object into a Json representation.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use</param>
    /// <returns>A Json string representation of this object</returns>
    public string ToJson(JsonSerializerOptions options = null) => JsonSerializer.Serialize(this, options);

    #endregion

}