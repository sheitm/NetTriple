using System;
using System.Reflection;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfChildrenAttribute : Attribute, IChildrenPredicateSpecification
    {
        public bool Inverse { get; set; }
        public PropertyInfo Property { get; set; }
        public string Predicate { get; set; }
    }
}
