using System;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RdfTypeAttribute : Attribute
    {
        public string Predicate { get; set; }
        public string Value { get; set; }
    }
}
