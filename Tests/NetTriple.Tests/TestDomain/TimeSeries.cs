using System.Collections.Generic;

namespace NetTriple.Tests.TestDomain
{
    public class TimeSeries
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PointInTime> Points { get; set; } 
    }
}
