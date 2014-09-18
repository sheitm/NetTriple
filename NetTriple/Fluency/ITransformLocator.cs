using System;

namespace NetTriple.Annotation.Fluency
{
    public interface ITransformLocator
    {
        IBuiltTransform GetTransform(Type type);
    }
}
