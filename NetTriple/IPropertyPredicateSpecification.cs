using System;
using System.Reflection;

namespace NetTriple.Annotation
{
    public interface IPropertyPredicateSpecification
    {
        string Predicate { get; }
        PropertyInfo Property { get; }
        PropertyInfo PropertySpecifiedProperty { get; }
        Func<string, object> ValueConverter { get; }
    }
}
