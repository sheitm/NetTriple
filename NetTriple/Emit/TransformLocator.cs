using System;
using System.Collections.Generic;
using System.Linq;
using NetTriple.Fluency;

namespace NetTriple.Emit
{
    public class TransformLocator : ITransformLocator
    {
        private readonly List<IBuiltTransform> _transforms = new List<IBuiltTransform>();

        public TransformLocator(IEnumerable<IBuiltTransform> transforms)
        {
            _transforms.AddRange(transforms);
        }

        public IBuiltTransform GetTransform(Type type)
        {
            return _transforms.SingleOrDefault(t => t.Type == type);
        }
    }
}
