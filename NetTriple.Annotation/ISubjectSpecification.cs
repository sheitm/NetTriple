using System.Reflection;

namespace NetTriple.Annotation
{
    public interface ISubjectSpecification
    {
        string Template { get; }
        PropertyInfo Property { get; }
    }
}
