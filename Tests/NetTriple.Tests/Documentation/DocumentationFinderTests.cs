using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Documentation;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Documentation
{
    [TestClass]
    public class DocumentationFinderTests
    {
        [TestInitialize]
        public void SetUp()
        {
            LoadAllRdfClasses.Clear();

            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Match>("http://nettriple/Match")
                .Subject(p => p.Id, "http://nettriple/match/{0}")
                .WithPropertyPredicateBase("http://nettriple/match")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Date, "/date")
                .Relation(m => m.Player1, "/player_1")
                .Relation(m => m.Player2, "/player_2"),

                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));
        }

        [TestMethod]
        public void GetTypeDocumentation_HappyDays_ReturnsExpectedList()
        {
            // Act
            var docList = DocumentationFinder.GetTypeDocumentation().ToList();

            // Assert
            Assert.AreEqual(4, docList.Count());
        }

        [TestMethod]
        public void GetSampleNTriples_ForEntitiesWithRelations_NTriplesHasReleations()
        {
            // Arrange
            var doc = DocumentationFinder.GetTypeDocumentation().Single(d => d.RdfType == "http://nettriple/Match");

            // Act
            var nTriples = doc.GetSampleNTriples();

            // Assert
            
        }
    }
}
