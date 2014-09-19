using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Annotation.Tests.Fluency
{
    [TestClass]
    public class TransformBuilderTests
    {
        [TestMethod]
        public void Constructor_HappyDays_TypeInfoSetAsExpected()
        {
            // Act
            var built = new TransformBuilder<Player>("http://nettriple/Player");

            // Assert
            Assert.AreEqual("http://nettriple/Player", built.TypeString);
            Assert.AreEqual("http://www.w3.org/1999/02/22-rdf-syntax-ns#type", built.TypePredicate);
        }

        [TestMethod]
        public void Prop_WithPropertyPredicateBase_ReturnsExpectedPropertySpecifications()
        {
            // Arrange
            var built = new TransformBuilder<Player>("http://nettriple/Player")
                .WithPropertyPredicateBase("http://nettriple/player")
                .Prop(p => p.Name, "/name")
                .Prop(p => p.Gender, "/gender")
                .Prop(p => p.DateOfBirth, "/dateOfBirth");

            // Act
            var properties = built.PropertySpecifications.ToList();

            // Assert
            Assert.AreEqual(3, properties.Count);
            Assert.IsTrue(properties.Any(p => p.Predicate == "http://nettriple/player/name" && p.Property.Name == "Name"));
        }

        [TestMethod]
        public void Subject_HappyDays_HasExpectedSubject()
        {
            // Arrange
            var built = new TransformBuilder<Player>("http://nettriple/Player")
                .Subject(p => p.Id, "http://nettriple/player/{0}");

            // Act
            var subject = built.SubjectSpecification;

            // Assert
            Assert.AreEqual("Id", subject.Property.Name);
            Assert.AreEqual("http://nettriple/player/{0}", subject.Template);
        }

        [TestMethod]
        public void RelationSpecifications_WithConfiguredRelations_ReturnsExpectedRelationSpecifications()
        {
            // Arrange
            var built = new TransformBuilder<Match>("http://nettriple/Match")
                .Relation(m => m.Player1, "http://nettriple/match/player_1")
                .Relation(m => m.Player2, "http://nettriple/match/player_2");

            // Act
            var relationSpecs = built.RelationSpecifications.ToList();

            // Assert
            Assert.AreEqual(2, relationSpecs.Count);
            Assert.IsTrue(relationSpecs.Any(rs => rs.Predicate == "http://nettriple/match/player_1" && rs.Property.Name == "Player1"));
            Assert.IsTrue(relationSpecs.Any(rs => rs.Predicate == "http://nettriple/match/player_2" && rs.Property.Name == "Player2"));
        }

        [TestMethod]
        public void RelationSpecifications_WithConfiguredRelationsAndWithPropertyPredicateBase_ReturnsExpectedRelationSpecifications()
        {
            // Arrange
            var built = new TransformBuilder<Match>("http://nettriple/Match")
                .WithPropertyPredicateBase("http://nettriple/match")
                .Relation(m => m.Player1, "/player_1")
                .Relation(m => m.Player2, "/player_2");

            // Act
            var relationSpecs = built.RelationSpecifications.ToList();

            // Assert
            Assert.AreEqual(2, relationSpecs.Count);
            Assert.IsTrue(relationSpecs.Any(rs => rs.Predicate == "http://nettriple/match/player_1"));
            Assert.IsTrue(relationSpecs.Any(rs => rs.Predicate == "http://nettriple/match/player_2"));
        }

        [TestMethod]
        public void Struct_ForEnumerable_BuildsExpectedStructureTransform()
        {
            // Arrange
            var built = new TransformBuilder<Sr>("http://psi.hafslund.no/sesam/quant/meterreading-day")
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

            // Act
            var transforms = built.StructureTransforms.ToList();

            // Assert
            Assert.AreEqual(1, transforms.Count);
            var transform = transforms.First();
            Assert.IsTrue(transform.IsEnumerable);
            Assert.AreEqual(6, transform.Elements.Count());
            Assert.AreEqual(0, transform.Elements.Single(e => e.Property.Name == "Val").Index);
            Assert.AreEqual(2, transform.Elements.Single(e => e.Property.Name == "Interval").Index);
            Assert.AreEqual(5, transform.Elements.Single(e => e.Property.Name == "Cst").Index);
            
        }
    }
}
