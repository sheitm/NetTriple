using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation;
using NetTriple.Emit;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class LinkerSourceCodeGeneratorTests
    {
        [TestMethod]
        public void Constructor_ValidInput_ConstructsInstance()
        {
            // Arrange
            var property = typeof (BookReview).GetProperty("Book");
            var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof (RdfChildrenAttribute));

            // Act
            var generator = new LinkerSourceCodeGenerator(typeof(BookReview), property, attrib);

            // Act
            Assert.IsNotNull(generator);
        }

        [TestMethod]
        public void AppendSubjectAssignment_HappyDays_AppendsExpectedCode()
        {
            // Arrange
            var property = typeof(BookReview).GetProperty("Book");
            var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof(RdfChildrenAttribute));
            var generator = new LinkerSourceCodeGenerator(typeof(BookReview), property, attrib);
            var sb = new StringBuilder();

            // Act
            generator.AppendSubjectAssignment(sb);

            // Assert
            var code = sb.ToString();
            Assert.IsTrue(code.Contains("var s1 = obj.Id.ToString();"));
            Assert.IsTrue(code.Contains("var template = \"<http://netriple.com/unittesting/book_review/{0}>\";"));
            Assert.IsTrue(code.Contains("var s = template.Replace(\"{0}\", s1);"));
        }

        [TestMethod]
        public void AppendSourceCode_ForForwardNonListRelation_AppendsExpectedCode()
        {
            // Arrange
            var property = typeof(BookReview).GetProperty("Book");
            var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof(RdfChildrenAttribute));
            var generator = new LinkerSourceCodeGenerator(typeof(BookReview), property, attrib);
            var sb = new StringBuilder();

            // Act
            generator.AppendSourceCode(sb);

            // Assert
            Assert.AreEqual("obj.Book = context.Get<NetTriple.Tests.TestDomain.Book>(s, \"http://netriple.com/unittesting/book_review/book\");\r\n", sb.ToString());
        }

        [TestMethod]
        public void AppendSourceCode_ForForwardListRelation_AppendsExpectedCode()
        {
            // Arrange
            var property = typeof (Book).GetProperty("Chapters");
            var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof(RdfChildrenAttribute));
            var generator = new LinkerSourceCodeGenerator(typeof(BookReview), property, attrib);
            var sb = new StringBuilder();

            // Act
            generator.AppendSourceCode(sb);

            // Assert
            var code = sb.ToString();
        }
    }
}
