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
            var subjectPair = Subject.GetSampleSubject();

            sb.AppendFormat("<{0}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <{1}> . \r\n", 
                subjectPair.Value,
                RdfType);

            foreach (var property in Properties)
            {
                var v = property.PropertyName == Subject.PropertyName
                    ? GetSampleValue(property.GetPropertyType(), subjectPair.Key)
                    : GetSampleValue(property.GetPropertyType());
                //var v = property.PropertyName == Subject.PropertyName
                //    ? subjectPair.Key
                //    : typeof(int) == property.GetPropertyType()
                //        ? Rand.Next().ToString()
                //        : Guid.NewGuid().ToString().Substring(0, 8);

                sb.AppendFormat("<{0}> <{1}> {2} .\r\n", subjectPair.Value, property.Predicate, v);
            }

            return sb.ToString();
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
    }
}
