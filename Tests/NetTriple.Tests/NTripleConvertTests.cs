using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            foreach (var triple in triples)
            {
                Console.WriteLine(string.Format("{0} {1} {2}", triple.Subject, triple.Predicate, triple.Object));
            }
        }

        [TestMethod]
        public void ToTriples_PropertiesOnClass_ReturnsExpectedTriples()
        {
            // Arrange
            var measurement = new Measurement
            {
                Id = "123",
                Value = (float)1.123,
                Unit = "mW"
            };

            // Act
            var triples = measurement.ToTriples().ToList();

            // Assert
            Assert.IsTrue(triples.Any(t => t.ToString() == "<http://netriple.com/unittesting/measurement/123> <http://netriple.com/unittesting/measurement/value> 1.123"));
        }

        [TestMethod]
        public void ToObject_PropertiesOnClass_ReturnsExpectedTriples()
        {
            // Arrange
            var subject = "<http://netriple.com/unittesting/measurement/123>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Measurement>" },
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/measurement/value>", Object = "1" },
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/measurement/unit>", Object = "mW" }
            };

            // Act
            var measurement = triples.ToObject<Measurement>();

            // Assert
            Assert.AreEqual("123", measurement.Id);
            Assert.AreEqual(1.0, measurement.Value);
            Assert.AreEqual("mW", measurement.Unit);
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
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/order/description>", Object = "Testing again" }
            };

            // Act
            var order = triples.ToObject<Order>();

            // Assert
            Assert.IsNotNull(order);
            Assert.AreEqual("c7519f11-686d-4087-856e-d014e61cfed3", order.Id);
            Assert.AreEqual(1, order.OrderNumber);
            Assert.AreEqual("Testing again", order.Description);
        }

        [TestMethod]
        public void ToObject_GraphWithForwardPointingNonListReference_ReturnesExpectedInstances()
        {
            // Arrange
            var reviewSubject = "<http://netriple.com/unittesting/book_review/900>";
            var bookSubject = "<http://netriple.com/unittesting/book/978-3-16-148410-0>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = reviewSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/book_review>" },
                new Triple{ Subject = bookSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Book>" },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/title>", Object = "Coding Rocks!" },
                new Triple{ Subject = reviewSubject, Predicate = "<http://netriple.com/unittesting/book_review/book>", Object = bookSubject }
            };

            // Act
            var review = triples.ToObject<BookReview>();

            // Assert
            Assert.IsNotNull(review);
            Assert.IsNotNull(review.Book);
            Assert.AreEqual("Coding Rocks!", review.Book.Title);
        }

        [TestMethod]
        public void ToObject_GraphWithForwardPointingListReference_ReturnesExpectedInstances()
        {
            // Arrange
            var bookSubject = "<http://netriple.com/unittesting/book/978-3-16-148410-0>";
            var chapterSubject = "<http://netriple.com/unittesting/chapter/978-3-16-148410-0_13>";
            var chapter2Subject = "<http://netriple.com/unittesting/chapter/978-3-16-148410-0_14>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = bookSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Book>" },
                new Triple{ Subject = chapterSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Chapter>" },
                new Triple{ Subject = chapter2Subject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Chapter>" },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/contained_chapter>", Object = chapterSubject },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/contained_chapter>", Object = chapter2Subject },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/title>", Object = "RDF FTW" },
                new Triple{ Subject = chapterSubject, Predicate = "<http://netriple.com/unittesting/chapter/chapternumber>", Object = "13" },
                new Triple{ Subject = chapterSubject, Predicate = "<http://netriple.com/unittesting/chapter/title>", Object = "RDF really is the future" },
                new Triple{ Subject = chapter2Subject, Predicate = "<http://netriple.com/unittesting/chapter/chapternumber>", Object = "14" },
                new Triple{ Subject = chapter2Subject, Predicate = "<http://netriple.com/unittesting/chapter/title>", Object = "50 ways to utilise RDF" }
            };

            // Act
            var book = triples.ToObject<Book>();

            // Assert
            Assert.AreEqual(2, book.Chapters.Count());
            Assert.IsTrue(book.Chapters.Any(c => c.ChapterNumber == 13));
            Assert.IsTrue(book.Chapters.Any(c => c.ChapterNumber == 14));
        }

        [TestMethod]
        public void ToObject_GraphWithInverseListReference_ReturnesExpectedInstances()
        {
            // Arrange
            var orderSubject = "<http://netriple.com/unittesting/order/bf96fe8b-bcd3-4bda-9989-0967beed8b55>";
            var detailSubject = "<http://netriple.com/unittesting/orderdetail/ab513d4a-87cd-4ab3-84a4-216c9c5fd78c>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = orderSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Order>" },
                new Triple{ Subject = detailSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/OrderDetail>" },
                new Triple{ Subject = detailSubject, Predicate = "<http://netriple.com/elements/owning_order>", Object = orderSubject },

                new Triple{ Subject = orderSubject, Predicate = "<http://netriple.com/unittesting/order/ordernumber>", Object = "1" },
                new Triple{ Subject = orderSubject, Predicate = "<http://netriple.com/unittesting/order/description>", Object = "Testing" },

                new Triple{ Subject = detailSubject, Predicate = "<http://netriple.com/unittesting/order/orderdetailnumber>", Object = "1" },
                new Triple{ Subject = detailSubject, Predicate = "<http://netriple.com/unittesting/order/description>", Object = "Test detail" },
                new Triple{ Subject = detailSubject, Predicate = "<http://netriple.com/unittesting/order/quantity>", Object = "5" },
                new Triple{ Subject = detailSubject, Predicate = "<http://netriple.com/unittesting/order/productcode>", Object = "0033" }
            };

            // Act
            var order = triples.ToObject<Order>();
        }

        [TestMethod]
        public void ToTriples_WithInverseUnaryRelation_ReturnsExpectedTriples()
        {
            // Arrange
            var wife = new Wife {Id = 1, Name = "Kari"};
            var husband = new Husband {Id = 2, Name = "Per", Wife = wife};

            // Act
            var triples = husband.ToTriples().ToList();

            // Assert
            Assert.AreEqual(5, triples.Count());
            Assert.IsTrue(triples.Any(t => t.ToString() == "<http://netriple.com/unittesting/husband/2> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://netriple.com/unittesting/husband>"));
            Assert.IsTrue(triples.Any(t => t.ToString() == "<http://netriple.com/unittesting/husband/2> <http://netriple.com/unittesting/husband/name> \"Per\""));
            Assert.IsTrue(triples.Any(t => t.ToString() == "<http://netriple.com/unittesting/wife/1> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://netriple.com/unittesting/wife>"));
            Assert.IsTrue(triples.Any(t => t.ToString() == "<http://netriple.com/unittesting/wife/1> <http://netriple.com/unittesting/wife/name> \"Kari\""));
            Assert.IsTrue(triples.Any(t => t.ToString() == "<http://netriple.com/unittesting/wife/1> <http://netriple.com/unittesting/wife_to_husband> <http://netriple.com/unittesting/husband/2>"));
        }

        [TestMethod]
        public void ToObject_WithInverseUnaryRelation_ReturnsExpectedObject()
        {
            // Arrange
            var husbandSubject = "<http://netriple.com/unittesting/husband/2>";
            var wifeSubject = "<http://netriple.com/unittesting/wife/1>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";

            var triples = new List<Triple>
            {
                new Triple{Subject = husbandSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/husband>"},
                new Triple{Subject = wifeSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/wife>"},
                new Triple{Subject = wifeSubject, Predicate = "<http://netriple.com/unittesting/wife_to_husband>", Object = husbandSubject},
                new Triple{Subject = wifeSubject, Predicate = "<http://netriple.com/unittesting/wife/name>", Object = "Kari"},
                new Triple{Subject = husbandSubject, Predicate = "<http://netriple.com/unittesting/husband/name>", Object = "Per"},
            };

            // Act
            var husband = triples.ToObject<Husband>();

            // Assert
            Assert.AreEqual(2, husband.Id);
            Assert.IsNotNull(husband.Wife);
            Assert.AreEqual(1, husband.Wife.Id);
        }

        [TestMethod]
        public void ToObjects_HappyDays_ReturnsAllObjects()
        {
            // Arrange
            var bookSubject = "<http://netriple.com/unittesting/book/978-3-16-148410-0>";
            var chapterSubject = "<http://netriple.com/unittesting/chapter/978-3-16-148410-0_13>";
            var chapter2Subject = "<http://netriple.com/unittesting/chapter/978-3-16-148410-0_14>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            var triples = new List<Triple>
            {
                new Triple{ Subject = bookSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Book>" },
                new Triple{ Subject = chapterSubject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Chapter>" },
                new Triple{ Subject = chapter2Subject, Predicate = typePredicate, Object = "<http://netriple.com/unittesting/Chapter>" },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/contained_chapter>", Object = chapterSubject },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/contained_chapter>", Object = chapter2Subject },
                new Triple{ Subject = bookSubject, Predicate = "<http://netriple.com/unittesting/book/title>", Object = "RDF FTW" },
                new Triple{ Subject = chapterSubject, Predicate = "<http://netriple.com/unittesting/chapter/chapternumber>", Object = "13" },
                new Triple{ Subject = chapterSubject, Predicate = "<http://netriple.com/unittesting/chapter/title>", Object = "RDF really is the future" },
                new Triple{ Subject = chapter2Subject, Predicate = "<http://netriple.com/unittesting/chapter/chapternumber>", Object = "14" },
                new Triple{ Subject = chapter2Subject, Predicate = "<http://netriple.com/unittesting/chapter/title>", Object = "50 ways to utilise RDF" }
            };

            // Act
            var objects = triples.ToObjects().ToList();

            // Assert
            Assert.AreEqual(3, objects.Count());
            Assert.AreEqual(1, objects.Count(o => o is Book));
            Assert.AreEqual(2, objects.Count(o => o is Chapter));
        }

        [TestMethod]
        public void ConvertToTriple_PerfectlyFormatted_ReturnsExpectedTriple()
        {
            // Arrange
            var s =
                "<http://nettriple/workorder/99> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://nettriple/workorder>";

            // Act
            var triple = s.ConvertToTriple();

            // Assert
            Assert.AreEqual(s, triple.ToString());
        }

        [TestMethod]
        public void ConvertToTriple_WhitespaceInObject_ReturnsExpectedTriple()
        {
            // Arrange
            var s =
                "<http://nettriple/workorder/99> <http://nettriple/name> \"Hey! There are spaces!  !  !\"";

            // Act
            var triple = s.ConvertToTriple();

            // Assert
            Assert.AreEqual(s, triple.ToString());
        }
    }
}
