using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NetTriple
{
    public static class Extensions
    {
        /// <summary>
        /// Converts the triples to the string format NTriples, and
        /// then transforms this string to byte representation.
        /// </summary>
        /// <param name="triples"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this IEnumerable<Triple> triples)
        {
            var str = triples.ToNTriples();
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// Converts the bytes to a list of triples. The bytes are assumed
        /// to be a byte representation of of a string of triples on the
        /// NTriples format.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IEnumerable<Triple> ToTriples(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            var nTriples = new string(chars);
            return nTriples.ToTriplesFromNTriples();
        }

        public static string ToNTriples(this IEnumerable<Triple> triples)
        {
            return triples
                .Aggregate(
                    new StringBuilder(),
                    (sb, triple) =>
                    {
                        sb.AppendFormat("{0} .\r\n", triple);
                        return sb;
                    }).ToString();
        }

        public static IEnumerable<Triple> ToTriplesFromNTriples(this string nTriples)
        {
            return nTriples.Split(new string[] {Environment.NewLine}, StringSplitOptions.None)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    line = line.Trim();
                    if (line.EndsWith("."))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    var arr = line.Split(' ');
                    string objString = "";
                    for (int i = 2; i < arr.Length; i++)
                    {
                        if (i > 2)
                        {
                            objString += " ";
                        }

                        objString += arr[i];
                    }

                    return new Triple
                    {
                        Subject = arr[0],
                        Predicate = arr[1],
                        Object = objString.Trim()
                    };
                });
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            if (!char.IsLetter(s[0]))
            {
                return s;
            }

            if (char.IsLower(s[0]))
            {
                return s;
            }

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var lwr = textInfo.ToLower(s[0]);
            return string.Format("{0}{1}", lwr, s.Substring(1, s.Length-1));
        }
    }
}
