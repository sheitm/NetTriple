using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Tests.TestDomain;

namespace NetTriple.Tests
{
    [TestClass]
    public class NTripleConvertTests
    {
        [TestInitialize]
        public void SetUp()
        {
            LoadAllRdfClasses.Load();
        }

        [TestMethod]
        public void ToTriples_SingleInstance_ReturnsExpectedTriples()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var expectedSubject = string.Format("<http://netriple.com/unittesting/order/{0}>", id);
            var obj = new Order { Id = id, OrderNumber = 1, Description = "Testing" };

            // Act
            var triples = obj.ToTriples();

            // Assert
            Assert.IsTrue(triples.Count() > 2);
            Assert.IsTrue(triples.Count(t => t.Subject == expectedSubject) > 2);

            var classTriple = triples.Single(t => t.Predicate == "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>");
            Assert.AreEqual("<http://netriple.com/unittesting/Order>", classTriple.Object);

            var orderNumberTriple = triples.Single(t => t.Predicate == "<http://netriple.com/unittesting/order/ordernumber>");
            Assert.AreEqual("1", orderNumberTriple.Object);

            foreach (var triple in triples)
            {
                Console.WriteLine(triple.GetDisplayString());
            }
        }

        [TestMethod]
        public void ToTriples_ObjectGraph_ReturnsExpectedTriples()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var detailId = Guid.NewGuid().ToString();
            var details = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Id = detailId,
                    Description = "Test detail",
                    OrderDetailNumber = 1,
                    ProductCode = "0033",
                    Quantity = 5
                }
            };

            var obj = new Order { Id = orderId, OrderNumber = 1, Description = "Testing", Details = details };

            // Act
            var triples = obj.ToTriples();

            // Assert
            Assert.IsTrue(triples.Count() == 9);
            foreach (var triple in triples)
            {
                Console.WriteLine("{0} {1} {2} .", triple.Subject, triple.Predicate, triple.Object);
            }
        }

        [TestMethod]
        public void ToObject_SingleInstance_ReturnsExpectedInstance()
        {
            // Arrange
            var subject = "<http://netriple.com/unittesting/order/c7519f11-686d-4087-856e-d014e61cfed3>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Order>" },
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/order/ordernumber>", Object = "1" },
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/order/description>", Object = "\"Testing\"" }
            };

            // Act
            var order = triples.ToObject<Order>();

            // Assert
            Assert.IsNotNull(order);
            Assert.AreEqual("c7519f11-686d-4087-856e-d014e61cfed3", order.Id);
            Assert.AreEqual(1, order.OrderNumber);
            Assert.AreEqual("Testing", order.Description);
        }

        //public void ToObject
    }
}
