using System;

namespace NetTriple.Fluency
{
    public interface ITransformLocator
    {
        IBuiltTransform GetTransform(Type type);
    }
}
