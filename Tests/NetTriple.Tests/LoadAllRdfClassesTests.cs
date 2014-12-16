using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests
{
    [TestClass]
    public class LoadAllRdfClassesTests
    {
        [TestInitialize]
        public void SetUp()
        {
            LoadAllRdfClasses.Clear();
        }

        [TestMethod]
        public void Find_HappyDays_FindsTypesAsExpected()
        {
            // Arrange
            // Act
            var types = LoadAllRdfClasses.Find();

            // Assert
            Assert.IsTrue(types.Any());
        }

        [TestMethod]
        public void GetConverter_ClassIsNotAnnotated_ReturnsNull()
        {
            // Arrange
            LoadAllRdfClasses.Load();

            // Act
            var converter = LoadAllRdfClasses.GetConverter(typeof(NotAnnotated));

            // Assert
            Assert.IsNull(converter);
        }

        [TestMethod]
        public void GetRelations_AfterLoad_HasRelastions()
        {
            // Arrange
            LoadAllRdfClasses.Load();

            // Act
            var relations = LoadAllRdfClasses.GetRelations();

            // Assert
            Assert.IsTrue(relations.Any());
        }

        [TestMethod]
        public void Load_HappyDays_LoadsAsExpected()
        {
            // Arrange
            //Act
            LoadAllRdfClasses.Load();

            // Assert
            Assert.IsTrue(LoadAllRdfClasses.HasConverterFor(typeof(Order)));
        }

        [TestMethod]
        public void GetConverterBySubject_ValidSubject_GetsConverter()
        {
            // Arrange
            var subject = "http://netriple.com/unittesting/order/112233";
        }

        [TestMethod]
        public void GetTypeForSubject_Exists_ReturnsExpectedClass()
        {
            // Arrange
            var subject = "http://netriple.com/unittesting/book_review/324234";
            LoadAllRdfClasses.Load();

            // Act
            var type = LoadAllRdfClasses.GetTypeForSubject(subject);

            // Assert
            Assert.AreSame(typeof(BookReview), type);
        }

        [TestMethod]
        public void WhatHaveIGot_WithLoadedConvertes_ReturnsExpectedReport()
        {
            // Arrange
            LoadAllRdfClasses.Load();

            // Act
            var report = LoadAllRdfClasses.WhatHaveIGot();

            // Assert
            Console.WriteLine(report);
        }

        [TestMethod]
        public void LoadFromAssemblyOf_AfterGlobalLoad_NoCrash()
        {
            // Arrange
            LoadAllRdfClasses.Load();

            // Act
            LoadAllRdfClasses.LoadFromAssemblyOf<Book>();
        }

        [TestMethod]
        public void LoadFromAssemblyOf_Twice_NoCrash()
        {
            // Act
            LoadAllRdfClasses.LoadFromAssemblyOf<Book>();
            LoadAllRdfClasses.LoadFromAssemblyOf<Book>();
        }

        [TestMethod]
        public void GetTypeForTriples_WithLoadedClasses_GetsExpectedType()
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
            var type = LoadAllRdfClasses.GetTypeForTriples(triples);

            // Assert
            Assert.AreSame(typeof(Match), type);

        }
    }
}
