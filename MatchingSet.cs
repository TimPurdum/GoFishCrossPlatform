using System.Collections.Generic;


namespace GoFish
{
    public class MatchingSet
    {
        public List<Card> Cards { get; } = new List<Card>();


        public MatchingSet(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);
        }


        public void AddCard(Card newCard)
        {
            Cards.Add(newCard);
        }
    }
}