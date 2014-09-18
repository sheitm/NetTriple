using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests
{
    [TestClass]
    public class RdfChildrenOnClassAttributeTests
    {
        [TestMethod]
        public void GetChildrenPredicateSpecifications_ValidInput_GetsExpectedSpecifications()
        {
            // Arrange
            var attribute = new RdfChildrenOnClassAttribute
            {
                PropertyPredicateTriples = "MeasurementsPerHour,http://netriple.com/unittesting/measurements_per_hour,true"
            };

            // Act
            var specs = attribute.GetChildrenPredicateSpecifications(typeof (Measurement)).ToList();

            // Assert
            Assert.AreEqual(1, specs.Count);
            Assert.IsTrue(specs.Any(s => 
                s.Value.Predicate == "http://netriple.com/unittesting/measurements_per_hour" 
                && s.Value.Inverse
                && s.Key.Name == "MeasurementsPerHour"));
        }
    }
}
