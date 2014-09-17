using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NetTriple.Annotation;
using NetTriple.Annotation.Fluency;

namespace NetTriple.Emit
{
    /// <summary>
    /// Generates the source code having to do with linking objects
    /// together.
    /// </summary>
    public class RelationSourceCodeGenerator
    {
        private readonly Type _type;
        private readonly IBuiltTransform _transform;

        public RelationSourceCodeGenerator(Type type)
        {
            _type = type;
        }

        public RelationSourceCodeGenerator(IBuiltTransform transform)
        {
            _transform = transform;
        }

        public string GetConversionScript()
        {
            var sb = new StringBuilder();
            AppendConversionScript(sb);
            return sb.ToString();
        }

        public void AppendConversionScript(StringBuilder sb)
        {
            var props = _transform == null ? GetChildProperties(_type) : GetChildProperties(_transform);
            foreach (var pair in props)
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
                        AppendInverseUnaryRelation(sb, pair.Key, pair.Value);
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

        private void AppendNonInverseUnaryRelation(StringBuilder sb, PropertyInfo property, IChildrenPredicateSpecification attribute)
        {
            var propType = property.PropertyType;
            var childSubjectProp =  GetNameOfSubjectProperty(propType, _transform);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", r.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.UnaryExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private void AppendNonInverseRelation(StringBuilder sb, PropertyInfo property, IChildrenPredicateSpecification attribute)
        {
            var propType = GetTypeOfProperty(_type, property);
            var childSubjectProp = GetNameOfSubjectProperty(propType, _transform);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.ChildExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private void AppendInverseRelation(StringBuilder sb, PropertyInfo property, IChildrenPredicateSpecification attribute)
        {
            var propType = GetTypeOfProperty(_type, property);
            var childSubjectProp = GetNameOfSubjectProperty(propType, _transform);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.InverseChildExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        private void AppendInverseUnaryRelation(StringBuilder sb, PropertyInfo property, IChildrenPredicateSpecification attribute)
        {
            var propType = property.PropertyType;
            var childSubjectProp = GetNameOfSubjectProperty(propType, _transform);
            var childProp = string.Format("var co = string.Format(\"<{0}>\", child.{1}.ToString());\r\n", childSubjectProp.Value, childSubjectProp.Key);
            var pred = string.Format("var p = \"<{0}>\";\r\n", attribute.Predicate);
            sb.Append(TemplateResources.InverseUnaryExpansionTemplate
                .Replace("##PROP##", property.Name)
                .Replace("##OBJECTASSIGNMET##", childProp)
                .Replace("##PREDICATEASSIGNMENT##", pred));
        }

        public static IEnumerable<KeyValuePair<PropertyInfo, IChildrenPredicateSpecification>> GetChildProperties(IBuiltTransform transform)
        {
            return transform.RelationSpecifications
                .Aggregate(
                    new Dictionary<PropertyInfo, IChildrenPredicateSpecification>(),
                    (map, each) =>
                    {
                        map.Add(each.Property, each);
                        return map;
                    });
        }

        public static IEnumerable<KeyValuePair<PropertyInfo, IChildrenPredicateSpecification>> GetChildProperties(Type type)
        {
            var list = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.GetCustomAttribute(p, typeof (RdfChildrenAttribute)) != null)
                .Aggregate(
                    new Dictionary<PropertyInfo, IChildrenPredicateSpecification>(),
                    (accumulator, pi) =>
                    {
                        var attrib =
                            (RdfChildrenAttribute) Attribute.GetCustomAttribute(pi, typeof (RdfChildrenAttribute));
                        accumulator[pi] = attrib;
                        return accumulator;
                    }
                ).ToList();

            var onClassAttrib = (RdfChildrenOnClassAttribute)Attribute.GetCustomAttribute(type, typeof (RdfChildrenOnClassAttribute));
            if (onClassAttrib != null)
            {
                list.AddRange(onClassAttrib.GetChildrenPredicateSpecifications(type));
            }

            return list;
        }

        private Type GetTypeOfProperty(Type owningType, PropertyInfo property)
        {
            var pType = property.PropertyType;

            return pType.GenericTypeArguments == null
                ? pType
                : pType.GenericTypeArguments[0];
        }

        private KeyValuePair<string, string> GetNameOfSubjectProperty(Type type, IBuiltTransform transform)
        {
            if (transform != null)
            {
                var relatedTransform = transform.GetRelatedTransform(type);
                if (relatedTransform == null)
                {
                    throw new InvalidOperationException(string.Format("Could not find transform for type: {0}", type.FullName));
                }

                return new KeyValuePair<string, string>(relatedTransform.SubjectSpecification.Property.Name, relatedTransform.SubjectSpecification.Template);
            }

            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            var attrib = (RdfSubjectAttribute)Attribute.GetCustomAttribute(property, typeof(RdfSubjectAttribute));

            return new KeyValuePair<string, string>(property.Name, attrib.Template);
        }
    }
}
