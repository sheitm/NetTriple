using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests
{
    [TestClass]
    public class LoadAllRdfClassesTests
    {
        [TestMethod]
        public void Find_HappyDays_FindsTypesAsExpected()
        {
            // Arrange
            // Act
            var types = LoadAllRdfClasses.Find();

            // Assert
            Assert.IsTrue(types.Any());
        }

        [TestMethod, TestCategory("Unit")]
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
    }
}
