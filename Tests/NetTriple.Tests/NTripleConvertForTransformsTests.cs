﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Emit;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;
using NodaTime;

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
        public void ToObject_SingleTriplesWithRandomStructure_GetsExpectedObject()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Match>("http://nettriple/Match")
                .Subject(p => p.Id, "http://nettriple/match/{0}")
                .WithPropertyPredicateBase("http://nettriple/match")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Date, "/date"));

            const string matchSubject = "<http://this_is_a_random_subject>";
            const string typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";

            var triples = new List<Triple>
            {
                new Triple{Subject = matchSubject, Predicate = typePredicate, Object = "<http://nettriple/Match>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/id>", Object = "1"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/date>", Object = "\"2014-09-17T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"}
            };

            // Act
            var match = triples.ToObject<Match>();

            // Assert
            Assert.AreEqual(1, match.Id);
        }

        [TestMethod]
        public void ToObject_StructuredTriplesWithRandomStructure_GetsExpectedObject()
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

            const string matchSubject = "<http://this_is_a_random_subject>";
            const string player1Subject = "<http://plyr1>";
            const string player2Subject = "<http://aCompletelyDifferentSubject>";
            const string typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";

            var triples = new List<Triple>
            {
                new Triple{Subject = matchSubject, Predicate = typePredicate, Object = "<http://nettriple/Match>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/id>", Object = "1"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/date>", Object = "\"2014-09-17T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/player_1>", Object = player1Subject},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/player_2>", Object = player2Subject},

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
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/id>" && t.Object == "\"997766\"^^<http://www.w3.org/2001/XMLSchema#string>"));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/gender>" && t.Object == "\"Male\""));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/dateOfBirth>" && t.Object == "\"1955-12-03T23:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"));

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToTriples_DefinedByTransformWithArray_CreatesExpectedTriples()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Tournament>("http://nettriple/Tournament")
                    .Subject(t => t.Id, "http://nettriple/tournament/{0}")
                    .Relation(t => t.Players, "http://nettriple/player/tournament_participation", true)
                    .WithPropertyPredicateBase("http://nettriple/tournament")
                    .Prop(t => t.Id, "/id")
                    .Prop(t => t.Name, "/name"),
                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var tournament = new Tournament
            {
                Id = 5,
                Name = "Wimbledon",
                Players = new Player[]
                {
                    new Player
                    {
                        Id = "997766",
                        Name = "Bjørn Borg",
                        Gender = Gender.Male,
                        DateOfBirth = new DateTime(1955, 12, 4)
                    },
                    new Player
                    {
                        Id = "1001",
                        Name = "John McEnroe",
                        Gender = Gender.Male,
                        DateOfBirth = new DateTime(1959, 2, 16)
                    }
                }
            };

            // Act
            var triples = tournament.ToTriples();

            // Assert
            Assert.AreEqual(2, triples.Count(t => 
                t.Predicate == "<http://nettriple/player/tournament_participation>"
                && t.Object == "<http://nettriple/tournament/5>"));
        }

        [TestMethod]
        public void ToObject_WithArray_ArrayFilledAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Tournament>("http://nettriple/Tournament")
                    .Subject(t => t.Id, "http://nettriple/tournament/{0}")
                    .Relation(t => t.Players, "http://nettriple/player/tournament_participation", true)
                    .WithPropertyPredicateBase("http://nettriple/tournament")
                    .Prop(t => t.Id, "/id")
                    .Prop(t => t.Name, "/name"),
                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var triples = new List<Triple>
            {
                new Triple{Subject = "<http://nettriple/tournament/5>", Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriple/Tournament>"},
                new Triple{Subject = "<http://nettriple/tournament/5>", Predicate = "<http://nettriple/tournament/id>", Object = "5"},

                new Triple{Subject = "<http://nettriple/player/997766>", Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriple/Player>"},
                new Triple{Subject = "<http://nettriple/player/997766>", Predicate = "<http://nettriple/player/id>", Object = "\"997766\""},
                new Triple{Subject = "<http://nettriple/player/997766>", Predicate = "<http://nettriple/player/tournament_participation>", Object = "<http://nettriple/tournament/5>"},

                new Triple{Subject = "<http://nettriple/player/1001>", Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriple/Player>"},
                new Triple{Subject = "<http://nettriple/player/1001>", Predicate = "<http://nettriple/player/id>", Object = "\"1001\""},
                new Triple{Subject = "<http://nettriple/player/1001>", Predicate = "<http://nettriple/player/tournament_participation>", Object = "<http://nettriple/tournament/5>"}
            };

            // Act
            var tournament = triples.ToObject<Tournament>();

            // Assert
            Assert.AreEqual(2, tournament.Players.Length);
            Assert.IsTrue(tournament.Players.Any(p => p.Id == "1001"));
            Assert.IsTrue(tournament.Players.Any(p => p.Id == "997766"));
        }

        [TestMethod]
        public void ToTriples_WithStruct_GeneratesExpectedTriples()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Sr>("http://psi.hafslund.no/sesam/quant/meterreading-day")
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
                   x => x.Cst));

            var series = new Sr
            {
                Mpt = "707057500032064897",
                Msno = "510000636",
                Rty = "REL",
                Vty = "ECR",
                Un = "kWh",
                MeterValues = new List<Mv>
                {
                    new Mv{ Val = (decimal)13.348, Ts = new DateTime(2014, 6, 14, 0, 0, 0), Interval = 60, Cm = "GE", Sts = "OK", Cst = "OK"},
                    new Mv{ Val = (decimal)10.358, Ts = new DateTime(2014, 6, 14, 1, 0, 0), Interval = 60, Cm = "GE", Sts = "OK", Cst = "OK"},
                }
            };

            // Act
            var triples = series.ToTriples();

            // Assert
            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToObject_WithStruct_GeneratesExpectedObject()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Sr>("http://psi.hafslund.no/sesam/quant/meterreading-day")
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
                   x => x.Cst));

            var subject =
                "<http://psi.hafslund.no/sesam/quant/meterreading-day/707057500032064897_REL_ECR_kWh_2014-06-14>";
            var triples = new List<Triple>
            {
                new Triple{Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://psi.hafslund.no/sesam/quant/meterreading-day>"},
                new Triple{Subject = subject, Predicate = "<http://psi.hafslund.no/sesam/quant/schema/mpt>", Object = "\"707057500032064897\""},
                new Triple{Subject = subject, Predicate = "<http://psi.hafslund.no/sesam/quant/schema/msno>", Object = "\"510000636\""},
                new Triple{Subject = subject, Predicate = "<http://psi.hafslund.no/sesam/quant/schema/rty>", Object = "\"REL\""},
                new Triple{Subject = subject, Predicate = "<http://psi.hafslund.no/sesam/quant/schema/vty>", Object = "\"ECR\""},
                new Triple{Subject = subject, Predicate = "<http://psi.hafslund.no/sesam/quant/schema/un>", Object = "\"kWh\""},
                new Triple{Subject = subject, Predicate = "<http://psi.hafslund.no/sesam/quant/schema/values>", Object = "\"13,348;;14.06.2014 00:00:00;;60;;GE;;OK;;OK##10,358;;14.06.2014 01:00:00;;60;;GE;;OK;;OK##10,358;;14.06.2014 02:00:00;;60;;GE;;OK;;OK##\""},
            };

            // Act
            var series = triples.ToObject<Sr>();

            // Assert
            Assert.AreEqual(3, series.MeterValues.Count());
        }

        [TestMethod]
        public void ToTriples_WithNodaTimeInstant_GeneratesExpectedTriples()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<PointInTime>("http://nettriples/point_in_time")
                    .Subject(s => s.Id, "http://nettriples/point_in_time/{0}")
                    .WithPropertyPredicateBase("http://nettriples/point_in_time/schema")
                    .Prop(s => s.Id, "/id")
                    .Prop(s => s.Instant, "/instant"));
            var pointInTime = new PointInTime
            {
                Id = "1",
                Instant = Instant.FromDateTimeUtc(DateTime.Parse("2014-09-29T15:16:00Z").ToUniversalTime()) 
            };

            // Act
            var triples = pointInTime.ToTriples().ToList();

            // Assert
            Assert.AreEqual(3, triples.Count());
            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://nettriples/point_in_time/1>"
                && t.Predicate == "<http://nettriples/point_in_time/schema/instant>"
                && t.Object == "\"2014-09-29T15:16:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"));
        }

        [TestMethod]
        public void ToTriples_WithNodaInstantInStruct_ReturnsExpectedTriples()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<TimeSeries>("http://nettriples/time-series")
                   .Subject(ts => ts.Id, "http://nettriples/time-series/{0}")
                   .WithPropertyPredicateBase("http://nettriples/time-series/schema")
                   .Prop(ts => ts.Id, "/id")
                   .Prop(ts => ts.Name, "/name")
                   .Struct<PointInTime>(ts => ts.Points, "/points",
                        p => p.Id,
                        p => p.Instant)
                );

            var timeSeries = new TimeSeries
            {
                Id = "YY-12",
                Name = "Gpx track",
                Points = new List<PointInTime>
                {
                    new PointInTime{ Id = "1", Instant = Instant.FromDateTimeUtc(DateTime.Parse("2014-09-29T15:16:00Z").ToUniversalTime()) },
                    new PointInTime{ Id = "2", Instant = Instant.FromDateTimeUtc(DateTime.Parse("2014-09-29T16:16:00Z").ToUniversalTime()) }
                }
            };

            // Act
            var triples = timeSeries.ToTriples();

            // Assert
            Assert.AreEqual(4, triples.Count());
            var st = triples.Single(t => t.Predicate == "<http://nettriples/time-series/schema/points>");
            Assert.AreEqual("\"1;;2014-09-29T15:16:00Z##2;;2014-09-29T16:16:00Z##\"^^<http://www.w3.org/2001/XMLSchema#string>", st.Object);
            //Assert.IsTrue(triples.Any(t => t.Object == "1;;2014-09-29T15:16:00Z##2;;2014-09-29T16:16:00Z##"));
        }

        [TestMethod]
        public void ToObject_WithNodaInstantInStruct_ReturnsExpectedObject()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<TimeSeries>("http://nettriples/time-series")
                   .Subject(ts => ts.Id, "http://nettriples/time-series/{0}")
                   .WithPropertyPredicateBase("http://nettriples/time-series/schema")
                   .Prop(ts => ts.Id, "/id")
                   .Prop(ts => ts.Name, "/name")
                   .Struct<PointInTime>(ts => ts.Points, "/points",
                        p => p.Id,
                        p => p.Instant)
                );

            var subject = "<http://nettriples/time-series/YY-12>";
            var triples = new List<Triple>
            {
                new Triple { Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriples/time-series>" },
                new Triple { Subject = subject, Predicate = "<http://nettriples/time-series/schema/id>", Object = "\"YY-12\"" },
                new Triple { Subject = subject, Predicate = "<http://nettriples/time-series/schema/name>", Object = "\"Gpx track\""},
                new Triple { Subject = subject, Predicate = "<http://nettriples/time-series/schema/points>", Object = "\"1;;2014-09-29T15:16:00Z##2;;2014-09-29T16:16:00Z##\""}
            };

            // Act
            var series = triples.ToObject<TimeSeries>();

            // Assert
            Assert.AreEqual("YY-12", series.Id);
            Assert.AreEqual("Gpx track", series.Name);
            Assert.AreEqual(2, series.Points.Count());
            Assert.IsTrue(series.Points.Any(p => p.Id == "1" && p.Instant.ToString() == "2014-09-29T15:16:00Z"));
            Assert.IsTrue(series.Points.Any(p => p.Id == "2" && p.Instant.ToString() == "2014-09-29T16:16:00Z"));
        }

        [TestMethod]
        public void ToObject_WithNodaInstant_ReturnsExpectedInstance()
        {
            // Arrange
             LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<PointInTime>("http://nettriples/point_in_time")
                    .Subject(s => s.Id, "http://nettriples/point_in_time/{0}")
                    .WithPropertyPredicateBase("http://nettriples/point_in_time/schema")
                    .Prop(s => s.Id, "/id")
                    .Prop(s => s.Instant, "/instant"));

            var subject = "<http://nettriples/point_in_time/1>";
            var triples = new List<Triple>
            {
                new Triple { Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriples/point_in_time>" },
                new Triple { Subject = subject, Predicate = "<http://nettriples/point_in_time/schema/id>", Object = "\"1\"" },
                new Triple { Subject = subject, Predicate = "<http://nettriples/point_in_time/schema/instant>", Object = "\"2014-09-29T15:16:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>" }
            };

            // Act
            var pointInTime = triples.ToObject<PointInTime>();

            // Assert
            Assert.AreEqual("1", pointInTime.Id);
            Assert.AreEqual("2014-09-29T15:16:00Z", pointInTime.Instant.ToString());
        }

        [TestMethod]
        public void ToObject_SpecialCharsInString_CharactersAsExpected()
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

            const string playerSubject = "<http://nettriple/player/909>";
            const string typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var triples = new List<Triple>
            {
                new Triple{Subject = playerSubject, Predicate = typePredicate, Object = "<http://nettriple/Player>"},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/id>", Object = "\"909\""},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/name>", Object = "\"Bj\u00F8rn Borg\""},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1956-06-06T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
            };

            // Act
            var player = triples.ToObject<Player>();

            // Assert
            Assert.AreEqual("Bjørn Borg", player.Name);
        }

        [TestMethod]
        public void ToObject_RdfTypeInfo_RdfTypeInfoRemoved()
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

            const string playerSubject = "<http://nettriple/player/909>";
            const string typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var triples = new List<Triple>
            {
                new Triple{Subject = playerSubject, Predicate = typePredicate, Object = "<http://nettriple/Player>"},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/id>", Object = "\"909\"^^<http://example.org/datatype1>"},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/name>", Object = "\"Bj\u00F8rn Borg\""},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple{Subject = playerSubject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1956-06-06T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
            };

            // Act
            var player = triples.ToObject<Player>();

            // Assert
            Assert.AreEqual("909", player.Id);

            Console.WriteLine(LoadAllRdfClasses.WhatHaveIGot());
        }

        [TestMethod]
        public void ToTriples_SomePropertiesAreNull_TripleNotGeneratedForNullProperties()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Meter2>("http://psi.hafslund.no/sesam/quant/meter")
                    .Subject(m => m.NetOwnerAssetId, "http://psi.hafslund.no/sesam/quant/meter/{0}")
                    .WithPropertyPredicateBase("http://psi.hafslund.no/sesam/quant/schema")
                    .Prop(m => m.ManufacturerSerialNumber, "/manufacturerSerialNumber")
                    .Prop(m => m.NetOwnerAssetId, "/netOwnerAssetId")
                    .Prop(m => m.Label, "/label")
                    .Prop(m => m.AssetModelName, "/assetModelName")
                    .Prop(m => m.AssetState, "/assetState")
                    .Prop(m => m.AssetLocation, "/assetLocation")
                    .Prop(m => m.ManufacturingYear, "/manufacturingYear")
                    .Prop(m => m.Firmware, "/firmware")
                    .Prop(m => m.MeterInterval, "/meterInterval")
                    .Prop(m => m.ServiceIntervalEntity, "/serviceIntervalEntity")
                    .Prop(m => m.ServiceIntervalValue, "/serviceIntervalValue")
                );

            var meter = new Meter2
            {
                ManufacturerSerialNumber = "419900019",
                NetOwnerAssetId = "0000026784",
                AssetModelName = "Maalermodell",
                AssetState = "WAREHOUSE",
                AssetLocation = "Hovedlager",
                ServiceIntervalEntity = 1,
                ServiceIntervalValue = 1,
                ManufacturingYear = 2013,
                Firmware = "MET7005 2.4.38",
                MeterInterval = 60
            };

            // Act
            var triples = meter.ToTriples();

            // Assert
            Assert.AreEqual(11, triples.Count());
            Assert.IsFalse(triples.Any(t => t.Predicate == "<http://psi.hafslund.no/sesam/quant/schema/label>"));
        }

        [TestMethod]
        public void ToTriples_WithBoolean_BooleSerializedCorrectly()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<AddressableDevice>("http://nettriples/adressabledevice")
                    .Subject(m => m.Id, "http://nettriples/adressabledevice/{0}")
                    .WithPropertyPredicateBase("http://nettriples/adressabledevice/schema")
                    .Prop(m => m.Id, "/id")
                    .Prop(m => m.WillAcknowledge, "/willAcknowledge")
                );

            var dev = new AddressableDevice
            {
                Id = 6,
                WillAcknowledge = true
            };

            // Act
            var triples = dev.ToTriples().ToList();

            // Assert
            var trpl = triples.Single(t => t.Predicate == "<http://nettriples/adressabledevice/schema/willAcknowledge>");
            Assert.AreEqual("\"true\"^^<http://www.w3.org/2001/XMLSchema#boolean>", trpl.Object);

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToObjects_ReferenceViaAbstractBaseClass_AreLinkedCorrectly()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Cat>("http://nettriples/cat")
                    .Subject(m => m.Name, "http://nettriples/cat/{0}")
                    .WithPropertyPredicateBase("http://nettriples/animal/schema")
                    .Prop(m => m.Name, "/name")
                    .Relation(m => m.Enemies, "/enemies"),
                BuildTransform.For<Dog>("http://nettriples/dog")
                    .Subject(m => m.Name, "http://nettriples/dog/{0}")
                    .WithPropertyPredicateBase("http://nettriples/animal/schema")
                    .Prop(m => m.Name, "/name")
                    .Relation(m => m.Enemies, "/enemies")
                );

            var catSubject = "<http://nettriples/cat/Kitty>";
            var dogSubject = "<http://nettriples/dog/Fido>";
            var triples = new List<Triple>
            {
                new Triple{Subject = catSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriples/cat>"},
                new Triple{Subject = catSubject, Predicate = "<http://nettriples/animal/schema/name>", Object = "\"Kitty\""},
                new Triple{Subject = catSubject, Predicate = "<http://nettriples/animal/schema/enemies>", Object = dogSubject},
                new Triple{Subject = dogSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriples/dog>"},
                new Triple{Subject = dogSubject, Predicate = "<http://nettriples/animal/schema/name>", Object = "\"Fido\""},
                new Triple{Subject = dogSubject, Predicate = "<http://nettriples/animal/schema/enemies>", Object = catSubject}
            };

            // Act
            var objs = triples.ToObjects();

            // Assert
            var cat = (Cat)objs.Single(o => o is Cat);
            var dog = (Dog)objs.Single(o => o is Dog);
            Assert.AreSame(cat, dog.Enemies.First());
            Assert.AreSame(dog, cat.Enemies.First());
        }

        [TestMethod]
        public void ToObject_WithQuasiNullables_SpecifiedPropertiesSetCorrectly()
        {
            //Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<WithQuasiNullables>("http://nettriples/withQuasiNullables")
                   .Subject(m => m.Id, "http://nettriples/withQuasiNullables/{0}")
                   .WithPropertyPredicateBase("http://nettriples/withQuasiNullables/schema")
                   .Prop(m => m.No, "/no", m => m.NoSpecified)
                   .Prop(m => m.Hour, "/hour", m => m.HourSpecified)
                   .Prop(m => m.Ok, "/ok", m => m.OkSpecified)
                );

            var subj = "http://nettriples/withQuasiNullables/1";

            var triples = new List<Triple>
            {
                new Triple{ Subject = subj, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriples/withQuasiNullables>"},
                new Triple{Subject = subj, Predicate = "<http://nettriples/withQuasiNullables/schema/id>", Object = "1"}, 
                new Triple{Subject = subj, Predicate = "<http://nettriples/withQuasiNullables/schema/no>", Object = "0"}
            };

            // Act
            var obj = triples.ToObject<WithQuasiNullables>();

            // Assert
            Assert.IsTrue(obj.NoSpecified);
            Assert.IsFalse(obj.OkSpecified);
            Assert.IsFalse(obj.HourSpecified);
        }

        [TestMethod]
        public void ToTriples_ReferenceViaAbstractBaseClass_AreLinkedCorrectly()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Cat>("http://nettriples/cat")
                    .Subject(m => m.Name, "http://nettriples/cat/{0}")
                    .WithPropertyPredicateBase("http://nettriples/animal/schema")
                    .Prop(m => m.Name, "/name")
                    .Relation(m => m.Enemies, "/enemies"),
                BuildTransform.For<Dog>("http://nettriples/dog")
                    .Subject(m => m.Name, "http://nettriples/dog/{0}")
                    .WithPropertyPredicateBase("http://nettriples/animal/schema")
                    .Prop(m => m.Name, "/name")
                    .Relation(m => m.Enemies, "/enemies")
                );

            var dog = new Dog {Name = "Fido"};
            var cat = new Cat {Name = "Kitty"};
            dog.Enemies = new List<AnimalBase> {cat};
            cat.Enemies = new List<AnimalBase> {dog};

            // Act
            var triples = cat.ToTriples().ToList();

            // Assert
            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://nettriples/dog/Fido>" &&
                t.Predicate == "<http://nettriples/animal/schema/enemies>" &&
                t.Object == "<http://nettriples/cat/Kitty>"));

            Assert.IsTrue(triples.Any(t =>
               t.Subject == "<http://nettriples/cat/Kitty>" &&
               t.Predicate == "<http://nettriples/animal/schema/enemies>" &&
               t.Object == "<http://nettriples/dog/Fido>"));
        }

        [TestMethod]
        public void ToObject_WithSerializedBools_BooleanValuesAreSetCorrectly()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<AddressableDevice>("http://nettriples/adressabledevice")
                    .Subject(m => m.Id, "http://nettriples/adressabledevice/{0}")
                    .WithPropertyPredicateBase("http://nettriples/adressabledevice/schema")
                    .Prop(m => m.Id, "/id")
                    .Prop(m => m.WillAcknowledge, "/willAcknowledge")
                );

            var typePred = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var typeObject = "<http://nettriples/adressabledevice>";
            var s1 = "<http://nettriples/adressabledevice/1>";
            var triples1 = new List<Triple>
            {
                new Triple{ Subject = s1, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = s1, Predicate = "<http://nettriples/adressabledevice/schema/id>", Object = "1" },
                new Triple{ Subject = s1, Predicate = "<http://nettriples/adressabledevice/schema/willAcknowledge>", Object = "true" },
            };

            var s2 = "<http://nettriples/adressabledevice/2>";
            var triples2 = new List<Triple>
            {
                new Triple{ Subject = s2, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = s2, Predicate = "<http://nettriples/adressabledevice/schema/id>", Object = "2" },
                new Triple{ Subject = s2, Predicate = "<http://nettriples/adressabledevice/schema/willAcknowledge>", Object = "1" },
            };

            var s3 = "<http://nettriples/adressabledevice/3>";
            var triples3 = new List<Triple>
            {
                new Triple{ Subject = s3, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = s3, Predicate = "<http://nettriples/adressabledevice/schema/id>", Object = "3" },
                new Triple{ Subject = s3, Predicate = "<http://nettriples/adressabledevice/schema/willAcknowledge>", Object = "\"true\"^^xsd:boolean" },
            };

            // Act
            var o1 = triples1.ToObject<AddressableDevice>();
            var o2 = triples2.ToObject<AddressableDevice>();
            var o3 = triples3.ToObject<AddressableDevice>();

            // Assert
            Assert.IsTrue(o1.WillAcknowledge);
            Assert.IsTrue(o2.WillAcknowledge);
            Assert.IsTrue(o3.WillAcknowledge);
        }

        [TestMethod]
        public void ToObject_ForObjectWithValueConverter_SetValuesAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Book>("http://netriple.com/unittesting/Book")
                    .Subject(m => m.Isbn, "http://netriple.com/unittesting/book/{0}")
                    .WithPropertyPredicateBase("http://netriple.com/unittesting/book")
                    .Prop(b => b.Isbn, "/isbn")
                    .Prop(b => b.YearOfPublication, "/year",
                        valueConverter: s => int.Parse(string.Format("20{0}", s.Substring(4, 2))))
                );

            const string typePred = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            const string typeObject = "<http://netriple.com/unittesting/Book>";
            const string subj = "<http://netriple.com/unittesting/book/990088007711>";

            var triples = new List<Triple>
            {
                new Triple{ Subject = subj, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/unittesting/book/isbn>", Object = "990088007711" },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/unittesting/book/year>", Object = "010113" },
            };

            // Act
            var book = triples.ToObject<Book>();

            // Assert
            Assert.AreEqual(2013, book.YearOfPublication);
        }

        [TestMethod]
        public void ToObject_WithUnknownRdfTypes_NoException()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Book>("http://netriple.com/unittesting/Book")
                    .Subject(m => m.Isbn, "http://netriple.com/unittesting/book/{0}")
                    .WithPropertyPredicateBase("http://netriple.com/unittesting/book")
                    .Prop(b => b.Isbn, "/isbn")
                    .Prop(b => b.YearOfPublication, "/year",
                        valueConverter: s => int.Parse(string.Format("20{0}", s.Substring(4, 2))))
                );

            const string typePred = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            const string typeObject = "<http://netriple.com/unittesting/Book>";
            const string subj = "<http://netriple.com/unittesting/book/990088007711>";

            const string unknownTypeObject = "<http://netriple.com/unittesting/UnknownEntity>";
            const string unknownSubj = "<http://netriple.com/unittesting/unknown/1>";

            var triples = new List<Triple>
            {
                new Triple{ Subject = subj, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/unittesting/book/isbn>", Object = "990088007711" },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/unittesting/book/year>", Object = "010113" },
                new Triple{ Subject = unknownSubj, Predicate = typePred, Object = unknownTypeObject }
            };

            // Act
            var book = triples.ToObject<Book>();

            // Assert
            Assert.IsNotNull(book);
        }

        [TestMethod]
        public void ToObject_WithUnknownRdfTypes_UnknownsListedCorrectly()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Book>("http://netriple.com/unittesting/Book")
                    .Subject(m => m.Isbn, "http://netriple.com/unittesting/book/{0}")
                    .WithPropertyPredicateBase("http://netriple.com/unittesting/book")
                    .Prop(b => b.Isbn, "/isbn")
                    .Prop(b => b.YearOfPublication, "/year",
                        valueConverter: s => int.Parse(string.Format("20{0}", s.Substring(4, 2))))
                );

            const string typePred = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            const string typeObject = "<http://netriple.com/unittesting/Book>";
            const string subj = "<http://netriple.com/unittesting/book/990088007711>";

            const string unknownTypeObject = "<http://netriple.com/unittesting/UnknownEntity>";
            const string unknownSubj = "<http://netriple.com/unittesting/unknown/1>";
            const string unknownSubj2 = "<http://netriple.com/unittesting/unknown/2>";

            var triples = new List<Triple>
            {
                new Triple{ Subject = subj, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/unittesting/book/isbn>", Object = "990088007711" },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/unittesting/book/year>", Object = "010113" },
                new Triple{ Subject = unknownSubj, Predicate = typePred, Object = unknownTypeObject },
                new Triple{ Subject = unknownSubj2, Predicate = typePred, Object = unknownTypeObject },
                new Triple{ Subject = unknownSubj2, Predicate = "<http://netriple.com/unittesting/unknown/age>", Object = "2" },
                new Triple{ Subject = "<http://netriple.com/unittesting/unknowntype2/abc>", Predicate = typePred, Object = "<http://netriple.com/unittesting/UnknownEntityType2>" },
            };

            // Act
            var unknowns = new List<string>();
            var book = triples.ToObject<Book>(unknowns);

            // Assert
            Assert.IsNotNull(book);
            Assert.AreEqual(3, unknowns.Count);
            Assert.IsTrue(unknowns.Contains("<http://netriple.com/unittesting/unknown/1> <http://psi.hafslund.no/sesam/orphaned-entity> <http://netriple.com/unittesting/UnknownEntity>"));
            Assert.IsTrue(unknowns.Contains("<http://netriple.com/unittesting/unknown/2> <http://psi.hafslund.no/sesam/orphaned-entity> <http://netriple.com/unittesting/UnknownEntity>"));
            Assert.IsTrue(unknowns.Contains("<http://netriple.com/unittesting/unknowntype2/abc> <http://psi.hafslund.no/sesam/orphaned-entity> <http://netriple.com/unittesting/UnknownEntityType2>"));
        }

        [TestMethod]
        public void ToObject_WithShort_ShortSetAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<EntityWithShort>("http://netriple.com/short")
                    .Subject(m => m.Id, "http://netriple.com/unittesting/short/{0}")
                    .WithPropertyPredicateBase("http://netriple.com/short")
                    .Prop(b => b.Id, "/id")
                    .Prop(b => b.MyShort, "/myshort")
                );

            var subj = "ss";
            short s = 2;
            const string typePred = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            const string typeObject = "<http://netriple.com/short>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = subj, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/short/id>", Object = "1" },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/short/myshort>", Object = s.ToString() },
            };

            // Act
            var ms = triples.ToObject<EntityWithShort>();

            // Assert
            Assert.AreEqual(s, ms.MyShort);
        }

        [TestMethod]
        public void ToObject_WithRdfClassQualifiers_GetsExpectedObject()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Payment>("http://netriple.com/payment")
                    .Subject(m => m.Id, "http://netriple.com/unittesting/payment/{0}")
                    .WithPropertyPredicateBase("http://netriple.com/payment")
                    .Prop(p => p.Amount, "/amount")
                    .Prop(p => p.Amount2, "/amount2")
                    .Prop(p => p.Count, "/count")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.To, "/to")
                );

            var subj = "<http://netriple.com/unittesting/payment/1>";
            const string typePred = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            const string typeObject = "<http://netriple.com/payment>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = subj, Predicate = typePred, Object = typeObject },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/payment/id>", Object = "1" },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/payment/to>", Object = "\"Petter\"^^<http://www.w3.org/2001/XMLSchema#string>" },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/payment/amount>", Object = "\"230\"^^<http://www.w3.org/2001/XMLSchema#decimal>"  },
                new Triple{ Subject = subj, Predicate = "<http://netriple.com/payment/amount2>", Object = "42^^<http://www.w3.org/2001/XMLSchema#decimal>"  }
            };

            // Act
            var payment = triples.ToObject<Payment>();
        }
    }
}
