using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NetTriple.Annotation;
using NetTriple.Annotation.Fluency;

namespace NetTriple.Emit
{
    public class SourceCodeGenerator
    {
        private readonly Type _type;
        private readonly RdfTypeAttribute _rdfTypeAttribute;

        private readonly IBuiltTransform _transform;

        public SourceCodeGenerator(IBuiltTransform transform)
        {
            _transform = transform;
        }

        public SourceCodeGenerator(Type type)
        {
            _type = type;
            _rdfTypeAttribute = (RdfTypeAttribute)Attribute.GetCustomAttribute(_type, typeof(RdfTypeAttribute));
        }

        public string GetSourceCode()
        {
            var type = DomainType;
            return TemplateResources.ConverterTemplate
                .Replace("##CLASS##", type.Name)
                .Replace("##FULLCLASS##", type.FullName)
                .Replace("##CONVERSION##", GetConversionScript())
                .Replace("##INFLATION##", GetInflationScript())
                .Replace("##RDFSUBJECT##", GetRdfSubjectTemplate().ToWashedRdfSubject())
                .Replace("##LINK##", GetLinkInvocationScript())
                .Replace("##LINKCORE##", GetLinkScript());
        }

        private Type DomainType
        {
            get { return _type ?? _transform.Type; }
        }

        private string GetLinkInvocationScript()
        {
            return string.Format("LinkCore(({0}) obj, context);\r\n", DomainType.FullName);
        }

        private string GetLinkScript()
        {
            var type = DomainType;
            var sb = new StringBuilder();

            sb.AppendFormat("private void LinkCore({0} obj, IInflationContext context)\r\n", type.FullName);
            sb.AppendLine("{");

            var started = false;
            var childProperties = _transform == null
                ? RelationSourceCodeGenerator.GetChildProperties(type)
                : RelationSourceCodeGenerator.GetChildProperties(_transform);

            foreach (var pair in childProperties)
            {
                var linkGenerator = new LinkerSourceCodeGenerator(type, pair.Key, pair.Value, _transform);
                if (!started)
                {
                    started = true;
                    linkGenerator.AppendSubjectAssignment(sb);
                }

                linkGenerator.AppendSourceCode(sb);
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GetInflationScript()
        {
            var sb = new StringBuilder();

            var subjectProperty = GetNameOfSubjectProperty(_type);
            sb.AppendLine("triple = triples.First();");
            var subjectPropertyType = GetTypeOfSubjectProperty(_type);
            if (subjectPropertyType == typeof (int))
            {
                sb.AppendFormat("obj.{0} = int.Parse(triple.Subject.GetIdOfSubject());\r\n", subjectProperty.Key);
            }
            else
            {
                sb.AppendFormat("obj.{0} = triple.Subject.GetIdOfSubject();\r\n", subjectProperty.Key);
            }

            foreach (var ppInfo in GetRdfProperties())
            {
                var prop = ppInfo.Property;

                sb.AppendFormat("triple = triples.SingleOrDefault(t => t.Predicate == \"<{0}>\");\r\n", ppInfo.Predicate);
                sb.Append("if (triple != null) { obj.");
                sb.AppendFormat("{0} = triple.GetObject<{1}>(); ", prop.Name, prop.PropertyType.FullName);
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private string GetConversionScript()
        {
            var sb = new StringBuilder();

            var subjectProperty = GetNameOfSubjectProperty(DomainType);
            sb.AppendFormat("var s1 = obj.{0}.ToString();\r\n", subjectProperty.Key);
            sb.AppendFormat("var template = \"<{0}>\";\r\n", subjectProperty.Value);
            sb.Append("var s = template.Replace(\"{0}\", s1);\r\n");

            sb.Append("triples.Add(new Triple { ");
            sb.AppendFormat("Subject = s, Predicate = \"<{0}>\", Object = \"<{1}>\"",
                _rdfTypeAttribute == null ? _transform.TypePredicate : _rdfTypeAttribute.Predicate,
                _rdfTypeAttribute == null ? _transform.TypeString : _rdfTypeAttribute.Value);

            sb.AppendLine(" });");

            foreach (var ppInfo in GetRdfProperties())
            {
                sb.Append("triples.Add(new Triple { ");
                sb.AppendFormat("Subject = s, Predicate = \"<{0}>\", Object = obj.{1}.ToTripleObject()", ppInfo.Predicate, ppInfo.Property.Name);
                sb.AppendLine(" });");
            }

            var relationGenerator = _transform == null
                ? new RelationSourceCodeGenerator(_type)
                : new RelationSourceCodeGenerator(_transform);

            relationGenerator.AppendConversionScript(sb);

            return sb.ToString();
        }

        private string GetRdfSubjectTemplate()
        {
            var subjectProperty = GetNameOfSubjectProperty(_type);
            return subjectProperty.Value;
        }

        private KeyValuePair<string, string> GetNameOfSubjectProperty(Type type)
        {
            if (_transform != null)
            {
                var subjSpec = _transform.SubjectSpecification;
                return new KeyValuePair<string, string>(subjSpec.Property.Name, subjSpec.Template);
            }

            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            string template;
            if (property == null)
            {
                var classAttrib = (RdfSubjectOnClassAttribute) Attribute.GetCustomAttribute(type, typeof (RdfSubjectOnClassAttribute));
                property = type.GetProperty(classAttrib.Property, BindingFlags.Instance | BindingFlags.Public);
                template = classAttrib.Template;
            }
            else
            {
                template = ((RdfSubjectAttribute)Attribute.GetCustomAttribute(property, typeof(RdfSubjectAttribute))).Template;
            }


            return new KeyValuePair<string, string>(property.Name, template);
        }

        private Type GetTypeOfSubjectProperty(Type type)
        {
            if (_transform != null)
            {
                return _transform.SubjectSpecification.Property.PropertyType;
            }

            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            if (property == null)
            {
                var classAttrib = (RdfSubjectOnClassAttribute)Attribute.GetCustomAttribute(type, typeof (RdfSubjectOnClassAttribute));
                property = type.GetProperty(classAttrib.Property, BindingFlags.Instance | BindingFlags.Public);
            }

            return property.PropertyType;
        }

        private IEnumerable<IPropertyPredicateSpecification> GetRdfProperties()
        {
            if (_transform != null)
            {
                return _transform.PropertySpecifications;
            }

            var result = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.GetCustomAttribute(p, typeof(RdfPropertyAttribute)) != null)
                .Aggregate(
                    new List<IPropertyPredicateSpecification>(),
                    (list, p) =>
                    {
                        var attrib = (RdfPropertyAttribute)Attribute.GetCustomAttribute(p, typeof(RdfPropertyAttribute));
                        attrib.SetProperty(p);
                        list.Add(attrib);
                        return list;
                    });

            var classAttrib = (RdfPropertyOnClassAttribute)Attribute.GetCustomAttribute(_type, typeof(RdfPropertyOnClassAttribute));
            if (classAttrib != null)
            {
                result.AddRange(classAttrib.GetPropertyPredicateSpecifications(_type));
            }

            return result;
        }
    }
}
