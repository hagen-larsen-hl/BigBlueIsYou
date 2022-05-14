using System;

namespace CS5410.Components
{
    public enum Adjectives
    {
        Stop,
        Push,
        Sink,
        Kill,
        You,
        Win
    }
    
    public class Adjective : Component
    {
        public Adjectives Type { get; set; }
        

        public Adjective(Adjectives type)
        {
            Type = type;
        }
    }
}