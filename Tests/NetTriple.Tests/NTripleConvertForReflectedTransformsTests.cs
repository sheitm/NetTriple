using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Fluency;

namespace NetTriple.Tests
{
    [TestClass]
    public class NTripleConvertForReflectedTransformsTests
    {
        [TestInitialize]
        public void SetUp()
        {
            LoadAllRdfClasses.Clear();
        }

        [TestMethod]
        public void Xx()
        {
            // Arrange
            var context = new ReflectionBuildContext();
            LoadAllRdfClasses.LoadTransforms(context.Transforms.ToArray());
        }
    }
}
