using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NetTriple.Annotation;
using NetTriple.Annotation.Fluency;
using NetTriple.Annotation.Internal;

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
        private readonly IChildrenPredicateSpecification _childAttribute;
        private readonly IBuiltTransform _transform;

        public LinkerSourceCodeGenerator(Type type, PropertyInfo property, IChildrenPredicateSpecification childAttribute, IBuiltTransform transform = null)
        {
            _type = type;
            _property = property;
            _childAttribute = childAttribute;
            _transform = transform;
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
            var getterMethod = _childAttribute.Inverse ? "GetInverse" : "Get";
            sb.AppendFormat("obj.{0} = context.{3}<{1}>(s, \"{2}\");\r\n", _property.Name, _property.PropertyType.FullName, _childAttribute.Predicate, getterMethod);
        }

        private void AppendEnumerableSourceCode(StringBuilder sb)
        {
            var getterMethod = _childAttribute.Inverse ? "GetAllInverse" : "GetAll";
            var innerType = _property.GetTypeOfProperty(); //_property.PropertyType.GenericTypeArguments[0];
            sb.AppendFormat("var all{0} = context.{3}<{1}>(s, \"{2}\");\r\n", _property.Name, innerType.FullName, _childAttribute.Predicate, getterMethod);
            sb.AppendFormat("if (all{0} != null && all{0}.Count() > 0)\r\n", _property.Name);
            sb.Append("{\r\n");
            if (_property.PropertyType.IsArray)
            {
                sb.AppendFormat("obj.{0} = all{0}.ToArray();\r\n", _property.Name);
            }
            else
            {
                sb.AppendFormat("obj.{0} = all{0};\r\n", _property.Name);
            }
            sb.Append("}\r\n");
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
            if (_transform != null)
            {
                var spec = _transform.SubjectSpecification;
                return new KeyValuePair<string, string>(spec.Property.Name, spec.Template);
            }

            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            var attrib = (RdfSubjectAttribute)Attribute.GetCustomAttribute(property, typeof(RdfSubjectAttribute));

            return new KeyValuePair<string, string>(property.Name, attrib.Template);
        }
    }
}
