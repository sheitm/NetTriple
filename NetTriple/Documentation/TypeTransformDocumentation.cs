﻿using System.Collections.Generic;
using System.Linq;
using NetTriple.Fluency;

namespace NetTriple.Documentation
{
    public class TypeTransformDocumentation
    {
        public TypeTransformDocumentation() { }

        public TypeTransformDocumentation(IBuiltTransform builtTransform)
        {
            RdfType = builtTransform.TypeString;
            Type = builtTransform.Type.FullName;
            Properties = builtTransform.PropertySpecifications.Select(ps => new PropertyTransformDocumention(ps));
            Subject = new SubjectTransformDocumentation(builtTransform.SubjectSpecification);
            Relations = builtTransform.RelationSpecifications.Select(rs => new RelationTransformDocumentation(rs));
        }

        public string RdfType { get; set; }
        public string Type { get; set; }
        public IEnumerable<PropertyTransformDocumention> Properties { get; set; }
        public IEnumerable<RelationTransformDocumentation> Relations { get; set; } 
        public SubjectTransformDocumentation Subject { get; set; }
    }
}
