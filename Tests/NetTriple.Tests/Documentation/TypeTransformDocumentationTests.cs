using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Documentation;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Documentation
{
    [TestClass]
    public class TypeTransformDocumentationTests
    {
        [TestMethod]
        public void Constructor_WithValidTransform_RdfTypeSetCorrectly()
        {
            // Arrange
            var bt = GetTransform();

            // Act
            var doc = new TypeTransformDocumentation(bt);

            // Assert
            Assert.AreEqual("http://nettriple/Player", doc.RdfType);
        }

        [TestMethod]
        public void Constructor_WithValidTransform_TypeSetCorrectly()
        {
            // Arrange
            var bt = GetTransform();

            // Act
            var doc = new TypeTransformDocumentation(bt);

            // Assert
            Assert.AreEqual("NetTriple.Tests.TestDomain.Player", doc.Type);
        }

        [TestMethod]
        public void Constructor_WithValidTransform_PropertiesSetAsExpected()
        {
            // Arrange
            var bt = GetTransform();

            // Act
            var doc = new TypeTransformDocumentation(bt);

            // Assert
            Assert.AreEqual(4, doc.Properties.Count());
            foreach (var property in doc.Properties)
            {
                Console.WriteLine(string.Format("{0} {1} {2}", property.Predicate, property.PropertyName, property.PropertyType));
            }
        }

        [TestMethod]
        public void Constructor_WithValidTransform_RelationsSetAsExpected()
        {
            // Arrange
            var bt = GetTransformWithRelation();

            // Act
            var doc = new TypeTransformDocumentation(bt);

            // Assert
            Assert.AreEqual(2, doc.Relations.Count());
            foreach (var relation in doc.Relations)
            {
                Console.WriteLine("{0} {1} {2}", relation.Predicate, relation.PropertyName, relation.PropertyType);
            }
        }

        private IBuiltTransform GetTransform()
        {
            return BuildTransform.For<Player>("http://nettriple/Player")
                .Subject(p => p.Id, "http://nettriple/player/{0}")
                .WithPropertyPredicateBase("http://nettriple/player")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Name, "/name")
                .Prop(p => p.Gender, "/gender")
                .Prop(p => p.DateOfBirth, "/dateOfBirth");
        }

        private IBuiltTransform GetTransformWithRelation()
        {
            return BuildTransform.For<Match>("http://nettriple/Match")
                .Subject(p => p.Id, "http://nettriple/match/schema/{0}")
                .WithPropertyPredicateBase("http://nettriple/match/schema")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Date, "/date")
                .Relation(p => p.Player1, "/p1")
                .Relation(p => p.Player2, "/p2");
        }
    }
}
