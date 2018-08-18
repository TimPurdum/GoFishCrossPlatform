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
        static readonly Player AI = new Player(){AI = true};
        static readonly Player Player = new Player();
        static void Main(string[] args)
        {
            Console.WriteLine("Let's play Go Fish! What's your name?");
            Player.Name = Console.ReadLine();
            Console.WriteLine($"Welcome, {Player.Name}! My name is {AI.Name}.");
            Console.WriteLine("Type the letter 's' to shuffle the deck.");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Console.WriteLine("Shuffling...");
                Deck.Shuffle();

                Deck.Deal(new List<Player>{Player, AI}, 5);

                while (Player.Hand.Count > 0 && AI.Hand.Count > 0 && Deck.Count > 0)
                {
                    ShowHand();
                    Guess();
                    LayoutSets(Player);
                    AIGuess();
                    LayoutSets(AI);
                }

                GameOver();
            }
        }

        static void ShowHand()
        {
            Console.WriteLine();
            Console.WriteLine("Here is your hand:");
            foreach (var card in Player.Hand)
            {
                Console.WriteLine($" - {card.Name} of {card.Suit.Name}");
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
            Console.WriteLine();
            var card = FindCard(guess, AI);
            
            if (card.HasValue)
            {
                Console.WriteLine($"You got the {card.Value.Name} of {card.Value.Suit.Name}!");
                Player.Hand.Add(card.Value);
                AI.Hand.Remove(card.Value);
                Guess();
            }
            else
            {
                Console.WriteLine("No! Go Fish!");
                Deck.Draw(Player, 1);
                var newCard = Player.Hand.Last();
                Console.WriteLine($"You drew the {newCard.Name} of {newCard.Suit.Name}");
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
            Console.WriteLine();
            Console.WriteLine("My turn.");
            var guess = guessCard.Name == "Six" ? guessCard.Name + "es" : guessCard.Name + "s";
            Console.WriteLine($"Do you have any {guess}?");
            Thread.Sleep(2000);
            Console.WriteLine();
            var card = FindCard(guessCard.Name, Player);
            
            if (card.HasValue)
            {
                Console.WriteLine($"Yes? I'll take that {guessCard.Name}!");
                AI.Hand.Add(card.Value);
                Player.Hand.Remove(card.Value);
                AIGuess();
            }
            else
            {
                Console.WriteLine("No? I have to draw...");
                Deck.Draw(AI, 1);
            }
        }


        static Card? FindCard(string guess, Player p)
        {
            var cardGuess = guess.Trim();

            if (p.Hand.Any(c => c.Name.ToLower() == cardGuess))
            {
                return p.Hand.First(c => c.Name.ToLower() == cardGuess);
            }
           
            return null;
        }
        
        
        static void LayoutSets(Player p)
        {
            Console.WriteLine();
            
            var numberGroups = p.Hand.GroupBy(c => c.Number);
            foreach (var numberGroup in numberGroups)
            {
                if (numberGroup.Count() == 4)
                {
                    var numberCards = numberGroup.ToArray();
                    p.Sets.Add(new MatchingSet(numberCards));
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
                var setName = set.Cards.First().Name;
                if (setName == "Six")
                {
                    setName = "Sixe";
                }
                Console.WriteLine($" - {setName}s");
            }
        }


        static void GameOver()
        {
            Console.WriteLine("Game Over!");
            var playerPoints = 0;
            Player.Sets.ForEach(set => playerPoints += set.Cards.Count);
            Console.WriteLine($"Your score is {playerPoints}");
            var aiPoints = 0;
            AI.Sets.ForEach(set => aiPoints += set.Cards.Count);
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
