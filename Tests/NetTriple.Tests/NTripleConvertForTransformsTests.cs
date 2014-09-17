using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests
{
    [TestClass]
    public class NTripleConvertForTransformsTests
    {
        [TestInitialize]
        public void SetUp()
        {
            LoadAllRdfClasses.Clear();
        }

        [TestMethod]
        public void ToObject_UnaryRelationsDefinedByTransform_GetsExpectedObjectGraph()
        {
            // Arrange
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

            var matchSubject = "<http://nettriple/match/1>";
            var player1Subject = "<http://nettriple/player/909>";
            var player2Subject = "<http://nettriple/player/1001>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";

            var triples = new List<Triple>
            {
                new Triple{Subject = matchSubject, Predicate = typePredicate, Object = "<http://nettriple/Match>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/id>", Object = "1"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/date>", Object = "\"2014-09-17T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/player_1>", Object = "<http://nettriple/player/909>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/player_2>", Object = "<http://nettriple/player/1001>"},

                new Triple{Subject = player1Subject, Predicate = typePredicate, Object = "<http://nettriple/Player>"},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/id>", Object = "\"909\""},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/name>", Object = "\"Bjørn Borg\""},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1956-06-06T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},

                new Triple{Subject = player2Subject, Predicate = typePredicate, Object = "<http://nettriple/Player>"},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/id>", Object = "1001"},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/name>", Object = "\"John McEnroe\""},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1959-02-16T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
            };

            // Act
            var match = triples.ToObject<Match>();

            // Assert
            Assert.AreEqual(1, match.Id);
            Assert.AreEqual("909", match.Player1.Id);
            Assert.AreEqual("1001", match.Player2.Id);
        }

        [TestMethod]
        public void ToTriples_UnaryRelationsDefinedByTransform_CreatesRelationTriples()
        {
            // Arrange
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

            var match = new Match
            {
                Id = 1,
                Date = new DateTime(2014, 9, 17),
                Player1 = new Player
                {
                    Id = "909",
                    Name = "Bjørn Borg",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1956, 6, 6)
                },
                Player2 = new Player
                {
                    Id = "1001",
                    Name = "John McEnroe",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1959, 2, 16)
                }
            };

            // Act
            var triples = match.ToTriples();

            // Assert
            Assert.IsTrue(triples.Count() > 3);
            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://nettriple/match/1>"
                && t.Predicate == "<http://nettriple/match/player_1>"
                && t.Object == "<http://nettriple/player/909>"));

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToObject_DefinedByTransform_InflatesObjectAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var subject = "<http://nettriple/player/997766>";
            var triples = new List<Triple>
            {
                new Triple {Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriple/Player>"},
                new Triple {Subject = subject, Predicate = "<http://nettriple/player/id>", Object = "\"997766\""},
                new Triple {Subject = subject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple {Subject = subject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1955-12-13T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
            };

            // Act
            var player = triples.ToObject<Player>();

            // Assert
            Assert.AreEqual("997766", player.Id);
            Assert.AreEqual(Gender.Male, player.Gender);
            Assert.AreEqual(1955, player.DateOfBirth.Year);
            Assert.AreEqual(13, player.DateOfBirth.Day);
            Assert.AreEqual(12, player.DateOfBirth.Month);
        }

        [TestMethod]
        public void ToTriples_DefinedByTransform_SubjectClassAndPropertiesAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var player = new Player
            {
                Id = "997766",
                Name = "Bjørn Borg",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1955, 12, 4)
            };

            // Act
            var triples = player.ToTriples().ToList();

            // Assert
            var sub = "<http://nettriple/player/997766>";
            Assert.IsTrue(triples.All(t => t.Subject == sub));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" && t.Object == "<http://nettriple/Player>"));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/id>" && t.Object == "\"997766\""));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/gender>" && t.Object == "Male"));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/dateOfBirth>" && t.Object == "\"1955-12-04T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"));

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }
    }
}
