using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GoFish.Business
{
    public class Game
    {
        public Game(IDisplayAdapter display)
        {
            _display = display;
            _display.PlayerRequestsRank += OnRequestRank;
            _display.PlayerDraws += OnPlayerDraw;
            _display.CardsReturned += OnCardsReturned;
            _display.StartNewGame += OnNewGame;
            _tokenSource = new CancellationTokenSource();
        }


        public async Task StartGame(CancellationToken? token = null)
        {
            if (token == null)
            {
                token = _tokenSource.Token;
            }
            
            _player = new Player();
            _ai = new Player();
            await _display.StartGame();
            _deck.Shuffle();
            await _display.Shuffle();

            _deck.Deal(new List<Player> {_player, _ai}, 7);
            await BeginGameLoop(token.Value);
        }


        private async Task BeginGameLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await _display.DrawTable(new[] {_player}, new[] {_ai});
                await _display.PlayersTurn();
                await WaitForPlayerResponse(token);
                await _display.DrawTable(new[] {_player}, new[] {_ai});
                await AiGuess();
                await WaitForPlayerResponse(token);
            }
        }


        private Task WaitForPlayerResponse(CancellationToken token)
        {
            while (!_playerTurnComplete && !token.IsCancellationRequested)
            {
                Thread.Sleep(500);
            }

            _playerTurnComplete = false;
            return Task.CompletedTask;
        }


        private async void OnNewGame(object sender, EventArgs e)
        {
            _tokenSource.Cancel();
            await StartGame(_tokenSource.Token);
        }


        private void OnPlayerDraw(object sender, MessageEventArgs e)
        {
            if (e.Message == "You")
            {
                DrawCard(_player);
            }
            else
            {
                DrawCard(_ai);   
            }
        }


        private async void OnRequestRank(object sender, RankEventArgs e)
        {
            if (!_player.Hand.Any(c => c.Rank.Equals(e.RankRequested)))
            {
                await _display.InvalidCardRequest();
                await _display.PlayersTurn();
                
                return;
            }

            var foundCards = FindCards(e.RankRequested, _ai);
            if (foundCards == null)
            {
                await _display.MustDraw("You");
                return;
            }

            _player.Hand.AddRange(foundCards);
            foreach (var c in foundCards)
            {
                _ai.Hand.Remove(c);
            }

            await _display.CardsFound(foundCards.ToArray(), "You");
            LayoutSets(_player);
            if (!_deck.Any() || !_player.Hand.Any())
            {
                await GameOver();
                return;
            }

            await _display.DrawTable(new[] {_player}, new[] {_ai});
            await _display.PlayersTurn();
            
        }


        private async void DrawCard(Player p)
        {
            _deck.Draw(p);
            var newCard = p.Hand.Last();
            await _display.CardsFound(new[] {newCard}, p == _player ? "You": "I");
            LayoutSets(p);
            if (!_deck.Any() || !p.Hand.Any())
            {
                await GameOver();
                return;
            }

            await _display.DrawTable(new[] {_player}, new[] {_ai});

            _playerTurnComplete = true;
        }


        private async Task AiGuess()
        {
            var randomGenerator = new Random();

            _guessRank = _ai.Hand[randomGenerator.Next(0, _ai.Hand.Count - 1)].Rank;

            await Task.Run(async () => { await _display.AIsTurn(_guessRank); });
        }


        private async void OnCardsReturned(object sender, MessageEventArgs e)
        {
            var foundCards = FindCards(_guessRank, _player);
            if (e.Message?.ToLowerInvariant() == "no" && foundCards != null && foundCards.Any())
            {
                await _display.InvalidCardReturn();
                return;
            }
            if (foundCards == null || foundCards.Count == 0)
            {
                await _display.MustDraw("I");
                return;
            }

            _ai.Hand.AddRange(foundCards);
            foreach (var c in foundCards)
            {
                _player.Hand.Remove(c);
            }

            await _display.CardsFound(foundCards.ToArray(), "I");

            if (!_deck.Any() || !_ai.Hand.Any())
            {
                await GameOver();
                return;
            }

            await _display.DrawTable(new[] {_player}, new[] {_ai});
            await AiGuess();
        }


        private static List<Card> FindCards(Rank rank, Player p)
        {
            if (p.Hand.Any(c => c.Rank.Equals(rank)))
            {
                return p.Hand.Where(c => c.Rank.Equals(rank)).ToList();
            }

            return null;
        }


        private static void LayoutSets(Player p)
        {
            var numberGroups = p.Hand.GroupBy(c => c.Rank);
            foreach (var numberGroup in numberGroups)
            {
                if (numberGroup.Count() != 4)
                {
                    continue;
                }

                var numberCards = numberGroup.ToList();
                p.Sets.Add(numberCards);
                foreach (var c in numberCards)
                {
                    p.Hand.Remove(c);
                }
            }
        }


        private async Task GameOver()
        {
            var sb = new StringBuilder("Game Over!");
            var playerPoints = 0;
            _player.Sets.ForEach(set => playerPoints += set.Count);
            sb.AppendLine($"Your score is {playerPoints}");
            var aiPoints = 0;
            _ai.Sets.ForEach(set => aiPoints += set.Count);
            sb.AppendLine($"My score is {aiPoints}");
            if (playerPoints > aiPoints)
            {
                sb.AppendLine("You Win!");
            }
            else if (aiPoints > playerPoints)
            {
                sb.AppendLine("I Win!");
            }
            else
            {
                sb.AppendLine("Tie Game!");
            }

            await _display.EndGame(sb.ToString());
        }

        private readonly CancellationTokenSource _tokenSource;
        private readonly Deck _deck = new Deck();
        private readonly IDisplayAdapter _display;
        private Player _ai = new Player();
        private Rank _guessRank;
        private Player _player = new Player();
        private bool _playerTurnComplete;
    }
}