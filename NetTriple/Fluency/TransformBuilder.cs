using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NetTriple.Annotation;
using NetTriple.Annotation.Internal;

namespace NetTriple.Fluency
{
    public class TransformBuilder<T> : IBuiltTransform
    {
        private string _propertyPredicateBase;
        private readonly List<IPropertyPredicateSpecification> _properties = new List<IPropertyPredicateSpecification>();
        private readonly List<IChildrenPredicateSpecification> _relations = new List<IChildrenPredicateSpecification>();
        private readonly List<StructureTransform> _structureTransforms = new List<StructureTransform>(); 
        private ITransformLocator _locator;

        public TransformBuilder(string typeString, string typePredicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type")
        {
            Type = typeof(T);
            TypeString = typeString;
            TypePredicate = typePredicate;
            //[RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Order")]
        }

        public Type Type { get; private set; }
        public string TypeString { get; private set; }
        public string TypePredicate { get; private set; }

        public IEnumerable<IPropertyPredicateSpecification> PropertySpecifications
        {
            get
            {
                foreach (var property in _properties)
                {
                    yield return property;
                }
            }
        }

        public IEnumerable<StructureTransform> StructureTransforms
        {
            get
            {
                foreach (var structureTransform in _structureTransforms)
                {
                    yield return structureTransform;
                }
            }
        } 

        public ISubjectSpecification SubjectSpecification { get; private set; }

        public IEnumerable<IChildrenPredicateSpecification> RelationSpecifications
        {
            get
            {
                foreach (var relation in _relations)
                {
                    yield return relation;
                }
            }
        }

        public void SetLocator(ITransformLocator locator)
        {
            _locator = locator;
        }

        public IBuiltTransform GetRelatedTransform(Type type)
        {
            return _locator.GetTransform(type);
        }

        public TransformBuilder<T> WithPropertyPredicateBase(string predicateBase)
        {
            _propertyPredicateBase = predicateBase;
            return this;
        }

        public TransformBuilder<T> Subject(Expression<Func<T, object>> accessor, string template)
        {
            if (accessor == null)
            {
                throw new ArgumentNullException("accessor");
            }

            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException("template");
            }

            var property = ReflectionHelper.FindProperty(accessor);
            SubjectSpecification = new SubjectSpecification(template, (PropertyInfo)property);

            return this;
        }

        public TransformBuilder<T> Prop(Expression<Func<T, object>> accessor, string predicate)
        {
            if (accessor == null)
            {
                throw new ArgumentNullException("accessor");
            }

            if (string.IsNullOrWhiteSpace(predicate))
            {
                throw new ArgumentNullException("predicate");
            }

            var property = ReflectionHelper.FindProperty(accessor);
            var pred = string.IsNullOrEmpty(_propertyPredicateBase) ? predicate : string.Format("{0}{1}", _propertyPredicateBase, predicate);

            _properties.Add(new PropertyPredicateSpecification(pred, (PropertyInfo)property));

            return this;
        }

        public TransformBuilder<T> Relation(Expression<Func<T, object>> accessor, string predicate, bool inverse = false)
        {
            if (accessor == null)
            {
                throw new ArgumentNullException("accessor");
            }

            if (string.IsNullOrWhiteSpace(predicate))
            {
                throw new ArgumentNullException("predicate");
            }

            var property = ReflectionHelper.FindProperty(accessor);
            var pred = string.IsNullOrEmpty(_propertyPredicateBase) ? predicate : string.Format("{0}{1}", _propertyPredicateBase, predicate);
            _relations.Add(new ChildrenPredicateSpecification((PropertyInfo)property, pred, inverse));

            return this;
        }

        public TransformBuilder<T> Struct<TRelationType>(Expression<Func<T, object>> accessor, string predicate, params Expression<Func<TRelationType, object>>[] elements)
        {
            var pred = string.IsNullOrEmpty(_propertyPredicateBase) ? predicate : string.Format("{0}{1}", _propertyPredicateBase, predicate);
            var transform = new StructureTransform((PropertyInfo)ReflectionHelper.FindProperty(accessor), pred)
            {
                Elements =
                    elements.Select(
                        (t, i) =>
                            new StructureTransformElement
                            {
                                Index = i,
                                Property = (PropertyInfo) ReflectionHelper.FindProperty(t)
                            }).ToList()
            };

            _structureTransforms.Add(transform);

            return this;
        } 
    }
}
