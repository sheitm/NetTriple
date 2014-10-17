using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetTriple.Internal
{
    public static class StringHelper
    {
        private const string RdbTypeRegexPattern = @"\^\^<http://[a-z,A-Z,0-9,.,/,#,:]*>";
        private const string RdbTypeRegexPatter2 = @"\^\^xsd:[a-z,A-Z]*";

        private static readonly List<Regex> TypeRegexes = new List<Regex>
        {
            new Regex(RdbTypeRegexPattern),
            new Regex(RdbTypeRegexPatter2)
        }; 

        public static string RemoveRdfTypeInfo(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            foreach (var regex in TypeRegexes)
            {
                var pair = RemoveRdfTypeInfoRegex(s, regex);
                if (pair.Key)
                {
                    return pair.Value;
                }
            }

            return s;
        }

        public static string RemoveLeadingAndTrailingQuotes(this string v)
        {
            var s = v.StartsWith("\"")
                ? v.Substring(1, v.Length - 1)
                : v;

            return s.EndsWith("\"")
                ? s.Substring(0, s.Length - 1)
                : s;
        }

        public static string UnescapeLiteral(this string value)
        {
            var ret = new StringBuilder();
            bool inEscape = false;
            bool inUtf16 = false;
            bool inUtf32 = false;
            var utf16Buff = new char[4];
            var utf32Buff = new char[8];
            int unicodeCharIx = 0;
            foreach (var c in value)
            {
                if (inEscape)
                {
                    switch (c)
                    {
                        case '\\':
                            ret.Append(c);
                            break;
                        case 't':
                            ret.Append('\t');
                            break;
                        case 'b':
                            ret.Append('\b');
                            break;
                        case 'n':
                            ret.Append('\n');
                            break;
                        case 'r':
                            ret.Append('\r');
                            break;
                        case 'f':
                            ret.Append('\f');
                            break;
                        case '"':
                            ret.Append('"');
                            break;
                        case '\'':
                            ret.Append('\'');
                            break;
                        case 'u':
                            inUtf16 = true;
                            unicodeCharIx = 0;
                            break;
                        case 'U':
                            inUtf32 = true;
                            unicodeCharIx = 0;
                            break;
                        case '~':
                        case '.':
                        case '-':
                        case '!':
                        case '$':
                        case '&':
                        case '(':
                        case ')':
                        case '*':
                        case '+':
                        case ',':
                        case ';':
                        case '=':
                        case '/':
                        case '?':
                        case '#':
                        case '@':
                        case '%':
                        case '_':
                            ret.Append(c);
                            break;
                        default:
                            throw new FormatException("Unrecognized escape sequence \\" + c + " in literal \"" + value + "\"");
                    }
                    inEscape = false;
                }
                else if (inUtf16)
                {
                    if (!IsHexDigit(c))
                    {
                        throw new FormatException("Unexpected non hex digit in unicode escaped string: " + c);
                    }
                    utf16Buff[unicodeCharIx++] = c;
                    if (unicodeCharIx == 4)
                    {
                        ret.Append(ConvertToUtf16Char(new string(utf16Buff)));
                        unicodeCharIx = 0;
                        inUtf16 = false;
                    }
                }
                else if (inUtf32)
                {
                    if (!IsHexDigit(c))
                    {
                        throw new FormatException("Unexpected non hex digit in unicode escaped string: " + c);
                    }
                    utf32Buff[unicodeCharIx++] = c;
                    if (unicodeCharIx == 4)
                    {
                        ret.Append(ConvertToUtf32Char(new string(utf32Buff)));
                        unicodeCharIx = 0;
                        inUtf32 = false;
                    }
                }
                else
                {
                    if (c == '\\')
                    {
                        inEscape = true;
                    }
                    else
                    {
                        ret.Append(c);
                    }
                }
            }
            return ret.ToString();
        }

        private static bool IsHexDigit(char c)
        {
            if (Char.IsDigit(c))
            {
                return true;
            }
            switch (c)
            {
                case 'A':
                case 'a':
                case 'B':
                case 'b':
                case 'C':
                case 'c':
                case 'D':
                case 'd':
                case 'E':
                case 'f':
                case 'F':
                    return true;
                default:
                    return false;
            }
        }

        private static char ConvertToUtf16Char(String hex)
        {
            try
            {
                ushort i = Convert.ToUInt16(hex, 16);
                return Convert.ToChar(i);
            }
            catch
            {
                throw new FormatException(String.Format("Unable to convert hex value {0} to a UTF-16 character", hex));
            }
        }

        private static string ConvertToUtf32Char(String hex)
        {
            try
            {
                //Convert to an Integer
                int i = Convert.ToInt32(hex, 16);
                return Char.ConvertFromUtf32(i);
            }
            catch
            {
                throw new FormatException("Unable to convert the String '" + hex + "' into a Unicode Character");
            }
        }

        private static KeyValuePair<bool, string> RemoveRdfTypeInfoRegex(string s, Regex regex)
        {
            if (!regex.IsMatch(s))
            {
                return new KeyValuePair<bool, string>(false, s);
            }

            var match = regex.Match(s);
            return new KeyValuePair<bool, string>(true, s.Substring(0, match.Index));
        }
    }
}
