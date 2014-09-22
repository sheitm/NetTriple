using System.Collections.Generic;
using System.Linq;
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
            Assert.IsTrue(nTriples.Contains("<http://netriple.com/unittesting/hourmeasurement/1112> <http://netriple.com/unittesting/hourmeasurement/level> 2 .\r\n"));
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
        public void ToBytes_WithValidTriples_ReturnsBytesAsExpected()
        {
            // Arrange
            var triples = GetTriples();

            // Act
            var bytes = triples.ToBytes();

            // Assert
            Assert.AreEqual(3056, bytes.Length);
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
                new Triple{Subject = hmSubject2, Predicate = "<http://netriple.com/unittesting/hourmeasurement/level>", Object = "2"},

                new Triple{Subject = meterSubject, Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Object = "<http://netriple.com/unittesting/Meter>"},
                new Triple{Subject = meterSubject, Predicate = "<http://netriple.com/unittesting/meter_measurements>", Object = "<http://netriple.com/unittesting/measurement/111>"}
            };
        }
    }
}
