using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Fluency;
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
    }
}
