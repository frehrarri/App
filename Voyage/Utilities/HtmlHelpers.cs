using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Voyage.Utilities
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Returns "checked" if two values are equivalent. Works for enums, ints, strings, etc.
        /// </summary>
        public static IHtmlContent CheckedIf<T1, T2>(this IHtmlHelper html, T1 currentValue, T2 valueToCheck)
        {
            // Both numeric types (int, long, enum, byte, etc.)
            if (IsNumericType(typeof(T1)) && IsNumericType(typeof(T2)))
            {
                var cv = Convert.ToInt64(currentValue);
                var vc = Convert.ToInt64(valueToCheck);
                if (cv == vc)
                    return new HtmlString("checked");
            }
            else
            {
                // Fallback to standard equality for non-numeric types
                if (object.Equals(currentValue, valueToCheck))
                    return new HtmlString("checked");
            }

            return HtmlString.Empty;
        }

        private static bool IsNumericType(Type type)
        {
            return type.IsEnum || type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong);
        }

        public static IHtmlContent HiddenIf<T1, T2>(this IHtmlHelper html, T1 currentValue, T2 valueToCheck)
        {
            // Both numeric types (int, long, enum, byte, etc.)
            if (IsNumericType(typeof(T1)) && IsNumericType(typeof(T2)))
            {
                var cv = Convert.ToInt64(currentValue);
                var vc = Convert.ToInt64(valueToCheck);
                if (cv == vc)
                    return new HtmlString("hidden");
            }
            else
            {
                // Fallback to standard equality for non-numeric types
                if (object.Equals(currentValue, valueToCheck))
                    return new HtmlString("hidden");
            }

            return HtmlString.Empty;
        }
    }
}

