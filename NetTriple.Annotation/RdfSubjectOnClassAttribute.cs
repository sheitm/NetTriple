using System;

namespace NetTriple.Annotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RdfSubjectOnClassAttribute : Attribute
    {
         public string Template { get; set; }
         public string Property { get; set; }
    }
}
