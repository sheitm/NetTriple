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
                .Replace("##INFLATION##", GetInflationScript())
                .Replace("##RDFSUBJECT##", GetRdfSubjectTemplate().ToWashedRdfSubject())
                .Replace("##LINK##", GetLinkInvocationScript())
                .Replace("##LINKCORE##", GetLinkScript());
        }

        private string GetLinkInvocationScript()
        {
            return string.Format("LinkCore(({0}) obj, context);\r\n", _type.FullName);
        }

        private string GetLinkScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("private void LinkCore({0} obj, IInflationContext context)\r\n", _type.FullName);
            sb.AppendLine("{");

            var started = false;
            foreach (var pair in RelationSourceCodeGenerator.GetChildProperties(_type))
            {
                var linkGenerator = new LinkerSourceCodeGenerator(_type, pair.Key, pair.Value);
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

            var subjectProperty = GetNameOfSubjectProperty(_type);
            sb.AppendFormat("var s1 = obj.{0}.ToString();\r\n", subjectProperty.Key);
            sb.AppendFormat("var template = \"<{0}>\";\r\n", subjectProperty.Value);
            sb.Append("var s = template.Replace(\"{0}\", s1);\r\n");

            sb.Append("triples.Add(new Triple { ");
            sb.AppendFormat("Subject = s, Predicate = \"<{0}>\", Object = \"<{1}>\"", _rdfTypeAttribute.Predicate, _rdfTypeAttribute.Value);
            sb.AppendLine(" });");

            foreach (var ppInfo in GetRdfProperties())
            {
                sb.Append("triples.Add(new Triple { ");
                sb.AppendFormat("Subject = s, Predicate = \"<{0}>\", Object = obj.{1}.ToTripleObject()", ppInfo.Predicate, ppInfo.Property.Name);
                sb.AppendLine(" });");
            }

            var relationGenerator = new RelationSourceCodeGenerator(_type);
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

        private IEnumerable<IPropertyPredicateSpecification> GetRdfProperties()
        {
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

            var classAttrib = Attribute.GetCustomAttribute(_type, typeof(RdfPropertyOnClassAttribute));
            if (classAttrib != null)
            {
                
            }

            return result;
        }
    }
}
