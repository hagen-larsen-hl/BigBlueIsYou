using System.Collections.Generic;
using Entities;
using Microsoft.Xna.Framework;

namespace CS5410.Objects
{
    public class Level
    {
        public Level()
        {
            
        } 
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<KeyValuePair<char,Vector2>> Entities { get; set; }
    }
}