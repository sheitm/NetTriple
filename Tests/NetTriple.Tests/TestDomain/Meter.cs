using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Meter")]
    public class Meter
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/meter/{0}")]
        public int Id { get; set; }
    }
}
