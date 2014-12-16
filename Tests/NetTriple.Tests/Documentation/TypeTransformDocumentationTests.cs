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
        public void Constructor_WithValidTransform_XmlNamespaceSetCorrectly()
        {
            // Arrange
            var bt = GetTransform();

            // Act
            var doc = new TypeTransformDocumentation(bt);

            // Assert
            Assert.AreEqual("http://nettriples.com/services", doc.XmlNamespace);
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

        [TestMethod]
        public void GetSampleNTriples_ValidTransform_GetsExpectedTripes()
        {
            // Arrange
            var bt = GetTransform();
            var doc = new TypeTransformDocumentation(bt);

            // Act
            var nTriples = doc.GetSampleNTriples();

            // Assert
            Console.WriteLine(nTriples);
        }

        [TestMethod]
        public void GetSampleNTriples_WithStruct_GetsExpectedTripes()
        {
            // Arrange
            var bt = GetTransformWithStruct();
            var doc = new TypeTransformDocumentation(bt);

            // Act
            var nTriples = doc.GetSampleNTriples();

            // Assert
            Console.WriteLine(nTriples);
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

        private IBuiltTransform GetTransformWithStruct()
        {
            return BuildTransform.For<Sr>("http://psi.hafslund.no/sesam/quant/meterreading-day")
               .Subject(s => s.Id, "http://psi.hafslund.no/sesam/quant/meterreading-day/{0}")
               .WithPropertyPredicateBase("http://psi.hafslund.no/sesam/quant/schema")
               .Prop(s => s.Id, "/id")
               .Prop(s => s.Mpt, "/mpt")
               .Prop(s => s.Msno, "/msno")
               .Prop(s => s.Rty, "/rty")
               .Prop(s => s.Vty, "/vty")
               .Prop(s => s.Un, "/un")
               .Struct<Mv>(s => s.MeterValues, "/values",
                   x => x.Val,
                   x => x.Ts,
                   x => x.Interval,
                   x => x.Cm,
                   x => x.Sts,
                   x => x.Cst);
                
        }
    }
}
