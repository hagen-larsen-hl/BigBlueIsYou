namespace CS5410.Components
{
    public enum Nouns
    {
        BigBlue, Wall, Text, Flag, Rock, Lava, Water
    }
    public class Noun : Component
    {
        public Nouns Type { get; set; }

        public Noun(Nouns type)
        {
            Type = type;
        }
    }
}