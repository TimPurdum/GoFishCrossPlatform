using System;


namespace GoFish.Business
{
    public interface ICommunicator
    {
        void Write(string message);
        string Read();
    }
}