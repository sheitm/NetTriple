using System.Collections.Generic;
using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/Order")]
    public class Order
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/order/{0}")]
        public string Id { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/order/ordernumber")]
        public int OrderNumber { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/order/description")]
        public string Description { get; set; }

        [RdfChildren(Inverse = true, Predicate = "http://netriple.com/elements/owning_order")]
        public IEnumerable<OrderDetail> Details { get; set; }
    }
}
