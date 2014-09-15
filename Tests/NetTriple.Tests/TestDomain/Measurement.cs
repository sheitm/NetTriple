using System.Collections.Generic;
using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Measurement")]
    [RdfPropertyOnClass(PredicateBase = "http://netriple.com/unittesting/measurement", PropertyPredicatePairs = "Value,/value;Unit,/unit")]
    [RdfChildrenOnClass(PropertyPredicateTriples = "MeasurementsPerHour,http://netriple.com/unittesting/measurements_per_hour,false;Meter,http://netriple.com/unittesting/meter_measurements,true")]
    public class Measurement
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/measurement/{0}")]
        public string Id { get; set; }

        public float Value { get; set; }

        public string Unit { get; set; }

        public IEnumerable<HourMeasurement> MeasurementsPerHour { get; set; }

        public Meter Meter { get; set; }
    }
}
