using System;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RdfSubjectAttribute : Attribute
    {
        public string Template { get; set; }
    }
}
