using System.Collections.Generic;
using System.Linq;
using NetTriple.Emit;

namespace NetTriple.Tests
{
    public class SampleConverter : IConverter
    {
        public void Convert(object obj, IList<Triple> triples, IConverterLocator locator, string parentSubject = null, string parentReferencePredicate = null)
        {
            ConvertCore((NetTriple.Tests.TestDomain.Order)obj, triples, locator);
        }

        public void Inflate(IEnumerable<string> triples, IInflationContext context, IConverterLocator locator)
        {
            var instance = new NetTriple.Tests.TestDomain.Order();
            var tripleObjects = triples.Select(t => t.ToTriple()).ToList();
            foreach (var triple in tripleObjects.Where(t => t.Predicate.Contains("http://psi.hafslund.no/elements/child")))
            {
                context.AddParentReference(triple.Object, instance);
            }

            context.Add(tripleObjects.First().Subject, instance);
            InflateCore(instance, tripleObjects.ToList(), context);
        }

        public void Link(object obj, IInflationContext context)
        {
            throw new System.NotImplementedException();
        }

        public void InflateCore(NetTriple.Tests.TestDomain.Order obj, IList<Triple> triples, IInflationContext context)
        {
            Triple triple;
            triple = triples.SingleOrDefault(trp => trp.Predicate == "<http://netriple.com/unittesting/order/ordernumber>");
            if (triple != null) { obj.OrderNumber = triple.GetObject<int>(); }
        }

        private void ConvertCore(NetTriple.Tests.TestDomain.Order obj, IList<Triple> triples, IConverterLocator locator)
        {
            var s = obj.Id;
            triples.Add(new Triple { Subject = s, Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Object = "http://netriple.com/unittesting/Order" });
            triples.Add(new Triple { Subject = s, Predicate = "http://netriple.com/unittesting/order/ordernumber", Object = obj.OrderNumber.ToTripleObject() });
            if (!string.IsNullOrEmpty(obj.Description)) triples.Add(new Triple { Subject = s, Predicate = "http://netriple.com/unittesting/order/ordernumber", Object = obj.Description.ToTripleObject() });
            if (obj.Details != null)
            {
                foreach (var child in obj.Details)
                {
                    locator.GetConverter(child.GetType()).Convert(child, triples, locator);
                    var co = string.Format("<xxx/{0}>", child.Id);
                    triples.Add(new Triple { Subject = s, Predicate = "http://psi.hafslund.no/elements/child", Object = "co" });

                }
            }
        }
    }
}
