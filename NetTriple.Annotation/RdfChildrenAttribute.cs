using System;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfChildrenAttribute : Attribute
    {
        public bool Inverse { get; set; }
        public string Predicate { get; set; }
    }
}
