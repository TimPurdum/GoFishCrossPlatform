using System;
using System.Collections.Generic;
using System.Text;

namespace GoFish
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        
        public List<MatchingSet> Sets { get; } = new List<MatchingSet>();
    }
}
