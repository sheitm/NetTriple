using System;
using System.Linq;
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
            var generator = new LinkerSourceCodeGenerator(typeof(Book), property, attrib);
            var sb = new StringBuilder();

            // Act
            generator.AppendSourceCode(sb);

            // Assert
            var code = sb.ToString();
            Assert.IsTrue(code.Contains("var allChapters = context.GetAll<NetTriple.Tests.TestDomain.Chapter>(s, \"http://netriple.com/unittesting/book/contained_chapter\");"));
            Assert.IsTrue(code.Contains("if (allChapters != null && allChapters.Count() > 0)"));
        }

        [TestMethod]
        public void AppendSourceCode_ForInverseListRelation_AppendsExpectedCode()
        {
            // Arrange
            var property = typeof (Order).GetProperty("Details");
            var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof(RdfChildrenAttribute));
            var generator = new LinkerSourceCodeGenerator(typeof(Order), property, attrib);
            var sb = new StringBuilder();

            // Act
            generator.AppendSourceCode(sb);

            // Assert
            var code = sb.ToString();
            Assert.IsTrue(code.Contains("var allDetails = context.GetAllInverse<NetTriple.Tests.TestDomain.OrderDetail>(s, \"http://netriple.com/elements/owning_order\");"));
            Assert.IsTrue(code.Contains("if (allDetails != null && allDetails.Count() > 0)"));
        }

        [TestMethod]
        public void AppendSourceCode_ForInverseUnaryRelation_AppendsExpectedCode()
        {
            // Arrange
            var property = typeof(Husband).GetProperty("Wife");
            var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof(RdfChildrenAttribute));
            var generator = new LinkerSourceCodeGenerator(typeof(Husband), property, attrib);
            var sb = new StringBuilder();

            // Act
            generator.AppendSourceCode(sb);

            // Assert
            var code = sb.ToString();
            Assert.IsTrue(code.StartsWith("obj.Wife = context.GetInverse<NetTriple.Tests.TestDomain.Wife>(s, \"http://netriple.com/unittesting/wife_to_husband\");"));
        }
    }
}
