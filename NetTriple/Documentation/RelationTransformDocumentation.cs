using NetTriple.Annotation;

namespace NetTriple.Documentation
{
    public class RelationTransformDocumentation 
    {
        public RelationTransformDocumentation(IChildrenPredicateSpecification relationSpecification)
        {
            Predicate = relationSpecification.Predicate;
            PropertyName = relationSpecification.Property.Name;
            PropertyType = relationSpecification.Property.PropertyType.FullName;
            Inverse = relationSpecification.Inverse;
        }

        public string Predicate { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public bool Inverse { get; private set; }

    }
}
