using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Emit;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class LinkerSourceCodeGeneratorTests
    {
        [TestMethod]
        public void GetSourceCode_UnaryProperty_GetsExpectedCode()
        {
            // Arrange
            var generator = new LinkerSourceCodeGenerator(typeof (BookReview), "Book");

            // Act
            var code = generator.GetSourceCode();

            // Assert
        }
    }
}
