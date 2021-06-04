using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Splitnab
{
    /// <summary>
    ///     Temp Dynamic Converter
    ///     by:tchivs@live.cn
    ///     From https://github.com/dotnet/runtime/issues/29690#issuecomment-571969037,
    ///     https://github.com/dotnet/runtime/issues/29690#issuecomment-644772205,
    ///     https://github.com/dotnet/runtime/issues/29690#issuecomment-660301949
    /// </summary>
    public class DynamicJsonConverter : JsonConverter<dynamic>
    {
        public override dynamic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number => reader.TryGetInt64(out var l) ? l : reader.GetDouble(),
                JsonTokenType.String => reader.TryGetDateTime(out var datetime)
                    ? datetime.ToString(CultureInfo.InvariantCulture)
                    : reader.GetString(),
                JsonTokenType.StartObject => ReadObject(JsonDocument.ParseValue(ref reader).RootElement),
                // Use JsonElement as fallback.
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };
        }

        private object ReadObject(JsonElement jsonElement)
        {
            IDictionary<string, object> expandoObject = new ExpandoObject();
            foreach (var obj in jsonElement.EnumerateObject())
            {
                var k = obj.Name;
                var value = ReadValue(obj.Value);
                expandoObject[k] = value;
            }

            return expandoObject;
        }

        private object ReadValue(JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Object => ReadObject(jsonElement),
                JsonValueKind.Array => ReadList(jsonElement),
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.TryGetInt64(out var l) ? l : 0,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Undefined => null,
                JsonValueKind.Null => null,
                _ => throw new ArgumentOutOfRangeException(nameof(jsonElement))
            };
        }

        private object ReadList(JsonElement jsonElement)
        {
            var list = new List<object>();
            jsonElement.EnumerateArray().ToList().ForEach(j => list.Add(ReadValue(j)));
            return list.Count == 0 ? null : list;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
