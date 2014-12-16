using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTriple.Fluency;

namespace NetTriple.Documentation
{
    public class StructureTransformDocumentation
    {
        public StructureTransformDocumentation(StructureTransform transform)
        {
            Elements = transform.Elements;
            IsEnumerable = transform.IsEnumerable;
            Predicate = transform.Predicate;
            PropertyName = transform.Property.Name;
            PropertyType = transform.Property.PropertyType.Name;
        }

        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public bool IsEnumerable { get; set; }
        public string Predicate { get; set; }
        public IEnumerable<StructureTransformElement> Elements { get; set; }

        public string GetSample()
        {
            var started = false;
            return Elements.OrderBy(e => e.Index).Aggregate(
                new StringBuilder(),
                (sb, element) =>
                {
                    if (started)
                    {
                        sb.Append(", ");
                    }

                    started = true;

                    return sb;
                }).ToString();
        }
    }
}
