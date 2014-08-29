using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/book_review")]
    public class BookReview
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/book_review/{0}")]
        public int Id { get; set; }

        [RdfChildren(Inverse = false, Predicate = "http://netriple.com/unittesting/book_review/book")]
        public Book Book { get; set; }
    }
}
