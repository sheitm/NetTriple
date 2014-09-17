using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTriple.Annotation.Fluency;
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

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToTriple_SubjectOnClass_GetsTriplesAsExpected()
        {
            // Arrange
            var car = new Car {Id = "XXYY"};

            // Act
            var triples = car.ToTriples();

            // Assert
            Assert.IsTrue(triples.Any(triple => triple.ToString() == "<http://netriple.com/unittesting/car/XXYY> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://netriple.com/unittesting/Car>"));

        }

        [TestMethod]
        public void ToObject_SubjectOnClass_GetsObjectAsExpected()
        {
            // Arrange
            var triples = new List<Triple>
            {
                new Triple{Subject = "<http://netriple.com/unittesting/car/990088>", Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Car>"}
            };

            // Act
            var car = triples.ToObject<Car>();

            // Assert
            Assert.AreEqual("990088", car.Id);
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
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/measurement/value>", Object = "1,22" },
                new Triple{ Subject = subject, Predicate = "<http://netriple.com/unittesting/measurement/unit>", Object = "mW" }
            };

            // Act
            var measurement = triples.ToObject<Measurement>();

            // Assert
            Assert.AreEqual("123", measurement.Id);
            //Assert.AreEqual(1.22000002861023, measurement.Value);
            Assert.AreEqual("mW", measurement.Unit);
        }

        [TestMethod]
        public void ToTriples_ChildrenOnClass_ReturnsExpectedTriples()
        {
            // Arrange
            var measurement = new Measurement
            {
                Id = "111",
                Unit = "mH",
                Value = 123,
                Meter = new Meter{Id = 99},
                MeasurementsPerHour = new List<HourMeasurement>
                {
                    new HourMeasurement {Id = "1111", Level = 1},
                    new HourMeasurement {Id = "1112", Level = 2}
                }
            };

            // Act
            var triples = measurement.ToTriples().ToList();

            // Assert
            Assert.AreEqual(11, triples.Count);
        }

        [TestMethod]
        public void ToObject_ChildrenOnClass_ReturnsExpectedObjectGraph()
        {
            // Arrange
            var measurmentSubject = "<http://netriple.com/unittesting/measurement/111>";
            var hmSubject1 = "<http://netriple.com/unittesting/hourmeasurement/1111>";
            var hmSubject2 = "<http://netriple.com/unittesting/hourmeasurement/1112>";
            var meterSubject = "<http://netriple.com/unittesting/meter/99>";
            var triples = new List<Triple>
            {
                new Triple{Subject = measurmentSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Measurement>"},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurement/value>", Object = "123"},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurement/unit>", Object = "\"mH\""},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurements_per_hour>", Object = "<http://netriple.com/unittesting/hourmeasurement/1111>"},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurements_per_hour>", Object = "<http://netriple.com/unittesting/hourmeasurement/1112>"},

                new Triple{Subject = hmSubject1, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/HourMeasurement>"},
                new Triple{Subject = hmSubject1, Predicate = "<http://netriple.com/unittesting/hourmeasurement/level>", Object = "1"},

                new Triple{Subject = hmSubject2, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/HourMeasurement>"},
                new Triple{Subject = hmSubject2, Predicate = "<http://netriple.com/unittesting/hourmeasurement/level>", Object = "2"},

                new Triple{Subject = meterSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Meter>"},
                new Triple{Subject = meterSubject, Predicate = "<http://netriple.com/unittesting/meter_measurements>", Object = "<http://netriple.com/unittesting/measurement/111>"}
            };

            // Act
            var measurement = triples.ToObject<Measurement>();

            // Assert
            Assert.AreEqual("111", measurement.Id);
            Assert.AreEqual("mH", measurement.Unit);
            Assert.AreEqual(2, measurement.MeasurementsPerHour.Count());
            Assert.AreEqual(99, measurement.Meter.Id);
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

        [TestMethod]
        public void ToTriples_DefinedByTransform_SubjectClassAndPropertiesAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var player = new Player
            {
                Id = "997766",
                Name = "Bjørn Borg",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1955, 12, 4)
            };

            // Act
            var triples = player.ToTriples().ToList();

            // Assert
            var sub = "<http://nettriple/player/997766>";
            Assert.IsTrue(triples.All(t => t.Subject == sub));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" && t.Object == "<http://nettriple/Player>"));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/id>" && t.Object == "\"997766\""));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/gender>" && t.Object == "Male"));
            Assert.IsTrue(triples.Any(t => t.Predicate == "<http://nettriple/player/dateOfBirth>" && t.Object == "\"1955-12-04T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"));

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToObject_DefinedByTransform_InflatesObjectAsExpected()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var subject = "<http://nettriple/player/997766>";
            var triples = new List<Triple>
            {
                new Triple {Subject = subject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://nettriple/Player>"},
                new Triple {Subject = subject, Predicate = "<http://nettriple/player/id>", Object = "\"997766\""},
                new Triple {Subject = subject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple {Subject = subject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1955-12-13T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
            };

            // Act
            var player = triples.ToObject<Player>();

            // Assert
            Assert.AreEqual("997766", player.Id);
            Assert.AreEqual(Gender.Male, player.Gender);
            Assert.AreEqual(1955, player.DateOfBirth.Year);
            Assert.AreEqual(13, player.DateOfBirth.Day);
            Assert.AreEqual(12, player.DateOfBirth.Month);
        }

        [TestMethod]
        public void ToTriples_UnaryRelationsDefinedByTransform_CreatesRelationTriples()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Match>("http://nettriple/Match")
                .Subject(p => p.Id, "http://nettriple/match/{0}")
                .WithPropertyPredicateBase("http://nettriple/match")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Date, "/date")
                .Relation(m => m.Player1, "/player_1")
                .Relation(m => m.Player2, "/player_2"),

                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var match = new Match
            {
                Id = 1,
                Date = new DateTime(2014, 9, 17),
                Player1 = new Player
                {
                    Id = "909",
                    Name = "Bjørn Borg",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1956, 6, 6)
                },
                Player2 = new Player
                {
                    Id = "1001",
                    Name = "John McEnroe",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1959, 2, 16)
                }
            };

            // Act
            var triples = match.ToTriples();

            // Assert
            Assert.IsTrue(triples.Count() > 3);
            Assert.IsTrue(triples.Any(t => 
                t.Subject == "<http://nettriple/match/1>" 
                && t.Predicate == "<http://nettriple/match/player_1>"
                && t.Object == "<http://nettriple/player/909>"));

            foreach (var triple in triples)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToObject_UnaryRelationsDefinedByTransform_GetsExpectedObjectGraph()
        {
            // Arrange
            LoadAllRdfClasses.LoadTransforms(
                BuildTransform.For<Match>("http://nettriple/Match")
                .Subject(p => p.Id, "http://nettriple/match/{0}")
                .WithPropertyPredicateBase("http://nettriple/match")
                .Prop(p => p.Id, "/id")
                .Prop(p => p.Date, "/date")
                .Relation(m => m.Player1, "/player_1")
                .Relation(m => m.Player2, "/player_2"),

                BuildTransform.For<Player>("http://nettriple/Player")
                    .Subject(p => p.Id, "http://nettriple/player/{0}")
                    .WithPropertyPredicateBase("http://nettriple/player")
                    .Prop(p => p.Id, "/id")
                    .Prop(p => p.Name, "/name")
                    .Prop(p => p.Gender, "/gender")
                    .Prop(p => p.DateOfBirth, "/dateOfBirth"));

            var matchSubject = "<http://nettriple/match/1>";
            var player1Subject = "<http://nettriple/player/909>";
            var player2Subject = "<http://nettriple/player/1001>";
            var typePredicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";

            var triples = new List<Triple>
            {
                new Triple{Subject = matchSubject, Predicate = typePredicate, Object = "<http://nettriple/Match>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/id>", Object = "1"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/date>", Object = "\"2014-09-17T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/player_1>", Object = "<http://nettriple/player/909>"},
                new Triple{Subject = matchSubject, Predicate = "<http://nettriple/match/player_2>", Object = "<http://nettriple/player/1001>"},

                new Triple{Subject = player1Subject, Predicate = typePredicate, Object = "<http://nettriple/Player>"},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/id>", Object = "\"909\""},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/name>", Object = "\"Bjørn Borg\""},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple{Subject = player1Subject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1956-06-06T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},

                new Triple{Subject = player2Subject, Predicate = typePredicate, Object = "<http://nettriple/Player>"},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/id>", Object = "1001"},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/name>", Object = "\"John McEnroe\""},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/gender>", Object = "Male"},
                new Triple{Subject = player2Subject, Predicate = "<http://nettriple/player/dateOfBirth>", Object = "\"1959-02-16T00:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>"},
            };

            // Act
            var match = triples.ToObject<Match>();

            // Assert
            Assert.AreEqual(1, match.Id);
            Assert.AreEqual("909", match.Player1.Id);
            Assert.AreEqual("1001", match.Player2.Id);
        }
    }
}
