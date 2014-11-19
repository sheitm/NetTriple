using System;
using System.Dynamic;
using System.Reflection;

namespace NetTriple.Annotation
{
    public class PropertyPredicateSpecification : IPropertyPredicateSpecification
    {
        public PropertyPredicateSpecification(string predicate, PropertyInfo property)
        {
            Predicate = predicate;
            Property = property;
        }

        public PropertyPredicateSpecification(string predicate, PropertyInfo property, PropertyInfo propSpecifiedProperty) : this(predicate, property)
        {
            PropertySpecifiedProperty = propSpecifiedProperty;
        }

        public string Predicate { get; private set; }
        public PropertyInfo Property { get; private set; }
        public PropertyInfo PropertySpecifiedProperty { get; private set; }
        public Func<string, object> ValueConverter { get; private set; }

        public void SetValueConverter(Func<string, object> converter)
        {
            ValueConverter = converter;
        }
    }
}
