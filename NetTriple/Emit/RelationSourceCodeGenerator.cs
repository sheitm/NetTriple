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
            foreach (var pair in GetChildProperties())
            {
                if (pair.Value.Inverse)
                {
                    AppendInverseRelation(sb, pair.Key, pair.Value);
                }
                else
                {
                    AppendNonInverseRelation(sb, pair.Key, pair.Value);
                }
            }
        }

        public string GetInflationScript()
        {
            return null;
        }

        private void AppendNonInverseRelation(StringBuilder sb, string propertyName, RdfChildrenAttribute attribute)
        {
            var propType = GetTypeOfProperty(_type, propertyName);
            var childSubjectProp = GetNameOfSubjectProperty(propType);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1});\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.ChildExpansionTemplate
                .Replace("##PROP##", propertyName)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private void AppendInverseRelation(StringBuilder sb, string propertyName, RdfChildrenAttribute attribute)
        {
            var propType = GetTypeOfProperty(_type, propertyName);
            var childSubjectProp = GetNameOfSubjectProperty(propType);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1});\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.InverseChildExpansionTemplate
                .Replace("##PROP##", propertyName)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private IEnumerable<KeyValuePair<string, RdfChildrenAttribute>> GetChildProperties()
        {
            return _type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.GetCustomAttribute(p, typeof (RdfChildrenAttribute)) != null)
                .Aggregate(
                    new Dictionary<string, RdfChildrenAttribute>(),
                    (accumulator, pi) =>
                    {
                        var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(pi, typeof (RdfChildrenAttribute));
                        accumulator[pi.Name] = attrib;
                        return accumulator;
                    }
                );
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
