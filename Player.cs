using System.Collections.Generic;

namespace GoFish
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        
        public List<List<Card>> Sets { get; } = new List<List<Card>>();
    }
}
