using System.Globalization;

namespace OpenRP.Framework.Features.Properties.Helpers
{
    /// <summary>
    /// Writes values as C# source code, for generated files.
    /// </summary>
    public static class CSharpLiterals
    {
        private const string Null = "null";

        public static string GetString(string? value)
        {
            if (value == null)
            {
                return Null;
            }

            string escaped = value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
            return $"\"{escaped}\"";
        }

        /// <summary>
        /// Round-trip precision, so the generated value is exactly the original one.
        /// </summary>
        public static string GetFloat(float value)
        {
            return $"{value.ToString("R", CultureInfo.InvariantCulture)}f";
        }

        public static string GetInt(int? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : Null;
        }

        /// <summary>
        /// With the ul suffix where the value is passed as an object (for example to UpdateData), so it keeps its type.
        /// </summary>
        public static string GetULong(ulong? value, bool withSuffix = false)
        {
            if (!value.HasValue)
            {
                return Null;
            }

            string number = value.Value.ToString(CultureInfo.InvariantCulture);
            return withSuffix ? $"{number}ul" : number;
        }
    }
}
