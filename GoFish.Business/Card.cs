using System;


namespace GoFish.Business
{
    public class Card
    {
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }


        public Suit Suit { get; }
        public Rank Rank { get; }


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


        public string RankShortName
        {
            get
            {
                switch (Rank)
                {
                    case Rank.Ace:
                        return "A ";
                    case Rank.King:
                        return "K ";
                    case Rank.Queen:
                        return "Q ";
                    case Rank.Jack:
                        return "J ";
                    case Rank.Ten:
                        return "10";
                    default:
                        return $"{((int)Rank).ToString()} ";
                }
            }
        }


        public char SuitSymbol
        {
            get
            {
                switch (Suit)
                {
                    case Suit.Clubs:
                        return '♣';
                    case Suit.Diamonds:
                        return '♢';
                    case Suit.Hearts:
                        return '♡';
                    case Suit.Spades:
                        return '♠';
                }

                throw new Exception("No Suit found");
            }
        }
    }
}