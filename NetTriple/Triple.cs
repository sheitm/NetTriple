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
