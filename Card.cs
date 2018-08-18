namespace GoFish
{
    public struct Card
    {
        public Suit Suit;
        public int Number;
        public string Name;
        
        public Card (Suit suit, int number, string name)
        {
            Suit = suit;
            Number = number;
            Name = name;
        }
    }
}