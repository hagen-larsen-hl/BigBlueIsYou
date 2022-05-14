using System.Runtime.CompilerServices;

namespace CS5410.Components
{
    public enum Texts
    {
        Noun,
        Verb,
        Adjective
    }
    public class Text : Component
    {
        public Texts Type { get; set; }

        public Text(Texts type)
        {
            Type = type;
        }
    }
}