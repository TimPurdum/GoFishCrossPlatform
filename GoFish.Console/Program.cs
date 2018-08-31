using System;
using GoFish.Business;


namespace GoFish.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(new ConsoleCommunicator());
            game.Start();
        }
    }
}