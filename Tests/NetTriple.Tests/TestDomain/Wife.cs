using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/wife")]
    public class Wife
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/wife/{0}")]
        public int Id { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/wife/name")]
        public string Name { get; set; }
    }
}
