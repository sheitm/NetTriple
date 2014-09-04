using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetTriple.Tests
{
    [TestClass]
    public class TripleTests
    {
        [TestMethod]
        public void ObjectSetter_WithWhiteSpace_IsSetAsExpected()
        {
            // Act
            var triple = new Triple
            {
                Subject = "<http://netriple.com/unittesting/order/c7519f11-686d-4087-856e-d014e61cfed3>",
                Predicate = "<http://netriple.com/unittesting/order/description>",
                Object = "Testing again"
            };

            // Assert
            Assert.AreEqual("Testing again", triple.Object);
        }
    }
}
