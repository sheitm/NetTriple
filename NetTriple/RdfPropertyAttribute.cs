﻿using System;
using System.Reflection;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfPropertyAttribute : Attribute, IPropertyPredicateSpecification
    {
        public string Predicate { get; set; }

        public PropertyInfo Property { get; private set; }
        public PropertyInfo PropertySpecifiedProperty { get; private set; }
        public Func<string, object> ValueConverter { get; private set; }

        public void SetProperty(PropertyInfo property)
        {
            Property = property;
        }
    }
}
