using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTriple.Emit;

namespace NetTriple
{
    public static class NTripleConvert
    {
        public static IEnumerable<Triple> ToTriples(this object obj)
        {
            var converter = LoadAllRdfClasses.GetConverter(obj.GetType());

            var triples = new List<Triple>();
            converter.Convert(obj, triples, LoadAllRdfClasses.GetLocator());
            return triples;
        }

        public static T ToObject<T>(this IEnumerable<Triple> triples)
        {
            return (T)ToObject(typeof(T), triples);
        }

        public static object ToObject(Type type, IEnumerable<Triple> triples)
        {
            var context = Expand(triples);
            return context.GetFirstOfType(type);
        }

        public static IEnumerable<object> ToObjects(this IEnumerable<Triple> triples)
        {
            var context = Expand(triples);
            return context.GetAll();
        }

        public static Triple ConvertToTriple(this string s)
        {
            var arr = s.Split(null);
            string v;
            if (arr.Length > 3)
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

                v = sb.ToString();
            }
            else
            {
                v = arr[2];
            }

            return new Triple
            {
                Subject = arr[0],
                Predicate = arr[1],
                Object = v
            };
        }

        private static IDictionary<string, List<Triple>> GetSubjectMap(IEnumerable<Triple> triples)
        {
            return triples.Aggregate(
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
        }

        private static IInflationContext Expand( IEnumerable<Triple> triples)
        {
            var subjectMap = GetSubjectMap(triples);
            var context = new InflationContext(triples);
            var locator = LoadAllRdfClasses.GetLocator();

            foreach (var pair in subjectMap)
            {
                var thisType = LoadAllRdfClasses.GetTypeForSubject(pair.Key);
                var converter = LoadAllRdfClasses.GetConverter(thisType);
                converter.Inflate(pair.Value.Select(t => t.ToString()), context, locator);
            }

            context.LinkAll(locator);
            return context;
        }
    }
}
