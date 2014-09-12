using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RdfPropertyOnClassAttribute : Attribute
    {
        /// <summary>
        /// e.g. http://nettriple/mytype
        /// </summary>
        public string PredicateBase { get; set; }

        /// <summary>
        /// Pairs of property names and predicates. The predicates can be either full
        /// predicates, or just the extesion (suffix) of the PredicateBase. 
        /// 
        /// e.g. Value,/value;Unit,/unit
        /// </summary>
        public string PropertyPredicatePairs { get; set; }

        public IEnumerable<IPropertyPredicateSpecification> GetPropertyPredicateSpecifications(Type type)
        {
            var arr = PropertyPredicatePairs.Split(';');
            return arr.Select(s =>
            {
                var pair = s.Split(',');
                var pred = string.IsNullOrEmpty(PredicateBase) ? pair[1] : PredicateBase + pair[1];
                var prop = type.GetProperty(pair[0], BindingFlags.Instance | BindingFlags.Public);
                return new PropertyPredicateSpecification(pred, prop);
            });
        }

        
    }
}
