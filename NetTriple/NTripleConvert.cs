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
            var converter = LoadAllRdfClasses.GetConverter(type);
            var context = new InflationContext();
            converter.Inflate(triples.Select(t => t.ToString()), context, LoadAllRdfClasses.GetLocator());

            return context.GetFirstOfType(type);
        }
    }
}
