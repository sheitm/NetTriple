using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Fluency;
using NetTriple.Emit;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class RelationSourceCodeGeneratorTests
    {
        [TestMethod]
        public void GetConversionScript_NoRelations_ReturnsEmptyScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Chapter));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Assert.AreEqual(0, sourceCode.Length);
        }

        [TestMethod]
        public void GetConversionScript_UnaryRelation_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(BookReview));

            // Act
            var sourceCode = generator.GetConversionScript();
        }

        [TestMethod]
        public void GetConversionScript_NonInverseRelations_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Book));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Assert.AreEqual(549, sourceCode.Length);
        }

        [TestMethod]
        public void GetConversionScript_HasRelations_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Order));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Assert.IsTrue(sourceCode.Contains("<http://netriple.com/unittesting/orderdetail/{0}>"));
        }

        [TestMethod]
        public void GetConversionScript_InverseunaryRelation_ReturnsExpectedScript()
        {
            // Arrange
            var generator = new RelationSourceCodeGenerator(typeof(Husband));

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Console.WriteLine(sourceCode);
        }

        [TestMethod]
        public void GetConversionScript_UnaryRelationsDefinedByTransforms_GeneratesExpectedCode()
        {
            // Arrange
            var built = BuildTransform.For<Match>("http://nettriple/Match")
                .Subject(p => p.Id, "http://nettriple/match/{0}")
                .WithPropertyPredicateBase("http://nettriple/match")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Date, "/date")
                .Relation(m => m.Player1, "/player_1")
                .Relation(m => m.Player2, "/player_2");

            var built2 = BuildTransform.For<Player>("http://nettriple/Player")
                .Subject(p => p.Id, "http://nettriple/player/{0}")
                .WithPropertyPredicateBase("http://nettriple/player")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Name, "/name")
                .Prop(p => p.Gender, "/gender")
                .Prop(p => p.DateOfBirth, "/dateOfBirth");

            var locator = new TransformLocator(new List<IBuiltTransform> {built, built2});
            built.SetLocator(locator);
            built2.SetLocator(locator);

            var generator = new RelationSourceCodeGenerator(built);

            // Act
            var sourceCode = generator.GetConversionScript();

            // Assert
            Console.WriteLine(sourceCode);
        }
    }
}
