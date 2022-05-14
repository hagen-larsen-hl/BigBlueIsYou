using System;
using System.Collections.Generic;
using CS5410.Components;
using Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Systems
{
    public class RenderSprite : System
    {
        public SpriteBatch SpriteBatch { get; set; }
        public override Type[] ComponentTypes { get; set; }
        
        public int VerticalOffset { get; set; } 
        
        public int HorizontalOffset { get; set; }
        
        public RenderSprite(SpriteBatch spriteBatch, Type[] componentTypes, int veticalOffset, int horizontalOffset)
        {
            ComponentTypes = componentTypes;
            SpriteBatch = spriteBatch;
            VerticalOffset = veticalOffset;
            HorizontalOffset = horizontalOffset;
        }

        public override Dictionary<uint, Entity> Update(GameTime gameTime)
        {
            foreach (Entity entity in m_entities.Values)
            {
                Sprite sprite = entity.GetComponent<Sprite>();
                Position position = entity.GetComponent<Position>();
                SpriteBatch.Draw(
                    sprite.SpriteSheet,
                    new Rectangle(
                        (int) (position.Coordinates.X * sprite.Size.X) + HorizontalOffset,
                        (int) (position.Coordinates.Y * sprite.Size.Y) + VerticalOffset,
                        (int)sprite.Size.X,
                        (int)sprite.Size.Y),
                    new Rectangle(
                        (sprite.SpriteSheet.Width / sprite.SpriteTime.Length) * sprite.State,
                        0,
                        sprite.SpriteSheet.Width / sprite.SpriteTime.Length,
                        sprite.SpriteSheet.Height),
                    sprite.Color);
            }
            return m_entities;
        }
    }
}