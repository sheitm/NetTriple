using System.Collections.Generic;
using System.Reflection;
using NetTriple.Annotation.Internal;

namespace NetTriple.Fluency
{
    public class StructureTransform
    {
        public StructureTransform(PropertyInfo property, string predicate)
        {
            Property = property;
            Predicate = predicate;
            IsEnumerable = ReflectionHelper.IsEnumerable(property);
        }

        public PropertyInfo Property { get; private set; }
        public string Predicate { get; private set; }
        public bool IsEnumerable { get; private set; }
        public IEnumerable<StructureTransformElement> Elements { get; set; } 
    }
}
