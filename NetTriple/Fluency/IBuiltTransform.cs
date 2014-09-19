using System;
using System.Collections.Generic;
using NetTriple.Annotation;

namespace NetTriple.Fluency
{
    public interface IBuiltTransform
    {
        Type Type { get; }
        string TypeString { get; }
        string TypePredicate { get; }

        IEnumerable<IPropertyPredicateSpecification> PropertySpecifications { get; }
        ISubjectSpecification SubjectSpecification { get; }
        IEnumerable<IChildrenPredicateSpecification> RelationSpecifications { get; }
        IEnumerable<StructureTransform> StructureTransforms { get; }

        void SetLocator(ITransformLocator locator);

        IBuiltTransform GetRelatedTransform(Type type);
    }
}
