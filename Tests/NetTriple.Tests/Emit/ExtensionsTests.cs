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

        [TestMethod]
        public void DeserializeStructString_ValidInput_DeserializesAsExpected()
        {
            // Arrange
            var s = "13,348;;14.06.2014 00:00:00;;60;;GE;;OK;;OKKK";

            // Act
            var arr = s.DeserializeStructString();

            // Assert
            Assert.AreEqual(6, arr.Length);
            Assert.AreEqual("13,348", arr[0]);
            Assert.AreEqual("14.06.2014 00:00:00", arr[1]);
            Assert.AreEqual("OKKK", arr[5]);
        }

        [TestMethod]
        public void DeserializeStructString_InputWithBlanks_DeserializesAsExpected()
        {
            // Arrange
            var s = "13,348;;;;60;;GE;;;;OKKK";

            // Act
            var arr = s.DeserializeStructString();

            // Assert
            Assert.AreEqual(6, arr.Length);
            Assert.AreEqual(string.Empty, arr[1]);
        }

        [TestMethod]
        public void DeserializeStructListString_ForList_DeserializesAsExpected()
        {
            // Arrange
            var s = "13,348;;14.06.2014 00:00:00;;60;;GE;;OK;;OK##10,358;;14.06.2014 01:00:00;;60;;GE;;OK;;OK##13,348;;;;60;;GE;;;;OKKK##";

            // Act
            var arr = s.DeserializeStructListString().ToList();
        }

        [TestMethod]
        public void ToTripleObject_StringWithoutSpecialChars_IsWrappedInQuotes()
        {
            // Arrange
            var s = "This is a regular string";

            // Act
            var r = s.ToTripleObject();

            // Assert
            Assert.AreEqual(string.Format("\"{0}\"", s), r);
        }
    }
}
