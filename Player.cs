using System;
using System.Collections.Generic;
using System.Text;

namespace GoFish
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        public bool AI { get; set; }
        public string Name { get; set; } = names[new Random().Next(0, 5)];
        public List<MatchingSet> Sets { get; set; } = new List<MatchingSet>();

        static readonly string[] names =
        {
            "Lisa",
            "Marco",
            "Lee",
            "Olivia",
            "Ahmad",
            "Anastasia"
        };
    }
}
