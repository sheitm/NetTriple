using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Chapter")]
    public class Chapter
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/chapter/{0}")]
        public string IsbnAndChapter { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/chapter/chapternumber")]
        public int ChapterNumber { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/chapter/title")]
        public string Title { get; set; }

        public string PrivateSetter { get; private set; }
    }
}
