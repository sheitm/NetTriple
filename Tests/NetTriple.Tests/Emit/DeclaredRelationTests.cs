using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation;
using NetTriple.Emit;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests.Emit
{
    [TestClass]
    public class DeclaredRelationTests
    {
        [TestMethod]
        public void IsRelationFor_ShouldBe_ReturnsTrue()
        {
            // Arrange
            var type = typeof(BookReview);
            var typeAttribute = (RdfTypeAttribute)Attribute.GetCustomAttribute(type, typeof(RdfTypeAttribute));
            var property = type.GetProperty("Book");
            var attribute = (RdfChildrenAttribute)Attribute.GetCustomAttribute(property, typeof(RdfChildrenAttribute));
            var relation = new DeclaredRelation(type, typeAttribute, attribute, property.PropertyType);

            var triple = new Triple
            {
                Subject = "",
                Predicate = "",
                Object = ""
            };
        }
    }
}
