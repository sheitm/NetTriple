    using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetTriple.Annotation;

namespace NetTriple.Emit
{
    public class SourceCodeGenerator
    {
        private readonly Type _type;
        private readonly RdfTypeAttribute _rdfTypeAttribute;

        public SourceCodeGenerator(Type type)
        {
            _type = type;
            _rdfTypeAttribute = (RdfTypeAttribute)Attribute.GetCustomAttribute(_type, typeof(RdfTypeAttribute));
        }

        public string GetSourceCode()
        {
            return TemplateResources.ConverterTemplate
                .Replace("##CLASS##", _type.Name)
                .Replace("##FULLCLASS##", _type.FullName)
                .Replace("##CONVERSION##", GetConversionScript())
                .Replace("##INFLATION##", GetInflationScript());
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

            foreach (var pair in GetRdfProperties())
            {
                var prop = _type.GetProperty(pair.Key);

                sb.AppendFormat("triple = triples.SingleOrDefault(t => t.Predicate == \"<{0}>\");\r\n", pair.Value.Predicate);
                sb.Append("if (triple != null) { obj.");
                sb.AppendFormat("{0} = triple.GetObject<{1}>(); ", pair.Key, prop.PropertyType.FullName);
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private string GetConversionScript()
        {
            var sb = new StringBuilder();

            var subjectProperty = GetNameOfSubjectProperty(_type);
            sb.AppendFormat("var s1 = obj.{0}.ToString();\r\n", subjectProperty.Key);
            sb.AppendFormat("var template = \"<{0}>\";\r\n", subjectProperty.Value);
            sb.Append("var s = template.Replace(\"{0}\", s1);\r\n");

            sb.Append("triples.Add(new Triple { ");
            sb.AppendFormat("Subject = s, Predicate = \"<{0}>\", Object = \"<{1}>\"", _rdfTypeAttribute.Predicate, _rdfTypeAttribute.Value);
            sb.AppendLine(" });");

            foreach (var pair in GetRdfProperties())
            {
                sb.Append("triples.Add(new Triple { ");
                sb.AppendFormat("Subject = s, Predicate = \"<{0}>\", Object = obj.{1}.ToTripleObject()", pair.Value.Predicate, pair.Key);
                sb.AppendLine(" });");
            }

            var relationGenerator = new RelationSourceCodeGenerator(_type);
            relationGenerator.AppendConversionScript(sb);

            return sb.ToString();
        }

        private KeyValuePair<string, string> GetNameOfSubjectProperty(Type type)
        {
            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            var attrib = (RdfSubjectAttribute)Attribute.GetCustomAttribute(property, typeof(RdfSubjectAttribute));

            return new KeyValuePair<string, string>(property.Name, attrib.Template);
        }

        private Type GetTypeOfSubjectProperty(Type type)
        {
            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            return property.PropertyType;
        }

        private IEnumerable<KeyValuePair<string, RdfPropertyAttribute>> GetRdfProperties()
        {
            return _type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.GetCustomAttribute(p, typeof(RdfPropertyAttribute)) != null)
                .Aggregate(
                    new Dictionary<string, RdfPropertyAttribute>(),
                    (map, p) =>
                    {
                        var attrib = (RdfPropertyAttribute)Attribute.GetCustomAttribute(p, typeof(RdfPropertyAttribute));
                        map[p.Name] = attrib;
                        return map;
                    });
        }
    }
}
