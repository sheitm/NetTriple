using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Emit;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class SourceCodeGeneratorTests
    {
        [TestMethod]
        public void GetSourceCode_ForSimpleType_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(OrderDetail));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsTrue(source.Contains("var template = \"<http://netriple.com/unittesting/orderdetail/{0}>\";"));
            Assert.AreEqual(3615, source.Length);
        }

        [TestMethod]
        public void GetSourceCode_ForComplexType_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(Order));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsNotNull(source);
        }

        [TestMethod]
        public void GetSourceCode_ForComplexTypeWithForwardNonListReference_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(Book));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsNotNull(source);
        }
    }
}
