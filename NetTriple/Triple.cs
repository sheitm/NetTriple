﻿namespace NetTriple
{
    public class Triple
    {
        public string Subject { get; set; }
        public string Predicate { get; set; }

        private string _object;

        public string Object
        {
            get { return _object; }
            set
            {
                _object = value;
            }
        }

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
            object res;
            if (typeof(int) == typeof(T))
            {
                res = int.Parse(Object);
                return (T)res;
            }

            if (typeof(string) == typeof(T))
            {
                return (T)WashStringObject();
            }

            res = Object;
            return (T)res;
        }

        public bool IsMatch(string subject, string predicate)
        {
            var subj = subject.StartsWith("<") ? subject : string.Format("<{0}>", subject);
            var pred = predicate.StartsWith("<") ? predicate : string.Format("<{0}>", predicate);
            return Subject == subj && Predicate == pred;
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
