using System;
using System.Collections.Generic;

namespace NetTriple.Emit
{
    public interface IInflationContext
    {
        void Add(string subject, object obj);
        void AddParentReference(string subjectOfParent, object childObject);

        void LinkAll(IConverterLocator locator);

        T GetFirstOfType<T>();
        object GetFirstOfType(Type type);

        T Get<T>(string sourceSubject, string predicate);
        IEnumerable<T> GetAll<T>(string sourceSubject, string predicate);
    }
}
