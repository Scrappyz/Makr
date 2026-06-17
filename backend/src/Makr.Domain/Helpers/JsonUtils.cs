using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Makr.Domain.Helpers
{
    public class JsonUtils
    {
        public static T? Convert<T>(object? value)
        {
            if (value is null)
                return default;

            // Step 1 — unwrap JsonElement if present
            if (value is JsonElement element)
                return ConvertJsonElement<T>(element);

            // Step 2 — already the exact type, no conversion needed
            if (value is T directMatch)
                return directMatch;

            // Step 3 — general conversion fallback (e.g., int → decimal)
            return ConvertGeneral<T>(value);
        }

        public static Type? GetType(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => typeof(string),
                JsonValueKind.True => typeof(bool),
                JsonValueKind.False => typeof(bool),
                JsonValueKind.Null => null,
                JsonValueKind.Number => GetNumericType(element),
                JsonValueKind.Array => typeof(List<object>),
                JsonValueKind.Object => typeof(Dictionary<string, object>),
                _ => null
            };
        }

        // For serialization
        public static object? UnwrapJsonElement(object? value)
        {
            if (value is JsonElement element)
            {
                return element.ValueKind switch
                {
                    JsonValueKind.String => element.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Number => ExtractNumber(element),
                    JsonValueKind.Null => null,
                    _ => value
                };
            }

            return value;
        }

        private static Type GetNumericType(JsonElement element)
        {
            // Try the smallest/most precise type first, widen as needed
            if (element.TryGetInt32(out _)) return typeof(int);
            if (element.TryGetInt64(out _)) return typeof(long);
            if (element.TryGetDecimal(out _)) return typeof(decimal);

            return typeof(double); // fallback — handles scientific notation, NaN-like edge cases
        }

        private static T? ConvertJsonElement<T>(JsonElement element)
        {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            object? raw = element.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.String => element.GetString(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number => ExtractNumber(element, targetType),
                _ => null // Array/Object — not handled generically
            };

            if (raw is null)
                return default;

            if (raw is T match)
                return match;

            return ConvertGeneral<T>(raw);
        }

        private static object ExtractNumber(JsonElement element, Type targetType)
        {
            // Extract using the TARGET type's own getter — avoids the
            // "always GetDouble()" mismatch bug from earlier
            if (targetType == typeof(int)) return element.GetInt32();
            if (targetType == typeof(long)) return element.GetInt64();
            if (targetType == typeof(short)) return element.GetInt16();
            if (targetType == typeof(byte)) return element.GetByte();
            if (targetType == typeof(decimal)) return element.GetDecimal();
            if (targetType == typeof(float)) return element.GetSingle();

            return element.GetDouble(); // fallback for double/unknown numeric target
        }

        private static object ExtractNumber(JsonElement element)
        {
            if (element.TryGetInt32(out int i)) return i;
            if (element.TryGetInt64(out long l)) return l;
            if (element.TryGetDecimal(out decimal d)) return d;
            return element.GetDouble();
        }

        private static T? ConvertGeneral<T>(object value)
        {
            try
            {
                var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)System.Convert.ChangeType(value, targetType);
            }
            catch
            {
                return default;
            }
        }
    }
}
