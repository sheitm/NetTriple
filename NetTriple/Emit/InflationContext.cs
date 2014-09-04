using System;
using System.Collections.Generic;
using System.Linq;

namespace NetTriple.Emit
{
    public class InflationContext : IInflationContext
    {
        private readonly Dictionary<string, object> _map = new Dictionary<string, object>();
        private readonly Dictionary<string, List<object>> _parentRefs = new Dictionary<string, List<object>>();
        private readonly IEnumerable<Triple> _allTriples;

        public InflationContext(IEnumerable<Triple> allTriples)
        {
            _allTriples = allTriples;
        }

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
            foreach (var obj in _map.Values)
            {
                locator.GetConverter(obj.GetType()).Link(obj, this);
            }
        }

        public T GetFirstOfType<T>()
        {
            return (T)GetFirstOfType(typeof(T));
        }

        public object GetFirstOfType(Type type)
        {
            return _map.Values.SingleOrDefault(v => v.GetType() == type);
        }

        public T Get<T>(string sourceSubject, string predicate)
        {
            var triple = _allTriples.SingleOrDefault(t => t.IsMatch(sourceSubject, predicate));
            if (triple == null)
            {
                return default(T);
            }

            return _map.ContainsKey(triple.Object)
                ? (T) _map[triple.Object]
                : default(T);
        }

        public IEnumerable<T> GetAll<T>(string sourceSubject, string predicate)
        {
            throw new NotImplementedException();
        }
    }
}
