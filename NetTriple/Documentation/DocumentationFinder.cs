using System.Collections.Generic;
using System.Linq;

namespace NetTriple.Documentation
{
    public static class DocumentationFinder
    {
        public static IEnumerable<TypeTransformDocumentation> GetTypeDocumentation()
        {
            return LoadAllRdfClasses.GetDeclaredTransforms().Select(dt => new TypeTransformDocumentation(dt));
        }
    }
}
