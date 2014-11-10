using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/HourMeasurement")]
    public class HourMeasurement
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/hourmeasurement/{0}")]
        public string Id { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/hourmeasurement/level")]
        public int Level { get; set; }
    }
}
