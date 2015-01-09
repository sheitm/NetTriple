using System;
using NetTriple.Annotation.Internal;

namespace NetTriple
{
    public class Triple
    {
        public string Subject { get; set; }
        public string Predicate { get; set; }
        public string Object { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Subject, Predicate, Object);
        }

        public string GetDisplayString()
        {
            return string.Format("{0} {1} {2} .", Subject, Predicate, Object);
        }

        public T GetObject<T>()
        {
            return ReflectionHelper.Deserialize<T>(Object);
        }

        public bool IsMatch(string subject, string predicate)
        {
            var subj = subject.StartsWith("<") ? subject : string.Format("<{0}>", subject);
            var pred = predicate.StartsWith("<") ? predicate : string.Format("<{0}>", predicate);
            return Subject == subj && Predicate == pred;
        }

        public bool IsInverseMatch(string objectValue, string predicate)
        {
            var obj = objectValue.StartsWith("<") ? objectValue : string.Format("<{0}>", objectValue);
            var pred = predicate.StartsWith("<") ? predicate : string.Format("<{0}>", predicate);
            return Object == obj && Predicate == pred;
        }

        public bool IsPredicateMatch(string predicate)
        {
            var pred = predicate.StartsWith("<") ? predicate : string.Format("<{0}>", predicate);
            return Predicate == pred;
        }

        private object WashStringObject()
        {
            if (string.IsNullOrWhiteSpace(Object))
            {
                return Object;
            }

            var s = Object.StartsWith("\"")
                ? Object.Substring(1, Object.Length - 1)
                : Object;

            return s.EndsWith("\"")
                ? s.Substring(0, s.Length - 1)
                : s;
        }
    }
}
