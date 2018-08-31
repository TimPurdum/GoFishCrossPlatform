namespace GoFish.Business
{
    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }


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