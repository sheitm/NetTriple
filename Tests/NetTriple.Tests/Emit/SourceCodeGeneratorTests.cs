using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Emit;
using NetTriple.Fluency;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class SourceCodeGeneratorTests
    {
        [TestMethod]
        public void GetSourceCode_ForSimpleType_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(OrderDetail));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsTrue(source.Contains("var template = \"<http://netriple.com/unittesting/orderdetail/{0}>\";"));
            Assert.AreEqual(3707, source.Length);
        }

        [TestMethod]
        public void GetSourceCode_ForComplexType_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(Order));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsNotNull(source);
        }

        [TestMethod]
        public void GetSourceCode_ForComplexTypeWithForwardNonListReference_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(Book));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsNotNull(source);
        }

        [TestMethod]
        public void GetSourceCode_ForTypeWithClassDecoration_GetsExpectedCode()
        {
            // Arrange
            var generator = new SourceCodeGenerator(typeof(Measurement));

            // Act
            var source = generator.GetSourceCode();

            // Assert
            Assert.IsTrue(source.Contains("triples.Add(new Triple { Subject = s, Predicate = \"<http://netriple.com/unittesting/measurement/value>\", Object = obj.Value.ToTripleObject() });"));
        }

        [TestMethod]
        public void GetSource_ForFluentlyDeclaredType_GetsExpectedCode()
        {
            // Arrange
            var built = new TransformBuilder<Player>("http://nettriple/Player")
                .Subject(p => p.Id, "http://nettriple/player/{0}")
                .WithPropertyPredicateBase("http://nettriple/player")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Name, "/name")
                .Prop(p => p.Gender, "/gender")
                .Prop(p => p.DateOfBirth, "/dateOfBirth");

            var generator = new SourceCodeGenerator(built);

            // Act
            var source = generator.GetSourceCode();
        }

        [TestMethod]
        public void GetSource_ForFluentlyDeclaredTypeWithRelations_GetsExpectedCode()
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

            var locator = new TransformLocator(new List<IBuiltTransform> { built, built2 });
            built.SetLocator(locator);
            built2.SetLocator(locator);

            var generator = new SourceCodeGenerator(built);

            // Act
            var source = generator.GetSourceCode();
        }

        [TestMethod]
        public void GetSource_ForFluentWithStruct_GetsExpectedCode()
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

            var locator = new TransformLocator(new List<IBuiltTransform> { built });
            built.SetLocator(locator);

            var generator = new SourceCodeGenerator(built);

            // Act
            var source = generator.GetSourceCode();
        }

        [TestMethod]
        public void GetSource_ForFluentWithUnaryStruct_GetsExpectedCode()
        {
            // Arrange
            var built = new TransformBuilder<Match>("match")
                .Subject(m => m.Id, "match/{0}")
                .WithPropertyPredicateBase("xxx")
                .Prop(m => m.Id, "/id")
                .Struct<Player>(m => m.Player1, "/player1", 
                                    p => p.Id,
                                    p => p.Name,
                                    p => p.Gender);

            var locator = new TransformLocator(new List<IBuiltTransform> { built });
            built.SetLocator(locator);

            var generator = new SourceCodeGenerator(built);

            // Act
            var source = generator.GetSourceCode();
        }

        [TestMethod]
        public void Xx()
        {
            // Arrange
            var built = BuildTransform.For<Cat>("http://nettriple/cat")
                .Subject(c => c.Name, "http://nettriple/cat/{0}")
                .WithPropertyPredicateBase("http://nettriple/animal")
                .Prop(c => c.Name, "/name")
                .Relation(c => c.Enemies, "/enemies");

            var built2 = BuildTransform.For<Dog>("http://nettriple/dog")
                .Subject(c => c.Name, "http://nettriple/dog/{0}")
                .WithPropertyPredicateBase("http://nettriple/animal")
                .Prop(c => c.Name, "/name")
                .Relation(c => c.Enemies, "/enemies");

            var locator = new TransformLocator(new List<IBuiltTransform> { built, built2 });
            built.SetLocator(locator);
            built2.SetLocator(locator);

            var generator = new SourceCodeGenerator(built2);

            // Act
            var source = generator.GetSourceCode();
        }
    }
}
