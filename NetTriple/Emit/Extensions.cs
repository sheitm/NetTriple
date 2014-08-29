namespace NetTriple.Emit
{
    public static class Extensions
    {
        public static string ToTripleObject(this object obj)
        {
            if (obj is string)
            {
                return string.Format("\"{0}\"", obj);
            }

            return obj.ToString();
        }

        public static Triple ToTriple(this string ts)
        {
            var arr = ts.Split(null);
            return new Triple
            {
                Subject = arr[0],
                Predicate = arr[1],
                Object = arr[2]
            };
        }

        public static string GetIdOfSubject(this string subject)
        {
            var arr = subject.Split('/');
            var id = arr[arr.Length - 1].Trim();
            return id.EndsWith(">") ? id.Substring(0, id.Length - 1) : id;
        }
    }
}
