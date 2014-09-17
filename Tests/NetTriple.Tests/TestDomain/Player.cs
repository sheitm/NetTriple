using System;

namespace NetTriple.Tests.TestDomain
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
