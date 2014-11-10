using System.Collections.Generic;

namespace NetTriple.Tests.TestDomain
{
    public abstract class AnimalBase
    {
        public string Name { get; set; }
        public IEnumerable<AnimalBase> Enemies { get; set; } 
    }
}
