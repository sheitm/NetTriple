using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Internal;
using NetTriple.Annotation.Tests.TestDomain;

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
    }
}
