using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Fluency
{
    [TestClass]
    public class ReflectedBuiltTransformTests
    {
        [TestMethod]
        public void Constructor_ValidInput_TypeStringSetCorrectly()
        {
            // Arrange
            var type = typeof (AddressableDevice);
            var rdfBase = "http://nettriples.com/testing";

            // Act
            var transform = new ReflectedBuiltTransform(type, rdfBase);

            // Assert
            Assert.AreEqual("http://nettriples.com/testing/addressableDevice", transform.TypeString);
        }

        [TestMethod]
        public void Constructor_ValidInput_TypePredicateSetAsExpected()
        {
            // Arrange
            var type = typeof(AddressableDevice);
            var rdfBase = "http://nettriples.com/testing";

            // Act
            var transform = new ReflectedBuiltTransform(type, rdfBase);

            // Assert
            Assert.AreEqual("http://www.w3.org/1999/02/22-rdf-syntax-ns#type", transform.TypePredicate);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ComputeStructure_LocatorNotSet_ThrowsInvalidOperationException()
        {
            // Arrange
            var type = typeof(AddressableDevice);
            var rdfBase = "http://nettriples.com/testing";
            var transform = new ReflectedBuiltTransform(type, rdfBase);

            // Act
            transform.ComputeStructure();
        }

        public void ComputeStructure_For_ThrowsInvalidOperationException()
        {
            // Arrange
            var type = typeof(AddressableDevice);
            var rdfBase = "http://nettriples.com/testing";
            var transform = new ReflectedBuiltTransform(type, rdfBase);

            // Act
            transform.ComputeStructure();
        }
    }
}
