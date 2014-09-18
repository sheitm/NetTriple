using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Internal;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Annotation.Tests.Internal
{
    internal delegate Object Expr(Player p);

    [TestClass]
    public class ReflectionHelperTests
    {
        [TestMethod]
        public void FindProperty_ForValidLambda_FindsTheCorrectProperty()
        {
            // Arrange
            Expression<Expr> myExpr = p => p.Name;
            
            // Act
            var property = ReflectionHelper.FindProperty(myExpr);

            // Assert
            Assert.AreEqual("Name", property.Name);
        }

        [TestMethod]
        public void GetTypeOfProperty_ForSimpleType_GetsExpectedType()
        {
            // Arrange
            var property = typeof (Tournament).GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);

            // Act
            var type = property.GetTypeOfProperty();

            // Assert
            Assert.AreSame(typeof(string), type);
        }

        [TestMethod]
        public void GetTypeOfProperty_ForGenericListType_GetsExpectedType()
        {
            // Arrange
            var property = typeof(Tournament).GetProperty("Matches", BindingFlags.Instance | BindingFlags.Public);

            // Act
            var type = property.GetTypeOfProperty();

            // Assert
            Assert.AreSame(typeof(Match), type);
        }

        [TestMethod]
        public void GetTypeOfProperty_ForArrayType_GetsExpectedType()
        {
            // Arrange
            var property = typeof(Tournament).GetProperty("Players", BindingFlags.Instance | BindingFlags.Public);

            // Act
            var type = property.GetTypeOfProperty();

            // Assert
            Assert.AreSame(typeof(Player), type);
        }

        [TestMethod]
        public void Deserialize_ForDateTime_GetsExpectedDateTime()
        {
            // Arrange
            var s = "\"1959-02-16T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>";

            // Act
            var d = ReflectionHelper.Deserialize<DateTime>(s);

            // Assert
            Assert.AreEqual(1959, d.Year);
            Assert.AreEqual(2, d.Month);
            Assert.AreEqual(16, d.Day);
        }

        [TestMethod]
        public void Deserialize_DecimalWithComma_GetsExpectedDecimal()
        {
            // Arrange
            var s = "123455,667";

            // Act
            var d = ReflectionHelper.Deserialize<decimal>(s);

            // Assert
            Assert.AreEqual((decimal)123455.667, d);
        }

        [TestMethod]
        public void Deserialize_DecimalWithDot_GetsExpectedDecimal()
        {
            // Arrange
            var s = "123455.667";

            // Act
            var d = ReflectionHelper.Deserialize<decimal>(s);

            // Assert
            Assert.AreEqual((decimal)123455.667, d);
        }
    }
}
