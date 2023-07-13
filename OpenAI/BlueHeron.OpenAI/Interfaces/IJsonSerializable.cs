using System.Text.Json;

namespace BlueHeron.OpenAI.Interfaces;

/// <summary>
/// Interface definition for objects that are able to (de-)serialize themselves.
/// </summary>
public interface IJsonSerializable
{
    /// <summary>
    /// Deserializes the given json into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="json">The json string to convert</param>
    /// <returns>A <typeparamref name="T"/></returns>
    public static T FromJson<T>(string json, JsonSerializerOptions options = null) => JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Serializes this cobject to json.
    /// </summary>
    /// <returns>A json string</returns>
    public string ToJson(JsonSerializerOptions options = null);
}