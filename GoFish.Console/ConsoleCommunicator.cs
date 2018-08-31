using System;
using GoFish.Business;


namespace GoFish.Console
{
    public class ConsoleCommunicator: ICommunicator
    {
        public void Write(string message)
        {
            System.Console.WriteLine(message);
        }


        public string Read()
        {
            return System.Console.ReadLine();
        }
    }
}