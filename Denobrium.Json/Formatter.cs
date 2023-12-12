using System.Collections.Generic;
using System.Text;

namespace Denobrium.Json
{
    /// <summary>
    /// Represents a class which formats a json string.
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9) could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects as well.
    /// </remarks>
    internal static class Formatter
    {
        /// <summary>
        /// Gets or sets the indent.
        /// </summary>
        public static string Indent = "    ";

        /// <summary>
        /// Appends the indent to the string.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="count"></param>
        public static void AppendIndent(StringBuilder sb, int count)
        {
            for (; count > 0; --count) sb.Append(Indent);
        }

        /// <summary>
        /// Returns true, if the character at the described position contains an escape character or not.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsEscaped(StringBuilder sb, int index)
        {
            bool escaped = false;
            while (index > 0 && sb[--index] == '\\') escaped = !escaped;
            return escaped;
        }

        /// <summary>
        /// Prints the given json string in a formatted way.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static string PrettyPrint(string jsonString)
        {
            var output = new StringBuilder(jsonString.Length * 2);
            char? quote = null;
            int depth = 0;

            for (int i = 0; i < jsonString.Length; ++i)
            {
                char ch = jsonString[i];

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            AppendIndent(output, ++depth);
                        }
                        break;
                    case '}':
                    case ']':
                        if (quote.HasValue)
                            output.Append(ch);
                        else
                        {
                            output.AppendLine();
                            AppendIndent(output, --depth);
                            output.Append(ch);
                        }
                        break;
                    case '"':
                    case '\'':
                        output.Append(ch);
                        if (quote.HasValue)
                        {
                            if (!IsEscaped(output, i))
                                quote = null;
                        }
                        else quote = ch;
                        break;
                    case ',':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            AppendIndent(output, depth);
                        }
                        break;
                    case ':':
                        if (quote.HasValue) output.Append(ch);
                        else output.Append(" : ");
                        break;
                    default:
                        if (quote.HasValue || !char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }
    }
}