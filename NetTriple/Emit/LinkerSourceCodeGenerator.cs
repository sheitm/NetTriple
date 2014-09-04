using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetTriple.Annotation;

namespace NetTriple.Emit
{
    /// <summary>
    /// Takes a source object, the name of a property on the source
    /// and a target object as input. Figures out how to link the
    /// to together.
    /// </summary>
    public class LinkerSourceCodeGenerator
    {
        private readonly Type _type;
        private readonly PropertyInfo _property;
        private readonly RdfChildrenAttribute _childAttribute;

        public LinkerSourceCodeGenerator(Type type, PropertyInfo property, RdfChildrenAttribute childAttribute)
        {
            _type = type;
            _property = property;
            _childAttribute = childAttribute;
        }

        public void AppendSourceCode(StringBuilder sb)
        {
            if (typeof (IEnumerable).IsAssignableFrom(_property.PropertyType))
            {
                AppendEnumerableSourceCode(sb);
            }
            else
            {
                AppendUnarySourceCode(sb);
            }
        }

        private void AppendUnarySourceCode(StringBuilder sb)
        {
            if (_childAttribute.Inverse)
            {
                throw new NotImplementedException("Inverse not implemented");
            }
            else
            {
                //T Get<T>(string sourceSubject, string predicate);
                sb.AppendFormat("obj.{0} = context.Get<{1}>(s, \"{2}\");\r\n", _property.Name, _property.PropertyType.FullName, _childAttribute.Predicate);
            }
        }

        private void AppendEnumerableSourceCode(StringBuilder sb)
        {

            //throw new NotImplementedException();
        }

        public void AppendSubjectAssignment(StringBuilder sb)
        {
            var subjectProperty = GetNameOfSubjectProperty(_type);
            sb.AppendFormat("var s1 = obj.{0}.ToString();\r\n", subjectProperty.Key);
            sb.AppendFormat("var template = \"<{0}>\";\r\n", subjectProperty.Value);
            sb.Append("var s = template.Replace(\"{0}\", s1);\r\n");
        }

        private KeyValuePair<string, string> GetNameOfSubjectProperty(Type type)
        {
            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            var attrib = (RdfSubjectAttribute)Attribute.GetCustomAttribute(property, typeof(RdfSubjectAttribute));

            return new KeyValuePair<string, string>(property.Name, attrib.Template);
        }
    }
}
