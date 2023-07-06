using System.Collections.ObjectModel;
using System.Text.Json;

namespace BlueHeron.OpenAI.Models;

/// <summary>
/// An <see cref="ObservableCollection{Chat}"/> that is easily (de-)serializable from and to json.
/// </summary>
public class ChatCollection : ObservableCollection<Chat>
{
    /// <summary>
    /// Deserializes the given json into a <see cref="ChatCollection"/>.
    /// </summary>
    /// <param name="json">The json string to convert</param>
    /// <returns>A <see cref="ChatCollection"/></returns>
    public static ChatCollection FromJson(string json, JsonSerializerOptions options = null) => JsonSerializer.Deserialize<ChatCollection>(json, options);

    /// <summary>
    /// Serializes this collection to json.
    /// </summary>
    /// <returns>A json string</returns>
    public string ToJson(JsonSerializerOptions options = null) => JsonSerializer.Serialize(this, options);
}