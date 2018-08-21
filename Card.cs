namespace GoFish
{
    public struct Card
    {
        public Suit Suit;
        public Rank Rank;
        
        public Card (Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }


        public string PluralName
        {
            get
            {
                if (Rank == Rank.Six)
                {
                    return Rank + "es";
                }

                return Rank + "s";   
            }
        }
    }
}