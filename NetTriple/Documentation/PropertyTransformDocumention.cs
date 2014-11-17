using NetTriple.Annotation;

namespace NetTriple.Documentation
{
    public class PropertyTransformDocumention
    {
        public PropertyTransformDocumention() { }

        public PropertyTransformDocumention(IPropertyPredicateSpecification spec)
        {
            Predicate = spec.Predicate;
            PropertyName = spec.Property.Name;
            PropertyType = spec.Property.PropertyType.FullName;
        }

        public string Predicate { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
    }
}
