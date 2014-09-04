using System;
using NetTriple.Annotation;

namespace NetTriple.Emit
{
    public class DeclaredRelation
    {
        private readonly Type _sourceType;
        //private readonly 

        public DeclaredRelation(Type sourceType, RdfTypeAttribute typeAttribute, RdfChildrenAttribute childAttribute,
            Type propertyType)
        {
            _sourceType = sourceType;
        }

        public bool IsRelationFor(Triple triple)
        {
            return false;
        }
    }
}
