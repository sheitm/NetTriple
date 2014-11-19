using System.Collections.Generic;
using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Book")]
    public class Book
    {
        [RdfProperty(Predicate = "http://netriple.com/unittesting/book/title")]
        public string Title { get; set; }

        [RdfSubject(Template = "http://netriple.com/unittesting/book/{0}")]
        public string Isbn { get; set; }

        public int YearOfPublication { get; set; }

        [RdfChildren(Inverse = false, Predicate = "http://netriple.com/unittesting/book/contained_chapter")]
        public IEnumerable<Chapter> Chapters { get; set; }
    }
}
