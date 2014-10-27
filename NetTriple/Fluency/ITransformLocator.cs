using System;
using System.Collections.Generic;

namespace NetTriple.Fluency
{
    public interface ITransformLocator
    {
        IBuiltTransform GetTransform(Type type);
        IEnumerable<IBuiltTransform> GetSubclassTransforms(Type type);
    }
}
