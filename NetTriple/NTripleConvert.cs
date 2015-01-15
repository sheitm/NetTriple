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
            return triples.Where(t => t.Object != null);
        }

        public static T ToObject<T>(this IEnumerable<Triple> triples)
        {
            return ToObject<T>(triples, new List<string>());
        }

        public static T ToObject<T>(this IEnumerable<Triple> triples, IList<string> unknownRdfTypes)
        {
            return (T)ToObject(typeof(T), triples, unknownRdfTypes);
        }

        public static object ToObject(Type type, IEnumerable<Triple> triples)
        {
            return ToObject(type, triples, new List<string>());
        }

        public static object ToObject(Type type, IEnumerable<Triple> triples, IList<string> unknownRdfTypes)
        {
            var known = RemoveUnknowns(triples, unknownRdfTypes);
            var context = Expand(known);
            return context.GetFirstOfType(type);
        }

        public static IEnumerable<object> ToObjects(this IEnumerable<Triple> triples)
        {
            return ToObjects(triples, new List<string>());
        }

        public static IEnumerable<object> ToObjects(this IEnumerable<Triple> triples, IList<string> unknownRdfTypes)
        {
            var known = RemoveUnknowns(triples, unknownRdfTypes);
            var context = Expand(known);
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

        private static IInflationContext Expand(IEnumerable<Triple> triples)
        {
            var subjectMap = GetSubjectMap(triples);
            var context = new InflationContext(triples);
            var locator = LoadAllRdfClasses.GetLocator();

            foreach (var pair in subjectMap)
            {
                var thisType = LoadAllRdfClasses.GetTypeForTriples(pair.Value);
                var converter = LoadAllRdfClasses.GetConverter(thisType);
                converter.Inflate(pair.Value.Select(t => t.ToString()), context, locator);
            }

            context.LinkAll(locator);
            return context;
        }

        private static IEnumerable<Triple> RemoveUnknowns(IEnumerable<Triple> triples, IList<string> unknownRdfTypes)
        {
            const string typePredicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
            const string unknownPredicate = "<http://psi.hafslund.no/sesam/orphaned-entity>";

            var declared = LoadAllRdfClasses.DeclaredRdfTypes.ToList();
            if (declared == null || !declared.Any())
            {
                return triples;
            }

            var notDeclaredRdfTypeTriples = triples
                .Where(t => t.IsPredicateMatch(typePredicate))
                .Where(t => declared.All(d => d != t.Object.TrimStart(new[] {'<'}).TrimEnd(new[] {'>'})))
                .ToList();

            foreach (var notDeclared in notDeclaredRdfTypeTriples)
            {
                unknownRdfTypes.Add(string.Format("{0} {1} {2}", notDeclared.Subject, unknownPredicate, notDeclared.Object));
            }

            return triples.Where(t => notDeclaredRdfTypeTriples.All(nt => nt.Subject != t.Subject));
        }
    }
}
