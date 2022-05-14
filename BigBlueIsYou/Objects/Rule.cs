using Entities;

namespace CS5410.Objects
{
    public class Rule
    {
        public Entity Verb { get; set; }
        public Entity Predecessor { get; set; }
        public Entity Successor { get; set; }

        public Rule(Entity verb, Entity pre, Entity post)
        {
            Verb = verb;
            Predecessor = pre;
            Successor = post;
        }
    }
}