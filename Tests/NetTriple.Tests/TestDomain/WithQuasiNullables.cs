using System;

namespace NetTriple.Tests.TestDomain
{
    public class WithQuasiNullables
    {
        public int Id { get; set; }

        public int No { get; set; }
        public bool NoSpecified { get; set; }

        public DateTime Hour { get; set; }
        public bool HourSpecified { get; set; }

        public bool Ok { get; set; }
        public bool OkSpecified { get; set; }
    }
}
