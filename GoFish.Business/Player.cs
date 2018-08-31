using System.Collections.Generic;


namespace GoFish.Business
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        
        public List<List<Card>> Sets { get; } = new List<List<Card>>();
    }
}
