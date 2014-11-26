using System;
using System.Collections.Generic;
using NetTriple.Annotation;

namespace NetTriple.Documentation
{
    public class SubjectTransformDocumentation
    {
        private static readonly Random Rand = new Random();

        private readonly Type _propertyType;
        
        public SubjectTransformDocumentation() { }

        public SubjectTransformDocumentation(ISubjectSpecification specification)
        {
            Template = specification.Template;
            PropertyName = specification.Property.Name;
            _propertyType = specification.Property.PropertyType;
            PropertyType = _propertyType.FullName;
        }

        public string Template { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }

        public KeyValuePair<string, string> GetSampleSubject()
        {
            var v = typeof (int) == _propertyType
                ? Rand.Next().ToString()
                : Guid.NewGuid().ToString().Substring(0, 8);

            return new KeyValuePair<string, string>(v, string.Format(Template, v));
        }
    }
}
