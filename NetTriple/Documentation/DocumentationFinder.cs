using System.Collections.Generic;
using System.Linq;

namespace NetTriple.Documentation
{
    public static class DocumentationFinder
    {
        public static IEnumerable<TypeTransformDocumentation> GetTypeDocumentation()
        {
            var docs = LoadAllRdfClasses.GetDeclaredTransforms().Select(dt => new TypeTransformDocumentation(dt)).ToList();

            foreach (var doc in docs)
            {
                doc.SetContext(docs);
            }

            return docs;
        }
    }
}
