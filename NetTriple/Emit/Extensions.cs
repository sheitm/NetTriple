using System.Text;

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

            if (obj is decimal || obj is double || obj is float)
            {
                return obj.ToString().Replace(',', '.');
            }

            return obj.ToString();
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
    }
}
