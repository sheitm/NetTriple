using System;
using System.Collections.Generic;

namespace NetTriple.Fluency
{
    public class ReflectionBuildContext : ITransformLocator
    {
        private readonly Dictionary<Type, IBuiltTransform> _transforms = new Dictionary<Type, IBuiltTransform>();

        public IEnumerable<IBuiltTransform> Transforms
        {
            get { return _transforms.Values; }
        }

        public IBuiltTransform GetTransform(Type type)
        {
            if (!_transforms.ContainsKey(type))
            {
                return null;
            }

            return _transforms[type];
        }

        public IEnumerable<IBuiltTransform> GetSubclassTransforms(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
