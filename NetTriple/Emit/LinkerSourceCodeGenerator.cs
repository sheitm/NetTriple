using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetTriple.Emit
{
    /// <summary>
    /// Takes a source object, the name of a property on the source
    /// and a target object as input. Figures out how to link the
    /// to together.
    /// </summary>
    public class LinkerSourceCodeGenerator
    {
        private readonly Type _type;
        private readonly PropertyInfo _property;

        public LinkerSourceCodeGenerator(Type type, string propertyName)
        {
            _type = type;
            _property = _type.GetProperty(propertyName);
        }

        public string GetSourceCode()
        {
            return typeof(IEnumerable).IsAssignableFrom(_property.PropertyType)
                ? GetEnumerableSourceCode()
                : GetUnarySourceCode();
        }

        private string GetUnarySourceCode()
        {
            return string.Empty;
            //throw new NotImplementedException();
        }

        private string GetEnumerableSourceCode()
        {
            return string.Empty;
            //throw new NotImplementedException();
        }
    }
}
