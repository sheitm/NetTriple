﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace NetTriple
{
    public static class Extensions
    {
        private const string TypePredicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

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

        public static IEnumerable<Triple> ToTypeUniqueTriples(this IEnumerable<Triple> triples)
        {
            var subjectMap = GetSubjectMap(triples);

            return subjectMap.Aggregate(
                new List<Triple>(),
                (result, pair) =>
                {
                    var c = pair.Value.Count(t => t.Predicate.Contains(TypePredicate));
                    if (c <= 1)
                    {
                        result.AddRange(pair.Value);
                    }
                    else
                    {
                        foreach (var duplicated in DuplicateMultitypeTriples(pair.Value))
                        {
                            result.AddRange(duplicated);
                        }
                    }

                    return result;
                });
        }

        public static string ToEnumValueFromRdfEnum(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            var p = s.EndsWith("/>")
                ? s.Substring(0, s.Length - 2)
                : s;

            var arr = p.TrimEnd('/').Split(new[] {'/'});
            return arr[arr.Length - 1].TrimEnd(new[] { '>' });
        }

        private static IEnumerable<IEnumerable<Triple>> DuplicateMultitypeTriples(IEnumerable<Triple> triples)
        {
            return triples
                .Where(t => t.Predicate.Contains(TypePredicate))
                .Select(t => t.Object)
                .Select(typeString =>
                {
                    var subject = string.Format("<{0}>", Guid.NewGuid().ToString());

                    var list = new List<Triple>();
                    list.Add(new Triple { Subject = subject, Predicate = string.Format("<{0}>", TypePredicate), Object = typeString });
                    list.AddRange(
                        triples
                        .Where(t => !t.Predicate.Contains(TypePredicate))
                        .Select(t => new Triple{Subject = subject, Predicate = t.Predicate, Object = t.Object}));

                    return list;
                });
        }

        private static Dictionary<string, List<Triple>> GetSubjectMap(IEnumerable<Triple> triples)
        {
            var subjectMap = triples.Aggregate(
                new Dictionary<string, List<Triple>>(),
                (map, triple) =>
                {
                    List<Triple> list;
                    if (map.ContainsKey(triple.Subject))
                    {
                        list = map[triple.Subject];
                    }
                    else
                    {
                        list = new List<Triple>();
                        map[triple.Subject] = list;
                    }

                    list.Add(triple);

                    return map;
                });
            return subjectMap;
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
                .Where(line => !line.TrimStart().StartsWith("#"))
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

        public static string Compress(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var buffer = Encoding.UTF8.GetBytes(text);
            var ms = new MemoryStream();
            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;

            var compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            var gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        public static string Decompress(this string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (var ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
