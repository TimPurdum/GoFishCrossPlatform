using System.Collections.Generic;


namespace GoFish
{
    public struct Suit
    {
        public string Name;
        public string Color;

        public Suit(string name, string color)
        {
            Name = name;
            Color = color;
        }
        
        public static bool operator == (Suit s1, Suit s2)
        {
            return s1.Name == s2.Name;
        }


        public static bool operator != (Suit s1, Suit s2)
        {
            return s1.Name != s2.Name;
        }


        public override bool Equals(object obj)
        {
            if (obj is Suit s2)
            {
                return Name == s2.Name;    
            }

            return false;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}