using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetTriple.Tests
{
    [TestClass]
    public class ConverterSnippetsTests
    {
        [TestMethod]
        public void AddSnippet_ValidInput_ReturnsKey()
        {
            // Arrange
            Func<string, object> snippet = s => int.Parse(s.Substring(4, 2));

            // Act
            var key = ConverterSnippets.AddSnippet(snippet);

            // Assert
            Assert.AreEqual(8, key.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Invoke_NoSuchSnippet_ThrowsInvalidOperationException()
        {
            // Act
            ConverterSnippets.Invoke<string>("no_such_key", "input");
        }

        [TestMethod]
        public void Invoke_HappyDays_ReturnsExpectedOutput()
        {
            // Arrange
            Func<string, object> snippet = s => int.Parse(string.Format("20{0}", s.Substring(4, 2)));
            var key = ConverterSnippets.AddSnippet(snippet);

            // Act
            var result = ConverterSnippets.Invoke<int>(key, "010113");

            // Assert
            Assert.AreEqual(2013, result);
        }
    }
}
