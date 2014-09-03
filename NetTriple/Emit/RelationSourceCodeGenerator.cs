using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            foreach (var pair in GetChildProperties(_type))
            {
                var isList = typeof (IEnumerable).IsAssignableFrom(pair.Key.PropertyType);
                if (pair.Value.Inverse)
                {
                    if (isList)
                    {
                        AppendInverseRelation(sb, pair.Key, pair.Value);
                    }
                    else
                    {
                        throw new NotImplementedException("Inverse relations not implemented for unary properties.");
                    }
                    
                }
                else
                {
                    if (isList)
                    {
                        AppendNonInverseRelation(sb, pair.Key, pair.Value);
                    }
                    else
                    {
                        AppendNonInverseUnaryRelation(sb, pair.Key, pair.Value);
                    }
                }
            }
        }

        private void AppendNonInverseUnaryRelation(StringBuilder sb, PropertyInfo property, RdfChildrenAttribute attribute)
        {
            var propType = property.PropertyType;
            var childSubjectProp = GetNameOfSubjectProperty(propType);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", r.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.UnaryExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private void AppendNonInverseRelation(StringBuilder sb, PropertyInfo property, RdfChildrenAttribute attribute)
        {
            var propType = GetTypeOfProperty(_type, property);
            var childSubjectProp = GetNameOfSubjectProperty(propType);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.ChildExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private void AppendInverseRelation(StringBuilder sb, PropertyInfo property, RdfChildrenAttribute attribute)
        {
            var propType = GetTypeOfProperty(_type, property);
            var childSubjectProp = GetNameOfSubjectProperty(propType);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.InverseChildExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        public static IEnumerable<KeyValuePair<PropertyInfo, RdfChildrenAttribute>> GetChildProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.GetCustomAttribute(p, typeof (RdfChildrenAttribute)) != null)
                .Aggregate(
                    new Dictionary<PropertyInfo, RdfChildrenAttribute>(),
                    (accumulator, pi) =>
                    {
                        var attrib = (RdfChildrenAttribute)Attribute.GetCustomAttribute(pi, typeof (RdfChildrenAttribute));
                        accumulator[pi] = attrib;
                        return accumulator;
                    }
                );
        }

        private Type GetTypeOfProperty(Type owningType, PropertyInfo property)
        {
            var pType = property.PropertyType;

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
