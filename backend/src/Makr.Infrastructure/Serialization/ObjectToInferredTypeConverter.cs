using System.Text.Json;
using System.Text.Json.Serialization;

namespace Makr.Infrastructure.Serialization
{
    public class ObjectToInferredTypeConverter : JsonConverter<object?>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Null => null,
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => ReadNumber(ref reader),
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone() // arrays/objects — fallback
            };
        }

        private static object ReadNumber(ref Utf8JsonReader reader)
        {
            if (reader.TryGetInt32(out int i)) return i;
            if (reader.TryGetInt64(out long l)) return l;
            if (reader.TryGetDecimal(out decimal d)) return d;
            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object));
    }
}
