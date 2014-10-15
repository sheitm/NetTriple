using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Internal;

namespace NetTriple.Tests.Internal
{
    [TestClass]
    public class StringHelperTests
    {
        [TestMethod]
        public void UnescapeLiteral_WithEuro_ReturnsExpectedString()
        {
            // Arrange
            var s = "There's a euro here: \u20AC";

            // Act
            var r = s.UnescapeLiteral();

            // Assert
            Assert.AreEqual("There's a euro here: €", r);
        }

        [TestMethod]
        public void UnescapeLiteral_WithNorwegiaLetters_ReturnsExpectedString()
        {
            // Arrange
            var s = "\u00C5\u00F8\u00E6";

            // Act
            var r = s.UnescapeLiteral();

            // Assert
            Assert.AreEqual("Åøæ", r);
        }

        [TestMethod]
        public void RemoveRdfTypeInfo_WithTypeInfo_ItIsRemoved()
        {
            // Arrange
            var input = "\"909\"^^<http://example.org/datatype1>";

            // Act
            var output = input.RemoveRdfTypeInfo();

            // Assert
            Assert.AreEqual("\"909\"", output);
        }

        [TestMethod]
        public void RemoveRdfTypeInfo_WithoutTypeInfo_SameStringIsreturned()
        {
            // Arrange
            var input = "No type information here";

            // Act
            var output = input.RemoveRdfTypeInfo();

            // Assert
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void RemoveLeadingAndTrailingQuotes_WithQuotes_QuotesAreRemoved()
        {
            // Arrange
            var input = "\"909\"";

            // Act
            var output = input.RemoveLeadingAndTrailingQuotes();

            // Assert
            Assert.AreEqual("909", output);
        }

        [TestMethod]
        public void RemoveLeadingAndTrailingQuotes_NoQuotes_SameStringReturned()
        {
            // Arrange
            var input = "909";

            // Act
            var output = input.RemoveLeadingAndTrailingQuotes();

            // Assert
            Assert.AreEqual(input, output);
        }
    }
}
