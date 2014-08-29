using System;

namespace NetTriple.Emit
{
    public interface IInflationContext
    {
        void Add(string subject, object obj);
        void AddParentReference(string subjectOfParent, object childObject);

        void LinkAll(IConverterLocator locator);

        T GetFirstOfType<T>();
        object GetFirstOfType(Type type);
    }
}
