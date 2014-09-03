using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Emit;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void ToWashedRdfSubject_WithValidrdfSubjectTemplate_ReturnsWashedSubject()
        {
            // Arrange
            var template = "http://netriple.com/unittesting/book_review/{0}";

            // Act
            var washed = template.ToWashedRdfSubject();

            // Assert
            Assert.AreEqual("http://netriple.com/unittesting/book_review", washed);
        }
    }
}
