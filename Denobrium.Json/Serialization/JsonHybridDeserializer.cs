using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Denobrium.Json.Data;
using Denobrium.Json.Serialization;

namespace Denobrium.Json
{
    /// <summary>
    /// This class decodes JSON strings. The root level object is always a mutable json array or a mustable json object. 
    /// Nested types are also mutables, value types are roghly parsed into long, double etc.
    /// Spec. details, see http://www.json.org/
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// </remarks>
    internal class JsonHybridDeserializer : JsonDeserializer
    {
        private Encoding _encoding;

        /// <summary>
        /// Creates a parser for the given json string. 
        /// </summary>
        /// <param name="ignorecase"></param>
        internal JsonHybridDeserializer(JsonParameters parameters)
        {
            _encoding = parameters.Encoding;
        }

        /// <summary>
        /// Deserializes an input stream and returns a mutable json object or a mutable json array.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public Object Deserialize(Stream inputStream)
        {
            StreamReader reader = new StreamReader(inputStream, _encoding);
            String jsonString = reader.ReadToEnd();

            return Deserialize(ref jsonString);
        }

        /// <summary>
        /// Returns the object represented by the string at construction time.
        /// </summary>
        /// <returns></returns>
        public IEnumerable Deserialize(ref string jsonString)
        {
            this._jsonOriginal = jsonString.ToCharArray();
            this._index = 0;

            return (IEnumerable) ParseValue();
        }

        private object ParseValue()
        {
            switch (LookAhead())
            {
                case Token.Number:
                    return ParseNumber();

                case Token.String:
                    return ParseString();

                case Token.Curly_Open:
                    return ParseObject();

                case Token.Squared_Open:
                    return ParseArray();

                case Token.True:
                    ConsumeToken();
                    return true;

                case Token.False:
                    ConsumeToken();
                    return false;

                case Token.Null:
                    ConsumeToken();
                    return null;
            }

            throw new SerializationException("Unrecognized token at index" + _index);
        }

        private MutableJsonObject ParseObject()
        {
            MutableJsonObject table = new MutableJsonObject();

            ConsumeToken(); // {

            while (true)
            {
                switch (LookAhead())
                {
                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Curly_Close:
                        ConsumeToken();
                        return table;

                    default:
                        {
                            // name
                            string name = ParseString();

                            // :
                            if (NextToken() != Token.Colon)
                            {
                                throw new SerializationException("Expected colon at index " + _index);
                            }

                            // value
                            object value = ParseValue();

                            table[name] = value;
                        }
                        break;
                }
            }
        }

        private MutableJsonArray ParseArray()
        {
            MutableJsonArray array = new MutableJsonArray();
            ConsumeToken(); // [

            while (true)
            {
                switch (LookAhead())
                {

                    case Token.Comma:
                        ConsumeToken();
                        break;

                    case Token.Squared_Close:
                        ConsumeToken();
                        return array;

                    default:
                        array.Add(ParseValue());
                        break;
                }
            }
        }

        private object ParseNumber()
        {
            ConsumeToken();

            // Need to start back one place because the first digit is also a token and would have been consumed
            var startIndex = _index - 1;
            bool dec = false;

            do
            {
                var c = _jsonOriginal[_index];

                if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
                {
                    if (c == '.' || c == 'e' || c == 'E')
                    {
                        dec = true;
                    }

                    if (++_index == _jsonOriginal.Length)
                    {
                        break;                        //throw new Exception("Unexpected end of string whilst parsing number");
                    }

                    continue;
                }

                break;

            } while (true);

            string s = new string(_jsonOriginal, startIndex, _index - startIndex);

            if (dec)
            {
                return double.Parse(s, NumberFormatInfo.InvariantInfo);
            }

            return long.Parse(s);
        }
    }
}
