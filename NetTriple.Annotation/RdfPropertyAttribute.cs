using System;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfPropertyAttribute : Attribute
    {
        public string Predicate { get; set; }
    }
}
