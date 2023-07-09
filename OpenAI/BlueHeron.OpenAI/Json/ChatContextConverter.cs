﻿using System.Text.Json;
using System.Text.Json.Serialization;
using BlueHeron.OpenAI.Models;

namespace BlueHeron.OpenAI.Json;

/// <summary>
/// Handles json (de-)serialization of <see cref="ChatContext"/> objects.
/// </summary>
public class ChatContextConverter : JsonConverter<ChatContext>
{
    #region Objects and variables

    private const string _errDeserialize = "Error deserializing json into a ChatContext object.";

    private const string PROPANSWERHANDLER = "AnswerHandler";
    private const string PROPCONTEXT = "Context";
    private const string PROPNAME = "Name";
    private const string PROPQUESTIONHANDLER = "QuestionHandler";

    #endregion

    /// <summary>
    /// Deserializes the given json string into a <see cref="ChatContext"/> instance.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to use</param>
    /// <param name="typeToConvert">The <see cref="System.Type"/> to convert the json into</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use</param>
    /// <returns>A <see cref="ChatContext"/> instance</returns>
    public override ChatContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = string.Empty;
        var context = string.Empty;
        var answerHandlerTypeName = string.Empty;
        var questionHandlerTypeName = string.Empty;

        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                var value = reader.GetString();

                switch (propertyName)
                {
                    case PROPNAME:
                        name = value;       
                        break;
                    case PROPCONTEXT:
                        context = value;
                        break;
                    case PROPANSWERHANDLER:
                        answerHandlerTypeName = value;
                        break;
                    case PROPQUESTIONHANDLER:
                        questionHandlerTypeName = value;
                        break;
                }
            }
            if (reader.TokenType == JsonTokenType.EndObject) //  finished reading fields
            {
                break;
            }
        }
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(context) || string.IsNullOrEmpty(answerHandlerTypeName) || string.IsNullOrEmpty(questionHandlerTypeName))
        {
            throw new JsonException(_errDeserialize);
        }
        return new ChatContext(name, context) { AnswerHandler = (Interfaces.IAnswerHandler)Activator.CreateInstance(Type.GetType(answerHandlerTypeName)), QuestionHandler = (Interfaces.IQuestionHandler)Activator.CreateInstance(Type.GetType(questionHandlerTypeName)) };
    }

    /// <summary>
    /// Serializes the given <see cref="ChatContext"/> into a json string representation.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to use</param>
    /// <param name="value">The <see cref="ChatContext"/> to serialize</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use</param>
    public override void Write(Utf8JsonWriter writer, ChatContext value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(PROPNAME, value.Name);
        writer.WriteString(PROPCONTEXT, value.Context);
        writer.WriteString(PROPANSWERHANDLER, value.AnswerHandler.GetType().AssemblyQualifiedName);
        writer.WriteString(PROPQUESTIONHANDLER, value.QuestionHandler.GetType().AssemblyQualifiedName);
        writer.WriteEndObject();
    }
}