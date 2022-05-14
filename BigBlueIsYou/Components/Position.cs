using System.Reflection.PortableExecutable;
using Microsoft.Xna.Framework;

namespace CS5410.Components
{
    public class Position : Component
    {
        public Vector2 Coordinates { get; set; }

        public Position(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }
    }
}