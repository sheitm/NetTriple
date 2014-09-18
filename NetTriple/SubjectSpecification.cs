using System.Reflection;

namespace NetTriple.Annotation
{
    public class SubjectSpecification : ISubjectSpecification
    {
        public SubjectSpecification(string template, PropertyInfo property)
        {
            Template = template;
            Property = property;
        }

        public string Template { get; private set; }
        public PropertyInfo Property { get; private set; }
    }
}
