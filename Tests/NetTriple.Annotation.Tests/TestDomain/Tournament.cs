using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTriple.Annotation.Tests.TestDomain
{
    public class Tournament
    {
        public string Name { get; set; }
        public IEnumerable<Match> Matches { get; set; }
        public Player[] Players { get; set; }
    }
}
