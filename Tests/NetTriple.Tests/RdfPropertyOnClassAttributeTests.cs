using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests
{
    [TestClass]
    public class RdfPropertyOnClassAttributeTests
    {
        [TestMethod]
        public void GetPropertyPredicateSpecifications_ValidDataWithPredicateBase_ReturnsExpectedSpecs()
        {
            // Arrange
            var attrib = new RdfPropertyOnClassAttribute
            {
                PredicateBase = "http://nettriple/measurement",
                PropertyPredicatePairs = "Value,/value;Unit,/Unit"
            };

            // Act
            var specs = attrib.GetPropertyPredicateSpecifications(typeof (Measurement)).ToList();

            // Assert
            Assert.AreEqual(2, specs.Count);
            Assert.IsTrue(specs.Any(s => s.Predicate == "http://nettriple/measurement/value" && s.Property.Name == "Value"));
        }
    }
}
