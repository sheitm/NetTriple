using System.Collections.Generic;

namespace NetTriple.Emit
{
    public interface IConverter
    {
        void Convert(object obj, IList<Triple> triples, IConverterLocator locator, string parentSubject = null, string parentReferencePredicate = null);

        void Inflate(IEnumerable<string> triples, IInflationContext context, IConverterLocator locator);
        void Link(object obj, IInflationContext context);
    }
}
