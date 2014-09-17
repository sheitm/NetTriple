using System.Collections.Generic;

namespace NetTriple.Tests.TestDomain
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Match> Matches { get; set; }
        public Player[] Players { get; set; }
    }
}
