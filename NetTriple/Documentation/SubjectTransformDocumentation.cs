using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using NetTriple.Annotation;

namespace NetTriple.Documentation
{
    public class SubjectTransformDocumentation
    {
        public SubjectTransformDocumentation() { }

        public SubjectTransformDocumentation(ISubjectSpecification specification)
        {
            Template = specification.Template;
            PropertyName = specification.Property.Name;
            PropertyType = specification.Property.PropertyType.FullName;
        }

        public string Template { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
    }
}
