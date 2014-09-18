using System;
using System.Collections.Generic;

namespace NetTriple.Annotation.Fluency
{
    public interface IBuiltTransform
    {
        Type Type { get; }
        string TypeString { get; }
        string TypePredicate { get; }

        IEnumerable<IPropertyPredicateSpecification> PropertySpecifications { get; }
        ISubjectSpecification SubjectSpecification { get; }
        IEnumerable<IChildrenPredicateSpecification> RelationSpecifications { get; }

        void SetLocator(ITransformLocator locator);

        IBuiltTransform GetRelatedTransform(Type type);
    }
}
