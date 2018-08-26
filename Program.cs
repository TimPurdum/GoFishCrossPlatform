using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;


namespace GoFish
{
    class Program
    {
        static readonly Deck Deck = new Deck();
        static readonly Player AI = new Player();
        static readonly Player Player = new Player();
        static void Main()
        {
            Console.WriteLine("Let's play Go Fish!");
            Console.WriteLine("Type the letter 's' to shuffle the deck.");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Console.WriteLine("Shuffling...");
                Deck.Shuffle();
                Console.WriteLine();
                Deck.Deal(new List<Player>{Player, AI}, 7);

                while (Player.Hand.Count > 0 && AI.Hand.Count > 0 && Deck.Count > 0)
                {
                    ShowHand();
                    Guess();
                    AIGuess();
                }

                GameOver();
            }
        }

        static void ShowHand()
        {
            Console.WriteLine("Here is your hand:");
            foreach (var card in Player.Hand)
            {
                Console.WriteLine($" - {card.Rank} of {card.Suit}");
            }
            
        }


        static void Guess()
        {
            if (Player.Hand.Count == 0)
            {
                Console.WriteLine("No cards, you must draw this turn!");
                Deck.Draw(Player, 1);
                return;
            }
            
            Console.WriteLine();
            Console.WriteLine("Ask me if I have a card...");
            Console.WriteLine("(e.g., Ace, Two, Three, Four, King...)");
            var guess = Console.ReadLine();

            while (Player.Hand.All(c => c.Rank.ToString().ToLower() != guess?.ToLower()))
            {
                Console.WriteLine("You may only guess card numbers that you already have.");
                Console.WriteLine("Ask me if I have a card...");
                guess = Console.ReadLine();
            }
            
            Console.WriteLine();
            var cards = FindCards(guess, AI);
            
            if (cards != null)
            {
                var message = $"You got {cards.Count} {cards.First().Rank}!";
                if (cards.Count > 1)
                {
                    message = $"You got {cards.Count} {cards.First().PluralName}!";
                }
                Console.WriteLine(message);
                
                Player.Hand.AddRange(cards);
                foreach (var c in cards)
                {
                    AI.Hand.Remove(c);
                }
                LayoutSets(Player);
                Guess();
            }
            else
            {
                Console.WriteLine("No! Go Fish!");
                Thread.Sleep(1000);
                Deck.Draw(Player, 1);
                var newCard = Player.Hand.Last();
                Console.WriteLine($"You drew the {newCard.Rank} of {newCard.Suit}");
                LayoutSets(Player);
            }
        }


        static void AIGuess()
        {
            if (AI.Hand.Count == 0)
            {
                Console.WriteLine("No cards, I must draw this turn!");
                Deck.Draw(AI, 1);
                return;
            }
            
            var guessDeck = new Deck();
            guessDeck.Shuffle();
            var guessCard = guessDeck.Peek();
            while (AI.Hand.All(c => c.Rank != guessCard.Rank))
            {
                guessDeck.Remove(guessCard);
                guessDeck.Shuffle();
                guessCard = guessDeck.Peek();
            }
            
            Console.WriteLine("My turn.");
            
            Console.WriteLine($"Do you have any {guessCard.PluralName}?");
            Thread.Sleep(2000);
            Console.WriteLine();
            var cards = FindCards(guessCard.Rank.ToString(), Player);
            
            if (cards != null)
            {
                if (cards.Count == 1)
                {
                    Console.WriteLine($"Yes? I'll take that {guessCard.Rank}!");
                }
                else
                {
                    Console.WriteLine($"Yes? I'll take those {guessCard.PluralName}!");
                }
                
                AI.Hand.AddRange(cards);
                foreach (var c in cards)
                {
                    Player.Hand.Remove(c);
                }
                LayoutSets(AI);
                AIGuess();
            }
            else
            {
                Console.WriteLine("No? I have to draw...");
                Thread.Sleep(1000);
                Deck.Draw(AI, 1);
                LayoutSets(AI);
            }
        }


        static List<Card> FindCards(string guess, Player p)
        {
            var cardGuess = guess.Trim().ToLower();

            if (p.Hand.Any(c => c.Rank.ToString().ToLower() == cardGuess))
            {
                return p.Hand.Where(c => c.Rank.ToString().ToLower() == cardGuess).ToList();
            }
           
            return null;
        }
        
        
        static void LayoutSets(Player p)
        {
            Console.WriteLine();
            
            var numberGroups = p.Hand.GroupBy(c => c.Rank);
            foreach (var numberGroup in numberGroups)
            {
                if (numberGroup.Count() == 4)
                {
                    var numberCards = numberGroup.ToList();
                    p.Sets.Add(numberCards);
                    foreach (var c in numberCards)
                    {
                        p.Hand.Remove(c);
                    }
                }
            }
            
            if (p.Sets.Count == 0)
            {
                return;
            }

            ShowSets(p);
        }


        static void ShowSets(Player p)
        {
            if (p == Player)
            {
                Console.WriteLine("You have these sets:");
            }
            else
            {
                Console.WriteLine("I have these sets:");
            }
            
            foreach (var set in p.Sets)
            {
                Console.WriteLine($" - {set.First().PluralName}");
            }

            if (p == Player)
            {
                ShowHand();
            }
        }


        static void GameOver()
        {
            Console.WriteLine("Game Over!");
            var playerPoints = 0;
            Player.Sets.ForEach(set => playerPoints += set.Count);
            Console.WriteLine($"Your score is {playerPoints}");
            var aiPoints = 0;
            AI.Sets.ForEach(set => aiPoints += set.Count);
            Console.WriteLine($"My score is {aiPoints}");
            if (playerPoints > aiPoints)
            {
                Console.WriteLine("You Win!");
            }
            else if (aiPoints > playerPoints)
            {
                Console.WriteLine("I Win!");
            }
            else
            {
                Console.WriteLine("Tie Game!");
            }
        }
    }
}
