using System;

namespace NetTriple.Emit
{
    public interface IConverterLocator
    {
        IConverter GetConverter(Type type);
        IConverter GetConverter(string subject);
    }
}
