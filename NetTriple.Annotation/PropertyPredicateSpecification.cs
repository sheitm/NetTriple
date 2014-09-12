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

        public string Predicate { get; private set; }
        public PropertyInfo Property { get; private set; }
    }
}
