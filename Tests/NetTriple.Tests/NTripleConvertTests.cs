﻿using System;
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
        public void ToTriples_ObjectGraphWithInverseRelation_ReturnsExpectedTriples()
        {
            // Arrange
            var orderId = "bf96fe8b-bcd3-4bda-9989-0967beed8b55";
            var detailId = "ab513d4a-87cd-4ab3-84a4-216c9c5fd78c";
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
            Assert.AreEqual(9, triples.Count());

            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://netriple.com/unittesting/orderdetail/ab513d4a-87cd-4ab3-84a4-216c9c5fd78c>"
                && t.Predicate == "<http://netriple.com/elements/owning_order>"
                && t.Object == "<http://netriple.com/unittesting/order/bf96fe8b-bcd3-4bda-9989-0967beed8b55>"));
        }

        [TestMethod]
        public void ToTriples_ObjectGraphWithNonInverseRelation_ReturnsExpectedTriples()
        {
            // Arrange
            var isbn = "978-3-16-148410-0";
            var chapter = 13;
            var isbnChapter = string.Format("{0}_{1}", isbn, chapter);
            var book = new Book
            {
                Isbn = isbn,
                Title = "RDF FTW",
                Chapters = new List<Chapter>
                {
                    new Chapter
                    {
                        IsbnAndChapter = isbnChapter,
                        ChapterNumber = chapter,
                        Title = "RDF really is the future"
                    }
                }
            };

            // Act
            var triples = book.ToTriples();

            // Assert
            Assert.AreEqual(6, triples.Count());
            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://netriple.com/unittesting/book/978-3-16-148410-0>"
                && t.Predicate == "<http://netriple.com/unittesting/book/contained_chapter>"
                && t.Object == "<http://netriple.com/unittesting/chapter/978-3-16-148410-0_13>"));
        }

        [TestMethod]
        public void ToTriple_InstanceWithUnaryRelation_ReturnsExpectedTriples()
        {
            // Arrange
            var review = new BookReview
            {
                Id = 900,
                Book = new Book
                {
                    Isbn = "978-3-16-148410-0",
                    Title = "Coding Rocks!"
                }
            };

            // Act
            var triples = review.ToTriples();

            // Assert
            Assert.AreEqual(4, triples.Count());
            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://netriple.com/unittesting/book_review/900>"
                && t.Predicate == "<http://netriple.com/unittesting/book_review/book>"
                && t.Object == "<http://netriple.com/unittesting/book/978-3-16-148410-0>"));
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
    }
}
