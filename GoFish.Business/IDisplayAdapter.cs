using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GoFish.Business
{
    public interface IDisplayAdapter
    {
        event EventHandler<RankEventArgs> PlayerRequestsRank;

        event EventHandler<MessageEventArgs> CardsReturned;

        event EventHandler<MessageEventArgs> PlayerDraws;

        event EventHandler StartNewGame;
        Task StartGame();
        Task Shuffle();
        Task DrawTable(Player[] players, Player[] aiPlayers);
        Task MustDraw(string player);
        Task PlayersTurn();


        Task CardsFound(Card[] foundCards, string player);


        Task AIsTurn(Rank guessRank);

        Task EndGame(string message);
        Task InvalidCardRequest();
        Task InvalidCardReturn();
    }
}