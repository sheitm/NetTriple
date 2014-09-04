using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/husband")]
    public class Husband
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/husband/{0}")]
        public int Id { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/husband/name")]
        public string Name { get; set; }

        [RdfChildren(Inverse = true, Predicate = "http://netriple.com/unittesting/wife_to_husband")]
        public Wife Wife { get; set; }
    }
}
