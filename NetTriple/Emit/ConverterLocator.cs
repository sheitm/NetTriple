using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTriple.Emit
{
    public class ConverterLocator : IConverterLocator
    {
        private readonly IDictionary<Type, IConverter> _map;
        private readonly IDictionary<string, Type> _subjectMap;

        public ConverterLocator(IDictionary<Type, IConverter> map, IDictionary<string, Type> subjectMap)
        {
            _map = map;
            _subjectMap = subjectMap;
        }

        public IConverter GetConverter(Type type)
        {
            return _map[type];
        }

        public IConverter GetConverter(string subject)
        {
            var key = _subjectMap.Keys.SingleOrDefault(k => k.StartsWith(subject));
            return key == null
                ? null
                : GetConverter(_subjectMap[key]);
        }
    }
}
