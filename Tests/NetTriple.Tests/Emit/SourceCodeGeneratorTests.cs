using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Fluency;
using NetTriple.Emit;
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
            Assert.AreEqual(3615, source.Length);
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

            LoadAllRdfClasses.LoadTransforms(
                built,
                BuildTransform.For<Player>("http://nettriple/Player")
                .Subject(p => p.Id, "http://nettriple/player/{0}")
                .WithPropertyPredicateBase("http://nettriple/player")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Name, "/name")
                .Prop(p => p.Gender, "/gender")
                .Prop(p => p.DateOfBirth, "/dateOfBirth")
                );

            var generator = new SourceCodeGenerator(built);

            // Act
            var source = generator.GetSourceCode();
        }
    }
}
