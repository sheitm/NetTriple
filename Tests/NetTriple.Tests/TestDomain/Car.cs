using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Car")]
    [RdfSubjectOnClass(Property = "Id", Template = "http://netriple.com/unittesting/car/{0}")]
    public class Car
    {
        public string Id { get; set; }
    }
}
