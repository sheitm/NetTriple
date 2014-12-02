using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NetTriple.Fluency;

namespace NetTriple.Documentation
{
    public class TypeTransformDocumentation
    {
        private static readonly Random Rand = new Random();

        private IEnumerable<TypeTransformDocumentation> _docs;

        public TypeTransformDocumentation() { }

        public TypeTransformDocumentation(IBuiltTransform builtTransform)
        {
            RdfType = builtTransform.TypeString;
            Type = builtTransform.Type.FullName;
            Properties = builtTransform.PropertySpecifications.Select(ps => new PropertyTransformDocumention(ps));
            Subject = new SubjectTransformDocumentation(builtTransform.SubjectSpecification);
            Relations = builtTransform.RelationSpecifications.Select(rs => new RelationTransformDocumentation(rs));

            SetXmlData(builtTransform);
        }

        public string RdfType { get; set; }
        public string Type { get; set; }
        public IEnumerable<PropertyTransformDocumention> Properties { get; set; }
        public IEnumerable<RelationTransformDocumentation> Relations { get; set; } 
        public SubjectTransformDocumentation Subject { get; set; }

        public string XmlNamespace { get; set; }

        public string GetSampleNTriples()
        {
            var sb = new StringBuilder();
            string s;
            AppendSampleNTriples(sb, out s);

            return sb.ToString();
        }

        public void AppendSampleNTriples(StringBuilder sb, out string subject)
        {
            var subjectPair = Subject.GetSampleSubject();
            subject = subjectPair.Value;

            sb.AppendFormat("<{0}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <{1}> . \r\n",
                subjectPair.Value,
                RdfType);

            foreach (var property in Properties)
            {
                var v = property.PropertyName == Subject.PropertyName
                    ? GetSampleValue(property.GetPropertyType(), subjectPair.Key)
                    : GetSampleValue(property.GetPropertyType());

                sb.AppendFormat("<{0}> <{1}> {2} .\r\n", subjectPair.Value, property.Predicate, v);
            }

            if (_docs != null && Relations != null)
            {
                foreach (var relation in Relations)
                {
                    var related = _docs.SingleOrDefault(d => d.Type == relation.PropertyType);
                    if (related != null)
                    {
                        string s;
                        related.AppendSampleNTriples(sb, out s);
                        sb.AppendFormat("<{0}> <{1}> <{2}> .\r\n", subjectPair.Value, relation.Predicate, s);
                    }
                }
            }
        }

        private string GetSampleValue(Type type, string v = null)
        {
            if (v == null)
            {
                if (typeof (string) == type)
                {
                    v = Guid.NewGuid().ToString().Substring(0, 8);
                }
                else if (typeof (DateTime) == type)
                {
                    var dt = DateTime.UtcNow.ToUniversalTime()
                        .ToString("yyyy-MM-ddTHH");
                    v = string.Format("\"{0}:00:00Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime>", dt);
                }
                else
                {
                    v = Rand.Next().ToString();
                }
            }

            if (typeof (string) == type)
            {
                return string.Format("\"{0}\"", v);
            }

            return v;
        }

        private void SetXmlData(IBuiltTransform builtTransform)
        {
            var attrib = Attribute.GetCustomAttribute(builtTransform.Type, typeof (XmlTypeAttribute));
            if (attrib != null)
            {
                XmlNamespace = ((XmlTypeAttribute) attrib).Namespace;
            }
        }

        public void SetContext(IEnumerable<TypeTransformDocumentation> docs)
        {
            _docs = docs;
        }
    }
}
