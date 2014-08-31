using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Emit;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class RelationSourceCodeGeneratorTests
    {
        [TestMethod]
        public void GetConversionScript_NoRelations_ReturnsEmptyScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Chapter));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Assert.AreEqual(0, sourceCode.Length);
        }

        [TestMethod]
        public void GetConversionScript_UnaryRelation_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(BookReview));

            // Act
            var sourceCode = generator.GetConversionScript();
        }

        [TestMethod]
        public void GetConversionScript_NonInverseRelations_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Book));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Assert.AreEqual(549, sourceCode.Length);
        }

        [TestMethod]
        public void GetConversionScript_HasRelations_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Order));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Assert.IsTrue(sourceCode.Contains("<http://netriple.com/unittesting/orderdetail/{0}>"));
        }
    }
}
