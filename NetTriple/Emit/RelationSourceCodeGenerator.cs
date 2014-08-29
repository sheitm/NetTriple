using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetTriple.Annotation;

namespace NetTriple.Emit
{
    /// <summary>
    /// Generates the source code having to do with linking objects
    /// together.
    /// </summary>
    public class RelationSourceCodeGenerator
    {
        private readonly Type _type;

        public RelationSourceCodeGenerator(Type type)
        {
            _type = type;
        }

        public string GetConversionScript()
        {
            var sb = new StringBuilder();
            AppendConversionScript(sb);
            return sb.ToString();
        }

        public void AppendConversionScript(StringBuilder sb)
        {
            foreach (var childProperty in GetChildProperties())
            {
                var propType = GetTypeOfProperty(_type, childProperty);
                var childSubjectProp = GetNameOfSubjectProperty(propType);
                var childPropSb = new StringBuilder();
                childPropSb.AppendFormat("var co = string.Format(\"<{0}>\", child.{1});\r\n", childSubjectProp.Value, childSubjectProp.Key);
                sb.Append(TemplateResources.ChildExpansionTemplate.Replace("##PROP##", childProperty).Replace("##OBJECTASSIGNMET##", childPropSb.ToString()));
            }
        }

        public string GetInflationScript()
        {
            return null;
        }

        private IEnumerable<string> GetChildProperties()
        {
            return _type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.GetCustomAttribute(p, typeof(RdfChildrenAttribute)) != null)
                .Select(prop => prop.Name);
        }

        private Type GetTypeOfProperty(Type owningType, string propertyName)
        {
            var pType = owningType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public).PropertyType;

            return pType.GenericTypeArguments == null
                ? pType
                : pType.GenericTypeArguments[0];
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
