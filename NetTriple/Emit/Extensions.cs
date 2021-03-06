﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTriple.Annotation.Internal;
using NodaTime;

namespace NetTriple.Emit
{
    public static class Extensions
    {
        public static string ToTripleObject(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string)
            {
                return string.Format("\"{0}\"^^<http://www.w3.org/2001/XMLSchema#string>", obj);
            }

            if (obj is decimal || obj is double || obj is float)
            {
                return string.Format("\"{0}\"^^<http://www.w3.org/2001/XMLSchema#{1}>", obj.ToString().Replace(',', '.'), obj.GetType().Name.ToLower());
            }

            if (obj is bool)
            {
                return string.Format("\"{0}\"^^<http://www.w3.org/2001/XMLSchema#boolean>", obj.ToString().ToLower());
            }

            if (obj is DateTime)
            {
                var dt = (DateTime) obj;
                return "\"" + dt.ToUniversalTime().ToString("u").Replace(" ", "T") + "\"^^<http://www.w3.org/2001/XMLSchema#dateTime>";
            }

            if (obj is Instant)
            {
                return string.Format("\"{0}\"^^<http://www.w3.org/2001/XMLSchema#dateTime>", obj);
            }

            return string.Format("\"{0}\"", obj.ToString());
        }

        public static T ToDeserializedStructObject<T>(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return default(T);
            }

            return ReflectionHelper.Deserialize<T>(s);
        }

        public static Triple ToTriple(this string ts)
        {
            var arr = ts.Split(null);
            string obj;
            if (arr.Length == 3)
            {
                obj = arr[2];
            }
            else
            {
                var sb = new StringBuilder();
                for (int i = 2; i < arr.Length; i++)
                {
                    sb.Append(arr[i]);
                    if (i < arr.Length - 1)
                    {
                        sb.Append(" ");
                    }
                }

                obj = sb.ToString();
            }

            return new Triple
            {
                Subject = arr[0],
                Predicate = arr[1],
                Object = obj
            };
        }

        public static string GetIdOfSubject(this string subject)
        {
            var arr = subject.Split('/');
            var id = arr[arr.Length - 1].Trim();
            return id.EndsWith(">") ? id.Substring(0, id.Length - 1) : id;
        }

        public static string ToWashedRdfSubject(this string subjectTemplate)
        {
            var arr = subjectTemplate.Split('/');
            var sb = new StringBuilder();
            for (int i = 0; i < arr.Length-1; i++)
            {
                sb.Append(arr[i]);
                if (i < arr.Length - 2)
                {
                    sb.Append("/");
                }
            }

            var washed = sb.ToString();
            return washed.StartsWith("<")
                ? washed.Substring(1, washed.Length - 1)
                : washed;
        }

        public static IEnumerable<string[]> DeserializeStructListString(this string s)
        {
            return ReflectionHelper.WashStringObject(s).Split(new string[] {"##"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(se => se.DeserializeStructString());
        }

        public static string[] DeserializeStructString(this string s)
        {
            return s.Split(new string[] { ";;" }, StringSplitOptions.None);
        }
    }
}
