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

                while (Player.Hand.Count > 0 && AI.Hand.Count > 0)
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
            Console.WriteLine();
            Console.WriteLine("Ask me if I have a card...");
            Console.WriteLine("(Use the format like 'ten of spades')");
            var guess = Console.ReadLine();
            Console.WriteLine();
            var card = FindCard(guess, AI);
            
            if (card.HasValue)
            {
                Console.WriteLine($"You got the {guess}!");
                Player.Hand.Add(card.Value);
                AI.Hand.Remove(card.Value);
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
            var guessDeck = new Deck();
            guessDeck.Shuffle();
            var guessCard = guessDeck.Peek();
            Console.WriteLine();
            Console.WriteLine("My turn.");
            var guess = $"{guessCard.Name} of {guessCard.Suit.Name}";
            Console.WriteLine($"Do you have the {guess}?");
            Thread.Sleep(2000);
            Console.WriteLine();
            var card = FindCard(guess, Player);
            
            if (card.HasValue)
            {
                Console.WriteLine($"Yes? I'll take that {guess}!");
                AI.Hand.Add(card.Value);
                Player.Hand.Remove(card.Value);
            }
            else
            {
                Console.WriteLine("No? I have to draw...");
                Deck.Draw(AI, 1);
            }
        }


        static Card? FindCard(string guess, Player p)
        {
            var words = guess.Split(" ");
            
            if (words.Length > 2)
            {
                var cardName = words[0].ToLower();
                var suitName = words[2].ToLower();
                    
                if (p.Hand.Any(c =>
                    c.Name.ToLower() == cardName && c.Suit.Name.ToLower() == suitName))
                {
                    return p.Hand.First(c =>
                        c.Name.ToLower() == cardName && c.Suit.Name.ToLower() == suitName);
                }
            }

            return null;
        }
        
        
        static void LayoutSets(Player p)
        {
            Console.WriteLine();
            
            FindSequences(p);

            FindThreeOrFourOfAKind(p);

            if (p.Sets.Count == 0)
            {
                return;
            }

            AddToExistingSets(p);

            ShowSets(p);
        }


        static void FindSequences(Player p)
        {
            var suitGroups = p.Hand.GroupBy(c => c.Suit);
            foreach (var suitGroup in suitGroups)
            {
                if (suitGroup.Count() > 2)
                {
                    var suitCards = suitGroup.ToArray().OrderBy(c => c.Number).ToArray();
                    var sequentialCards = new List<Card>{suitCards[0]};
                    foreach (var card in suitCards)
                    {
                        if (card.Number == sequentialCards.Last().Number + 1)
                        {
                            sequentialCards.Add(card);
                        }
                        else
                        {
                            if (sequentialCards.Count > 2)
                            {
                                // save
                                p.Sets.Add(new MatchingSet(sequentialCards));
                                foreach (var c in sequentialCards)
                                {
                                    p.Hand.Remove(c);
                                }
                            }
                            // empty and start over
                            sequentialCards = new List<Card>{card};
                        }
                    }
                    if (sequentialCards.Count > 2)
                    {
                        // save
                        p.Sets.Add(new MatchingSet(sequentialCards));
                        foreach (var c in sequentialCards)
                        {
                            p.Hand.Remove(c);
                        }
                    }
                } 
            }
        }


        static void FindThreeOrFourOfAKind(Player p)
        {
            var numberGroups = p.Hand.GroupBy(c => c.Number);
            foreach (var numberGroup in numberGroups)
            {
                if (numberGroup.Count() > 2)
                {
                    var numberCards = numberGroup.ToArray();
                    p.Sets.Add(new MatchingSet(numberCards));
                    foreach (var c in numberCards)
                    {
                        p.Hand.Remove(c);
                    }
                }
            }
        }


        static void AddToExistingSets(Player p)
        {
            foreach (var set in p.Sets)
            {
                // 3 of a kind sets
                if (set.Cards.GroupBy(c => c.Number).Count() == 1)
                {
                    var setNumber = set.Cards.First().Number;
                    if (set.Cards.Count == 3 && p.Hand.Any(c => c.Number == setNumber))
                    {
                        var newCard = p.Hand.Single(c => c.Number == setNumber);
                        set.AddCard(newCard);
                        p.Hand.Remove(newCard);
                    }
                }
                else
                {
                    var suit = set.Cards.First().Suit;
                    if (p.Hand.Any(c => c.Suit == suit))
                    {
                        var suitCards = p.Hand.Where(c => c.Suit == suit).ToArray();
                        foreach (var suitCard in suitCards)
                        {
                            if (set.Cards.Any(c =>
                                c.Number == suitCard.Number - 1 || c.Number == suitCard.Number + 1))
                            {
                                set.AddCard(suitCard);
                                p.Hand.Remove(suitCard);
                            }
                        }
                    }
                }
            }
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
                Console.WriteLine("Set:");
                foreach (var card in set.Cards)
                {
                    Console.WriteLine($"- {card.Name} of {card.Suit.Name}");
                }
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
