using System;
using System.Collections.Generic;
using System.Linq;

namespace NetTriple.Emit
{
    public class InflationContext : IInflationContext
    {
        private readonly Dictionary<string, object> _map = new Dictionary<string, object>();
        private readonly Dictionary<string, List<object>> _parentRefs = new Dictionary<string, List<object>>();

        public void Add(string subject, object obj)
        {
            _map.Add(subject, obj);
        }

        public void AddParentReference(string subjectOfParent, object childObject)
        {
            List<object> refs;
            if (_parentRefs.ContainsKey(subjectOfParent))
            {
                refs = _parentRefs[subjectOfParent];
            }
            else
            {
                refs = new List<object>();
                _parentRefs[subjectOfParent] = refs;
            }

            refs.Add(childObject);
        }

        public void LinkAll(IConverterLocator locator)
        {
            //foreach (var obj in _map.Values)
            //{
            //    locator.GetConverter(obj.GetType()).Link(obj, this);
            //}
        }

        public T GetFirstOfType<T>()
        {
            return (T)GetFirstOfType(typeof(T));
        }

        public object GetFirstOfType(Type type)
        {
            return _map.Values.SingleOrDefault(v => v.GetType() == type);
        }
    }
}
