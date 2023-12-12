using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Serialization
{
    /// <summary>
    /// Represents a base class for json related deserializations.
    /// </summary>
    internal abstract class JsonDeserializer
    {
        /// <summary>
        /// The original json string.
        /// </summary>
        protected char[] _jsonOriginal;

        /// <summary>
        /// The current processing index.
        /// </summary>
        protected int _index;

        private Token _lookAheadToken = Token.None;
        private readonly StringBuilder _localString;

        /// <summary>
        /// Creates an instance of the deserializer.
        /// </summary>
        public JsonDeserializer()
        {
            _localString = new StringBuilder();
        }

        #region Token Helpers

        protected Token LookAhead()
        {
            if (_lookAheadToken != Token.None) { return _lookAheadToken; }

            return _lookAheadToken = NextTokenCore();
        }

        protected void ConsumeToken()
        {
            _lookAheadToken = Token.None;
        }

        protected Token NextToken()
        {
            var result = _lookAheadToken != Token.None ? _lookAheadToken : NextTokenCore();

            _lookAheadToken = Token.None;

            return result;
        }

        private Token NextTokenCore()
        {
            char c;

            try
            {
                // Skip past whitespace
                do
                {
                    c = _jsonOriginal[_index];

                    if (c > ' ') break;
                    if (c != ' ' && c != '\t' && c != '\n' && c != '\r') break;

                } while (++_index < _jsonOriginal.Length);
            }
            catch (IndexOutOfRangeException e)
            {
                throw new SerializationException("String syntax error. See inner exception. ", e);
            }

            if (_index == _jsonOriginal.Length)
            {
                throw new SerializationException("Reached end of string unexpectedly");
            }

            c = _jsonOriginal[_index];

            _index++;

            switch (c)
            {
                case '{':
                    return Token.Curly_Open;

                case '}':
                    return Token.Curly_Close;

                case '[':
                    return Token.Squared_Open;

                case ']':
                    return Token.Squared_Close;

                case ',':
                    return Token.Comma;

                case '"':
                    return Token.String;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                case '+':
                case '.':
                    return Token.Number;

                case ':':
                    return Token.Colon;

                case 'f':
                    if (_jsonOriginal.Length - _index >= 4 &&
                        _jsonOriginal[_index + 0] == 'a' &&
                        _jsonOriginal[_index + 1] == 'l' &&
                        _jsonOriginal[_index + 2] == 's' &&
                        _jsonOriginal[_index + 3] == 'e')
                    {
                        _index += 4;
                        return Token.False;
                    }
                    break;

                case 't':
                    if (_jsonOriginal.Length - _index >= 3 &&
                        _jsonOriginal[_index + 0] == 'r' &&
                        _jsonOriginal[_index + 1] == 'u' &&
                        _jsonOriginal[_index + 2] == 'e')
                    {
                        _index += 3;
                        return Token.True;
                    }
                    break;

                case 'n':
                    if (_jsonOriginal.Length - _index >= 3 &&
                        _jsonOriginal[_index + 0] == 'u' &&
                        _jsonOriginal[_index + 1] == 'l' &&
                        _jsonOriginal[_index + 2] == 'l')
                    {
                        _index += 3;
                        return Token.Null;
                    }
                    break;

            }

            throw new SerializationException("Could not find token at index " + --_index);
        }

        #endregion

        #region Value Conversion and Parsing

        /// <summary>
        /// Returns the parsed string from 
        /// </summary>
        /// <returns></returns>
        protected string ParseString()
        {
            ConsumeToken(); // "

            _localString.Length = 0;

            int runIndex = -1;

            while (_index < _jsonOriginal.Length)
            {
                var c = _jsonOriginal[_index++];

                if (c == '"')
                {
                    if (runIndex != -1)
                    {
                        if (_localString.Length == 0)
                        {
                            return new string(_jsonOriginal, runIndex, _index - runIndex - 1);
                        }

                        _localString.Append(_jsonOriginal, runIndex, _index - runIndex - 1);
                    }

                    return _localString.ToString();
                }

                if (c != '\\')
                {
                    if (runIndex == -1)
                    {
                        runIndex = _index - 1;
                    }

                    continue;
                }

                if (_index == _jsonOriginal.Length) { break; }

                if (runIndex != -1)
                {
                    _localString.Append(_jsonOriginal, runIndex, _index - runIndex - 1);
                    runIndex = -1;
                }

                switch (_jsonOriginal[_index++])
                {
                    case '"':
                        _localString.Append('"');
                        break;

                    case '\\':
                        _localString.Append('\\');
                        break;

                    case '/':
                        _localString.Append('/');
                        break;

                    case 'b':
                        _localString.Append('\b');
                        break;

                    case 'f':
                        _localString.Append('\f');
                        break;

                    case 'n':
                        _localString.Append('\n');
                        break;

                    case 'r':
                        _localString.Append('\r');
                        break;

                    case 't':
                        _localString.Append('\t');
                        break;

                    case 'u':
                        {
                            int remainingLength = _jsonOriginal.Length - _index;

                            if (remainingLength < 4) { break; }

                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint = ParseUnicode(_jsonOriginal[_index], _jsonOriginal[_index + 1], _jsonOriginal[_index + 2], _jsonOriginal[_index + 3]);
                            _localString.Append((char)codePoint);

                            // skip 4 chars
                            _index += 4;
                        }
                        break;
                }
            }

            throw new SerializationException("Unexpectedly reached the end of string.");
        }

        private uint ParseUnicode(char c1, char c2, char c3, char c4)
        {
            uint p1 = ParseSingleChar(c1, 0x1000);
            uint p2 = ParseSingleChar(c2, 0x100);
            uint p3 = ParseSingleChar(c3, 0x10);
            uint p4 = ParseSingleChar(c4, 1);

            return p1 + p2 + p3 + p4;
        }

        private uint ParseSingleChar(char c1, uint multipliyer)
        {
            uint p1 = 0;

            if (c1 >= '0' && c1 <= '9')
            {
                p1 = (uint)(c1 - '0') * multipliyer;
            }
            else if (c1 >= 'A' && c1 <= 'F')
            {
                p1 = (uint)((c1 - 'A') + 10) * multipliyer;
            }
            else if (c1 >= 'a' && c1 <= 'f')
            {
                p1 = (uint)((c1 - 'a') + 10) * multipliyer;
            }

            return p1;
        }

        #endregion
    }
}
