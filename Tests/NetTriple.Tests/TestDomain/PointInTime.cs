using System;
using NodaTime;

namespace NetTriple.Tests.TestDomain
{
    public class PointInTime
    {
        public string Id { get; set; }
        public Instant Instant { get; set; }
    }
}
