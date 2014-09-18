using System;
using System.Reflection;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfPropertyAttribute : Attribute, IPropertyPredicateSpecification
    {
        public string Predicate { get; set; }

        public PropertyInfo Property { get; private set; }

        public void SetProperty(PropertyInfo property)
        {
            Property = property;
        }
    }
}
