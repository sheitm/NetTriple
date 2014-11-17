using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTriple.Annotation;

namespace NetTriple.Fluency
{
    public class ReflectedBuiltTransform : IBuiltTransform
    {
        private const string Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        private ITransformLocator _locator;

        public ReflectedBuiltTransform(Type type, string rdfBase)
        {
            Type = type;
            TypeString = string.Format("{0}/{1}", rdfBase, type.Name.ToCamelCase());
            TypePredicate = Predicate;
        }

        public Type Type { get; private set; }
        public string TypeString { get; private set; }
        public string TypePredicate { get; private set; }
        public IEnumerable<IPropertyPredicateSpecification> PropertySpecifications { get; private set; }
        public ISubjectSpecification SubjectSpecification { get; private set; }
        public IEnumerable<IChildrenPredicateSpecification> RelationSpecifications { get; private set; }
        public IEnumerable<StructureTransform> StructureTransforms { get; private set; }

        public void ComputeStructure()
        {
            if (_locator == null)
            {
                throw new InvalidOperationException("Can't compute structure until locator is set.");
            }
        }

        public void SetLocator(ITransformLocator locator)
        {
            _locator = locator;
        }

        public IBuiltTransform GetRelatedTransform(Type type)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBuiltTransform> GetSubclassTransforms(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
