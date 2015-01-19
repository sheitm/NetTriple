using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetTriple.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        //public void ToBytes_WithValidTriples_

        [TestMethod]
        public void ToNTriples_WithValidTriples_ConvertsToNTriples()
        {
            // Arrange
            var triples = GetTriples();

            // Act
            var nTriples = triples.ToNTriples();

            // Assert
            Assert.IsTrue(nTriples.Contains("<http://netriple.com/unittesting/hourmeasurement/1111> <http://netriple.com/unittesting/hourmeasurement/level> 1 .\r\n"));
        }

        [TestMethod]
        public void ToTriplesFromNTriples_WithValidNTriples_ReturnsExpectedTriples()
        {
            // Arrange
            var nTriples = TestResources.NTriples;

            // Act
            var triples = nTriples.ToTriplesFromNTriples().ToList();

            // Assert
            Assert.AreEqual(11, triples.Count);
            Assert.IsTrue(triples.Any(t => 
                t.Subject == "<http://netriple.com/unittesting/measurement/111>"
                && t.Predicate == "<http://netriple.com/unittesting/measurement/value>"
                && t.Object == "123"));
        }

        [TestMethod]
        public void ToTriplesFromNTriples_WithHashes_ReturnsExpectedTriples()
        {
            // Arrange
            var nTriples = TestResources.NTriplesWithHashes;

            // Act
            var triples = nTriples.ToTriplesFromNTriples().ToList();

            // Assert
            Assert.AreEqual(19, triples.Count);
            Assert.IsTrue(triples.All(triple => triple.Subject.StartsWith("<http:")));
        }

        [TestMethod]
        public void ToBytes_WithValidTriples_ReturnsBytesAsExpected()
        {
            // Arrange
            var triples = GetTriples();

            // Act
            var bytes = triples.ToBytes();

            // Assert
            Assert.AreEqual(3090, bytes.Length);
        }

        [TestMethod]
        public void ToTriples_FromBytes_ReturnsExpectedTriples()
        {
            // Arrange
            var bytes = GetTriples().ToBytes();

            // Act
            var triples = bytes.ToTriples().ToList();

            // Assert
            Assert.AreEqual(11, triples.Count);
            Assert.IsTrue(triples.Any(t =>
                t.Subject == "<http://netriple.com/unittesting/measurement/111>"
                && t.Predicate == "<http://netriple.com/unittesting/measurement/value>"
                && t.Object == "123"));
        }

        [TestMethod]
        public void ToNTriples_BackAndForth_TriplesAsIdentical()
        {
            // Arrange
            var triples = GetTriples().ToList();

            // Act
            var triples2 = triples.ToNTriples().ToTriplesFromNTriples().ToList();

            // Assert
            Assert.AreEqual(triples.Count(), triples2.Count);
            foreach (var triple in triples)
            {
                var triple2 = triples2.SingleOrDefault(t => 
                    t.Subject == triple.Subject && 
                    t.Predicate == triple.Predicate &&
                    t.Object == triple.Object);
                var msg = string.Format("{0}", triple);
                Assert.IsNotNull(triple2, msg);
            }
        }

        [TestMethod]
        public void ToTriplesFromNTriples_WithDoubleSpaceInFrontOfObject_GetsExpectedTriples()
        {
            // Arrange

            // Act
            var triples = TestResources.TriplesDoubleSpace.ToTriplesFromNTriples().ToList();

            // Assert
            var terminalTriple = triples.Single(t => t.Predicate == "<http://nettriple.com/schema/terminal>");
            Assert.AreEqual("\"000D6F00003862F5\"", terminalTriple.Object);
        }

        [TestMethod]
        public void ToCamelCase_PascalCase_ReturnsExpected()
        {
            // Arrange
            var pascal = "ThisIsPascal";

            // Act
            var camel = pascal.ToCamelCase();

            // Assert
            Assert.AreEqual("thisIsPascal", camel);
        }

        [TestMethod]
        public void ToCamelCase_AlreadyPascalCase_ReturnsExpected()
        {
            // Arrange
            var cml = "thisIsCamel";

            // Act
            var camel = cml.ToCamelCase();

            // Assert
            Assert.AreEqual(cml, camel);
        }

        [TestMethod]
        public void ToCamelCase_SingleUppercase_ReturnsExpected()
        {
            // Arrange
            var cml = "X";

            // Act
            var camel = cml.ToCamelCase();

            // Assert
            Assert.AreEqual("x", camel);
        }

        [TestMethod]
        public void HowDoes_ExpressionToString_Work()
        {
            // Expression<Func<T, object>>
            Expression<Func<string, int>> expr = s => int.Parse(s.Substring(4, 2));
            var ex2 = (Expression) expr;
            
            var bdy = expr.Body.ToString();

            Console.WriteLine(bdy);
        }

        [TestMethod]
        public void ToTypeUniqueTriples_WithDuplicatedTriples_ReturnsDeduplicatedTriples()
        {
            // Arrange
            var triples = TestResources.TriplesTypeDuplicated.ToTriplesFromNTriples().ToList();

            // Act
            var duplicated = triples.ToTypeUniqueTriples().ToList();

            // Assert
            Assert.AreEqual((triples.Count() * 2) - 2, duplicated.Count());

            foreach (var triple in duplicated)
            {
                Console.WriteLine(triple);
            }
        }

        [TestMethod]
        public void ToEnumValueFromRdfEnum_Null_ReturnsNull()
        {
            // Arrange
            string s = null;

            // Act
            var enumString = s.ToEnumValueFromRdfEnum();

            // Assert
            Assert.IsNull(enumString);
        }

        [TestMethod]
        public void ToEnumValueFromRdfEnum_Whitespace_ReturnsNull()
        {
            // Arrange
            string s = "            ";

            // Act
            var enumString = s.ToEnumValueFromRdfEnum();

            // Assert
            Assert.IsNull(enumString);
        }

        [TestMethod]
        public void ToEnumValueFromRdfEnum_WithAngledBracketes_ReturnsExpectedValue()
        {
            // Arrange
            string s = "<http://nettriples.com/extensions/expectedValue>";

            // Act
            var enumString = s.ToEnumValueFromRdfEnum();

            // Assert
            Assert.AreEqual("expectedValue", enumString);
        }

        [TestMethod]
        public void ToEnumValueFromRdfEnum_WithAngledBracketesAndTrailingSlash_ReturnsExpectedValue()
        {
            // Arrange
            string s = "<http://nettriples.com/extensions/expectedValue/>";

            // Act
            var enumString = s.ToEnumValueFromRdfEnum();

            // Assert
            Assert.AreEqual("expectedValue", enumString);
        }

        [TestMethod]
        public void ToEnumValueFromRdfEnum_PlainUrl_ReturnsExpectedValue()
        {
            // Arrange
            string s = "http://nettriples.com/extensions/expectedValue";

            // Act
            var enumString = s.ToEnumValueFromRdfEnum();

            // Assert
            Assert.AreEqual("expectedValue", enumString);
        }

        [TestMethod]
        public void ToEnumValueFromRdfEnum_PlainUrlTrailingSlash_ReturnsExpectedValue()
        {
            // Arrange
            string s = "http://nettriples.com/extensions/expectedValue/";

            // Act
            var enumString = s.ToEnumValueFromRdfEnum();

            // Assert
            Assert.AreEqual("expectedValue", enumString);
        }

        [TestMethod]
        public void Compression_WithData_CompressesAndDecompressesCorrectly()
        {
            // Arrange
            var original = TestResources.NTriplesWithHashes;

            // Act
            var compressed = original.Compress();
            var decompressed = compressed.Decompress();

            // Assert
            Assert.IsTrue(compressed.Length < decompressed.Length);
            Assert.AreEqual(original, decompressed);
        }

        private IEnumerable<Triple> GetTriples()
        {
            var measurmentSubject = "<http://netriple.com/unittesting/measurement/111>";
            var hmSubject1 = "<http://netriple.com/unittesting/hourmeasurement/1111>";
            var hmSubject2 = "<http://netriple.com/unittesting/hourmeasurement/1112>";
            var meterSubject = "<http://netriple.com/unittesting/meter/99>";
            return new List<Triple>
            {
                new Triple{Subject = measurmentSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Measurement>"},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurement/value>", Object = "123"},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurement/unit>", Object = "\"mH\""},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurements_per_hour>", Object = "<http://netriple.com/unittesting/hourmeasurement/1111>"},
                new Triple{Subject = measurmentSubject, Predicate = "<http://netriple.com/unittesting/measurements_per_hour>", Object = "<http://netriple.com/unittesting/hourmeasurement/1112>"},

                new Triple{Subject = hmSubject1, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/HourMeasurement>"},
                new Triple{Subject = hmSubject1, Predicate = "<http://netriple.com/unittesting/hourmeasurement/level>", Object = "1"},

                new Triple{Subject = hmSubject2, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/HourMeasurement>"},
                new Triple{Subject = hmSubject2, Predicate = "<http://netriple.com/unittesting/hourmeasurement/date>", Object = "14.06.2014 00:00:00"},

                new Triple{Subject = meterSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Meter>"},
                new Triple{Subject = meterSubject, Predicate = "<http://netriple.com/unittesting/meter_measurements>", Object = "<http://netriple.com/unittesting/measurement/111>"}
            };
        }
    }
}
