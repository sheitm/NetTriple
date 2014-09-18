using System.Reflection;

namespace NetTriple.Annotation
{
    public interface IChildrenPredicateSpecification
    {
        string Predicate { get; }
        bool Inverse { get; }
        PropertyInfo Property { get; }
    }
}
