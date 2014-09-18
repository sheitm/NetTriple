using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RdfChildrenOnClassAttribute : Attribute
    {
        /// <summary>
        /// For each property that is a relation, the following triple must
        /// på specified:
        /// property,predicate,inverse
        /// 
        /// E.g:
        /// Orders,/orders,false;Sibling,/sibling,true
        /// </summary>
        public string PropertyPredicateTriples { get; set; }

        public IEnumerable<KeyValuePair<PropertyInfo, IChildrenPredicateSpecification>> GetChildrenPredicateSpecifications(Type type)
        {
            var arr = PropertyPredicateTriples.Split(';');
            return arr.Select(s =>
            {
                var triple = s.Split(',');
                var spec = new ChildrenPredicateSpecification(triple);
                var pi = type.GetProperty(triple[0], BindingFlags.Instance | BindingFlags.Public);
                return new KeyValuePair<PropertyInfo, IChildrenPredicateSpecification>(pi, spec);
            });
        }
    }
}
