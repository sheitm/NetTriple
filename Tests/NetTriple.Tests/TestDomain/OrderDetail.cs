using NetTriple.Annotation;

namespace NetTriple.Tests.TestDomain
{
    [RdfType(Predicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", Value = "http://netriple.com/unittesting/OrderDetail")]
    public class OrderDetail
    {
        [RdfSubject(Template = "http://netriple.com/unittesting/orderdetail/{0}")]
        public string Id { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/order/orderdetailnumber")]
        public int OrderDetailNumber { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/order/description")]
        public string Description { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/order/quantity")]
        public int Quantity { get; set; }

        [RdfProperty(Predicate = "http://netriple.com/unittesting/order/productcode")]
        public string ProductCode { get; set; }
    }
}
