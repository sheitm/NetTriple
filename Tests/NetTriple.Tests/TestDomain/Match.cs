using System;

namespace NetTriple.Tests.TestDomain
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
    }
}
