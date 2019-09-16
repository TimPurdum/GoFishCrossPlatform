using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoFish.Business;


namespace GoFish.Console
{
    public class ConsoleDisplayAdapter : IDisplayAdapter
    {
        public ConsoleDisplayAdapter()
        {
            System.Console.OutputEncoding = Encoding.UTF8;
        }
        
        
        public Task StartGame()
        {
            System.Console.Clear();
            System.Console.WriteLine("Welcome to Go Fish!");
            Thread.Sleep(1000);
            return Task.CompletedTask;
        }


        public Task Shuffle()
        {
            System.Console.Clear();
            System.Console.WriteLine("Shuffling...");
            Thread.Sleep(2000);
            return Task.CompletedTask;
        }


        public Task DrawTable(Player[] players, Player[] aiPlayers)
        {
            System.Console.Clear();
            for (var i = 0; i < players.Length; i++)
            {
                var p = players[i];
                DrawPlayer(p, i + 1);
            }

            for (var j = 0; j < aiPlayers.Length; j++)
            {
                DrawAi(aiPlayers[j], j + 1);
            }

            return Task.CompletedTask;
        }

        
        public Task MustDraw(string player)
        {
            System.Console.WriteLine($"Go Fish! {player} must draw a card!");
            Thread.Sleep(2000);
            PlayerDraws?.Invoke(this, new MessageEventArgs(player));
            return Task.CompletedTask;
        }


        public Task PlayersTurn()
        {
            System.Console.WriteLine("Your turn!");
            Thread.Sleep(1000);
            return Task.Run(() =>
            {
                System.Console.WriteLine("Ask me if I have a card...");
                System.Console.WriteLine("(e.g., Ace, Two, Three, Four, King...)");
                var response = System.Console.ReadLine();
                if (string.IsNullOrEmpty(response))
                {
                    PlayersTurn();
                    return;
                }

                try
                {
                    var rank = (Rank)Enum.Parse(typeof(Rank), response, true);
                    PlayerRequestsRank?.Invoke(this, new RankEventArgs(rank));
                }
                catch
                {
                    System.Console.WriteLine("I didn't catch that...'");
                    Thread.Sleep(1000);
                    PlayersTurn();
                }
            });
        }


        public Task CardsFound(Card[] foundCards, string player)
        {
            if (foundCards.Length > 1)
            {
                System.Console.WriteLine($"{player} got these cards!");
            }
            else
            {
                System.Console.WriteLine($"{player} got this card!");
            }
            foreach (var card in foundCards)
            {
                System.Console.WriteLine($"{card.Rank.ToString()} of {card.Suit.ToString()}");
            }
            Thread.Sleep(3000);
            return Task.CompletedTask;
        }


        public Task AIsTurn(Rank guessRank)
        {
            Thread.Sleep(1000);
            System.Console.WriteLine($"My turn! Do you have any {guessRank.ToString()}s?");
            System.Console.WriteLine("Type yes or no.");
            var response = System.Console.ReadLine();
            CardsReturned?.Invoke(this, new MessageEventArgs(response));
            return Task.CompletedTask;
        }


        public Task EndGame(string message)
        {
            System.Console.WriteLine(message);
            return Task.CompletedTask;
        }
        

        public Task InvalidCardRequest()
        {
            System.Console.WriteLine("You can only request a card rank that is in your hand.");
            Thread.Sleep(3000);
            PlayersTurn();
            return Task.CompletedTask;
        }


        public Task InvalidCardReturn()
        {
            System.Console.WriteLine("You must give up all cards of this rank in your hand!");
            var response = System.Console.ReadLine();
            CardsReturned?.Invoke(this, new MessageEventArgs(response));
            return Task.CompletedTask;
        }
        
        
        public event EventHandler<RankEventArgs> PlayerRequestsRank;
        public event EventHandler<MessageEventArgs> CardsReturned;
        public event EventHandler<MessageEventArgs> PlayerDraws;
        public event EventHandler StartNewGame;


        private static void DrawPlayer(Player player, int i)
        {
            System.Console.WriteLine($"Player {i} Hand:");
            System.Console.WriteLine(Divider);
            DrawHand(player.Hand.ToArray());
            System.Console.WriteLine(Divider);
            
            if (!player.Sets.Any())
            {
                return;
            }

            System.Console.WriteLine($"Player {i} Sets:");
            System.Console.WriteLine(Divider);
            DrawSets(player.Sets.ToArray());
            System.Console.WriteLine(Divider);
            System.Console.WriteLine();
        }

        
        private void DrawAi(Player aiPlayer, int i)
        {
            if (aiPlayer.Sets.Any())
            {
                System.Console.WriteLine("My Sets:");
                System.Console.WriteLine(Divider);
                DrawSets(aiPlayer.Sets.ToArray());
                System.Console.WriteLine(Divider);
            }
        }
        

        private static void DrawHand(Card[] hand)
        {
            // top row
            for (var i = 0; i < hand.Length; i++)
            {
                System.Console.Write(" ___  ");
            }

            System.Console.WriteLine();

            // rank row
            foreach (var card in hand)
            {
                System.Console.Write($"|{card.RankShortName} | ");
            }

            System.Console.WriteLine();

            // suit row
            foreach (var card in hand)
            {
                System.Console.Write($"|{card.SuitSymbol}  | ");
            }

            System.Console.WriteLine();

            // bottom row
            for (var i = 0; i < hand.Length; i++)
            {
                System.Console.Write("|___| ");
            }
            
            System.Console.WriteLine();
        }


        private static void DrawSets(List<Card>[] sets)
        {
            // top row
            foreach (var set in sets)
            {
                for (var i = 0; i < set.Count; i++)
                {
                    if (i < set.Count - 1)
                    {
                        System.Console.Write(" __");
                    }
                    else
                    {
                        System.Console.Write(" ___   ");
                    }
                }
            }

            System.Console.WriteLine();

            // rank row
            foreach (var set in sets)
            {
                foreach (var card in set)
                {
                    System.Console.Write($"|{card.RankShortName}");
                }

                System.Console.Write(" |  ");
            }

            System.Console.WriteLine();

            // suit row
            foreach (var set in sets)
            {
                foreach (var card in set)
                {
                    System.Console.Write($"|{card.SuitSymbol} ");
                }

                System.Console.Write(" |  ");
            }

            System.Console.WriteLine();

            // bottom row
            foreach (var set in sets)
            {
                for (var i = 0; i < set.Count; i++)
                {
                    System.Console.Write("|__");
                }

                System.Console.Write("_|  ");
            }

            System.Console.WriteLine();
        }


        private const string Divider =
            "--------------------------------------------------------------------------------------------------------------------------";
    }
}