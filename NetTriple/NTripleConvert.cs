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
            return context.GetFirstOfType(type);
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
    }
}
