using System;
using NetTriple.Annotation;

namespace NetTriple.Documentation
{
    public class PropertyTransformDocumention
    {
        private readonly Type _propertyType;

        public PropertyTransformDocumention() { }

        public PropertyTransformDocumention(IPropertyPredicateSpecification spec)
        {
            Predicate = spec.Predicate;
            PropertyName = spec.Property.Name;
            _propertyType = spec.Property.PropertyType;
            PropertyType = _propertyType.FullName;
        }

        public string Predicate { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }

        public Type GetPropertyType()
        {
            return _propertyType;
        }
    }
}
