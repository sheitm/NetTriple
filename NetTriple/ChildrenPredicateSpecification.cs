using System.Reflection;

namespace NetTriple.Annotation
{
    public class ChildrenPredicateSpecification : IChildrenPredicateSpecification
    {
        public ChildrenPredicateSpecification(string[] triple)
        {
            Predicate = triple[1];
            Inverse = triple.Length > 2 && triple[2].ToLower() == "true";
        }

        public ChildrenPredicateSpecification(PropertyInfo property, string predicate, bool inverse)
        {
            Property = property;
            Predicate = predicate;
            Inverse = inverse;
        }

        public string Predicate { get; private set; }
        public bool Inverse { get; private set; }
        public PropertyInfo Property { get; private set; }
    }
}
