using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Components
{
    public class Sprite : Component
    {
        public Texture2D SpriteSheet { get; set; }
        public Vector2 Size { get; set; }
        
        public Color Color { get; set; }
        
        public int[] SpriteTime { get; set; }
        
        public TimeSpan AnimationTime { get; set; }
        
        public int State { get; set; }

        public Sprite(Texture2D spriteSheet, Vector2 size, Color color, int[] spriteTime)
        {
            SpriteSheet = spriteSheet;
            Size = size;
            Color = color;
            SpriteTime = spriteTime;
            State = new CustomRandom().Next(1, spriteTime.Length);
            AnimationTime = TimeSpan.Zero;
        }
    }
}