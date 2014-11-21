using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NetTriple.Fluency;

namespace NetTriple.Documentation
{
    public class TypeTransformDocumentation
    {
        // [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ams.embriq.no/types/asset_2")]
        // [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        // [System.Xml.Serialization.XmlAttributeAttribute()]
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
