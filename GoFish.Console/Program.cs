using System.Threading.Tasks;
using GoFish.Business;


namespace GoFish.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var game = new Game(new ConsoleDisplayAdapter());
            await game.StartGame();
        }
    }
}