namespace NetTriple.Annotation.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Measurement")]
    [RdfPropertyOnClass(PredicateBase = "http://netriple.com/unittesting/measurement", PropertyPredicatePairs = "Value,/value;Unit,/unit")]
    public class Measurement
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/measurement/{0}")]
        public string Id { get; set; }

        public float Value { get; set; }

        public string Unit { get; set; }
    }
}
