using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoFish
{
    public class Deck : List<Card>
    {
        readonly Suit[] suits =
        {
            Suit.Hearts,
            Suit.Clubs,
            Suit.Diamonds,
            Suit.Spades
        };

        public Deck()
        {
            for (var i = 0; i < 13; i++)
            {
                foreach (var suit in suits)
                {
                    Add(new Card(suit, (Rank)i));
                }
            }
        }


        public void Shuffle()
        {
            var rnd = new Random();
            for (var i = 0; i < Count - 1; i++)
            {
                Swap(i, rnd.Next(i, Count));
            }
        }


        public void Deal(List<Player> players, int numberOfCards)
        {
            foreach (var player in players)
            {
                player.Hand = new List<Card>();
                Draw(player, numberOfCards);
            }
        }


        public void Draw(Player player, int numberOfCards = 1)
        {
            for (var cardNum = 0; cardNum < numberOfCards; cardNum++)
            {
                player.Hand.Add(this.First());
                RemoveAt(0);
            }
        }


        public Card Peek()
        {
            return this.First();
        }


        void Swap(int a, int b)
        {
            var temp = this[a];
            this[a] = this[b];
            this[b] = temp;
        }
    }
}
