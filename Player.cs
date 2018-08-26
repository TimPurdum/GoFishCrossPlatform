using System;
using System.Collections.Generic;
using System.Text;

namespace GoFish
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        
        public List<List<Card>> Sets { get; } = new List<List<Card>>();
    }
}
