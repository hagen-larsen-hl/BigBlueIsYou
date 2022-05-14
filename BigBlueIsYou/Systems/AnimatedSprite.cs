using System;
using System.Collections.Generic;
using CS5410.Components;
using Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Systems
{
    public class AnimatedSprite : System
    {
        public override Type[] ComponentTypes { get; set; }
        public AnimatedSprite(Type[] componentTypes)
        {
            ComponentTypes = componentTypes;
        }
        public override Dictionary<uint, Entity> Update(GameTime gameTime)
        {
            foreach (KeyValuePair<uint, Entity> ent in m_entities)
            {
                Sprite component = ent.Value.GetComponent<Sprite>();
                component.AnimationTime -= gameTime.ElapsedGameTime;
                if (component.AnimationTime < TimeSpan.Zero)
                {
                    component.State += 1;
                    component.State %= component.SpriteTime.Length;
                    component.AnimationTime = new TimeSpan(0, 0, 0, 0, component.SpriteTime[component.State]);
                }
            }

            return m_entities;
        }
    }
}