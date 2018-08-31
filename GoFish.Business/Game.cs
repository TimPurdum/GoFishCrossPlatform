using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace GoFish.Business
{
    public class Game
    {
        readonly ICommunicator Communicator;
        readonly Deck Deck = new Deck();
        readonly Player AI = new Player();
        readonly Player Player = new Player();

        public Game(ICommunicator comm)
        {
            Communicator = comm;
        }


        public void Start()
        {
            Communicator.Write("Let's play Go Fish!");
            Communicator.Write("Type the letter 's' to shuffle the deck.");
            if (Communicator.Read()?.ToLower() == "s")
            {
                Communicator.Write("Shuffling...");
                Deck.Shuffle();
                Communicator.Write("");
                Deck.Deal(new List<Player>{Player, AI}, 7);

                while (Player.Hand.Count > 0 || AI.Hand.Count > 0 && Deck.Count > 0)
                {
                    ShowHand();
                    Guess();
                    AIGuess();
                }

                GameOver();
            }
        }
        
        
        void ShowHand()
        {
            Communicator.Write("Here is your hand:");
            foreach (var card in Player.Hand)
            {
                Communicator.Write($" - {card.Rank} of {card.Suit}");
            }
        }


        void Guess()
        {
            if (Player.Hand.Count == 0)
            {
                Communicator.Write("No cards, you must draw this turn!");
                Deck.Draw(Player);
                return;
            }
            
            Communicator.Write("");
            Communicator.Write("Ask me if I have a card...");
            Communicator.Write("(e.g., Ace, Two, Three, Four, King...)");
            var guess = Communicator.Read();

            while (Player.Hand.All(c => c.Rank.ToString().ToLower() != guess?.ToLower()))
            {
                Communicator.Write("You may only guess card numbers that you already have.");
                Communicator.Write("Ask me if I have a card...");
                guess = Communicator.Read();
            }
            
            Communicator.Write("");
            var cards = FindCards(guess, AI);
            
            if (cards != null)
            {
                var message = $"You got {cards.Count} {cards.First().Rank}!";
                if (cards.Count > 1)
                {
                    message = $"You got {cards.Count} {cards.First().PluralName}!";
                }
                Communicator.Write(message);
                
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
                Communicator.Write("No! Go Fish!");
                Thread.Sleep(1000);
                Deck.Draw(Player);
                var newCard = Player.Hand.Last();
                Communicator.Write($"You drew the {newCard.Rank} of {newCard.Suit}");
                LayoutSets(Player);
            }
        }


        void AIGuess()
        {
            if (AI.Hand.Count == 0)
            {
                Communicator.Write("No cards, I must draw this turn!");
                Deck.Draw(AI);
                return;
            }
            
            var randomGenerator = new Random();

            var guessCard = AI.Hand[randomGenerator.Next(0, AI.Hand.Count - 1)];
            
            Communicator.Write("My turn.");
            
            Communicator.Write($"Do you have any {guessCard.PluralName}?");
            Thread.Sleep(2000);
            Communicator.Write("");
            var cards = FindCards(guessCard.Rank.ToString(), Player);
            
            if (cards != null)
            {
                if (cards.Count == 1)
                {
                    Communicator.Write($"Yes? I'll take that {guessCard.Rank}!");
                }
                else
                {
                    Communicator.Write($"Yes? I'll take those {guessCard.PluralName}!");
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
                Communicator.Write("No? I have to draw...");
                Thread.Sleep(1000);
                Deck.Draw(AI);
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
        
        
        void LayoutSets(Player p)
        {
            Communicator.Write("");
            
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


        void ShowSets(Player p)
        {
            if (p == Player)
            {
                Communicator.Write("You have these sets:");
            }
            else
            {
                Communicator.Write("I have these sets:");
            }
            
            foreach (var set in p.Sets)
            {
                Communicator.Write($" - {set.First().PluralName}");
            }

            if (p == Player)
            {
                ShowHand();
            }
        }


        void GameOver()
        {
            Communicator.Write("Game Over!");
            var playerPoints = 0;
            Player.Sets.ForEach(set => playerPoints += set.Count);
            Communicator.Write($"Your score is {playerPoints}");
            var aiPoints = 0;
            AI.Sets.ForEach(set => aiPoints += set.Count);
            Communicator.Write($"My score is {aiPoints}");
            if (playerPoints > aiPoints)
            {
                Communicator.Write("You Win!");
            }
            else if (aiPoints > playerPoints)
            {
                Communicator.Write("I Win!");
            }
            else
            {
                Communicator.Write("Tie Game!");
            }
        }
    }
}