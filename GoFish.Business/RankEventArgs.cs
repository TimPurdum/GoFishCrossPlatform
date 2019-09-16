using System;


namespace GoFish.Business
{
    public class RankEventArgs : EventArgs
    {
        public RankEventArgs(Rank rankRequested)
        {
            RankRequested = rankRequested;
        }


        public Rank RankRequested { get; }
    }
}