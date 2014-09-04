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
    public class InflationContextTests
    {
        [TestMethod]
        public void Xxx()
        {
            // Arrange
            var relations = GetDeclaredRelations();
        }

        private IEnumerable<DeclaredRelation> GetDeclaredRelations()
        {
            var type = typeof (BookReview);
            var typeProperty = (RdfTypeAttribute) Attribute.GetCustomAttribute(type, typeof (RdfTypeAttribute));
            var property = type.GetProperty("Book");
            var attribute = (RdfChildrenAttribute) Attribute.GetCustomAttribute(property, typeof (RdfChildrenAttribute));

            return new List<DeclaredRelation>
            {
                new DeclaredRelation(type, typeProperty, attribute, property.PropertyType)
            };

        }
    }
}
