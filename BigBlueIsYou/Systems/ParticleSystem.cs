using System;
using System.Collections.Generic;
using CS5410.Components;
using CS5410.Particles;
using Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Systems
{
    public enum EmitterTypeEnum
    {
        Win,
        You,
        Destroy
    }
    public class ParticleSystem
    {
        public List<ParticleEmitter> Emitters { get; set; }
        public Texture2D Texture { get; set; }
        private CustomRandom MyRandom { get; set; }
        private int m_heightBuffer;
        private int m_widthBuffer;

        public ParticleSystem(Texture2D texture, int heightBuffer, int widthBuffer)
        {
            Emitters = new List<ParticleEmitter>();
            Texture = texture;
            MyRandom = new CustomRandom();
            m_heightBuffer = heightBuffer;
            m_widthBuffer = widthBuffer;
        }

        public void Update(GameTime gameTime)
        {
            List<ParticleEmitter> removeMe = new List<ParticleEmitter>();
            foreach (ParticleEmitter emitter in Emitters)
            {
                if (emitter.m_particles.Count == 0)
                {
                    emitter.addParticles(gameTime, Texture, new Rectangle(50, 50, 100, 100));
                }
                emitter.update(gameTime);
                if (emitter.m_lifeRemaining < TimeSpan.Zero)
                {
                    removeMe.Add(emitter);
                }
            }
            foreach (ParticleEmitter remove in removeMe)
            {
                Emitters.Remove(remove);
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ParticleEmitter emitter in Emitters)
            {
                emitter.draw(spriteBatch);
            }
            
        }

        public void addEmitter(EmitterTypeEnum type, Entity entity)
        {
            if (type == EmitterTypeEnum.Win)
            {
                Emitters.Add( new ParticleEmitter(
                    EmitterTypeEnum.Win,
                    new TimeSpan(0, 0, 0, 1),
                    new TimeSpan(0, 0, 0, 0, 10),
                    2,
                    5,
                    new TimeSpan(0, 0, 0, 0, 500),
                    new TimeSpan(0, 0, 0, 2)));
            }
        }

        public void playerWon(Rectangle window)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 point = new Vector2(MyRandom.Next(window.Left, window.Right),
                    MyRandom.Next(window.Top, window.Bottom));
                ParticleEmitter emitter = new ParticleEmitter(
                    EmitterTypeEnum.Win,
                    new TimeSpan(0, 0, 0, 1),
                    new TimeSpan(0, 0, 0, 0, 10),
                    2,
                    2,
                    new TimeSpan(0, 0, 0, 0, 500),
                    new TimeSpan(0, 0, 0, 2));
                emitter.addParticlesAtSource(Texture, point, Color.White);
                Emitters.Add(emitter);
            }
        }
        
        public void winChanged(List<Entity> newWins)
        {
            foreach (Entity win in newWins)
            {
                Rectangle border = new Rectangle(
                    (int) (m_widthBuffer + win.GetComponent<Position>().Coordinates.X * win.GetComponent<Sprite>().Size.X),
                    (int) (m_heightBuffer +
                           win.GetComponent<Position>().Coordinates.Y * win.GetComponent<Sprite>().Size.Y),
                    (int) win.GetComponent<Sprite>().Size.X,
                    (int) win.GetComponent<Sprite>().Size.Y);
                ParticleEmitter emitter = new ParticleEmitter(
                    EmitterTypeEnum.Win,
                    new TimeSpan(0, 0, 0, 1),
                    new TimeSpan(0, 0, 0, 0, 100),
                    2,
                    1,
                    new TimeSpan(0, 0, 0, 0, 500),
                    new TimeSpan(0, 0, 0, 2));
                emitter.addParticlesOnBorder(Texture, border, Color.Yellow);
                Emitters.Add(emitter);
            }
        }
        
        public void youChanged(List<Entity> newYous)
        {
            foreach (Entity you in newYous)
            {
                Rectangle border = new Rectangle(
                    (int) (m_widthBuffer + you.GetComponent<Position>().Coordinates.X * you.GetComponent<Sprite>().Size.X),
                    (int) (m_heightBuffer +
                           you.GetComponent<Position>().Coordinates.Y * you.GetComponent<Sprite>().Size.Y),
                    (int) you.GetComponent<Sprite>().Size.X,
                    (int) you.GetComponent<Sprite>().Size.Y);
                ParticleEmitter emitter = new ParticleEmitter(
                    EmitterTypeEnum.Win,
                    new TimeSpan(0, 0, 0, 1),
                    new TimeSpan(0, 0, 0, 0, 100),
                    2,
                    1,
                    new TimeSpan(0, 0, 0, 0, 500),
                    new TimeSpan(0, 0, 0, 2));
                emitter.addParticlesOnBorder(Texture, border, Color.LightBlue);
                Emitters.Add(emitter);
            }
        }
        
        public void objectDeath(List<Entity> died)
        {
            foreach (Entity you in died)
            {
                Rectangle border = new Rectangle(
                    (int) (m_widthBuffer + you.GetComponent<Position>().Coordinates.X * you.GetComponent<Sprite>().Size.X),
                    (int) (m_heightBuffer +
                           you.GetComponent<Position>().Coordinates.Y * you.GetComponent<Sprite>().Size.Y),
                    (int) you.GetComponent<Sprite>().Size.X,
                    (int) you.GetComponent<Sprite>().Size.Y);
                ParticleEmitter emitter = new ParticleEmitter(
                    EmitterTypeEnum.Win,
                    new TimeSpan(0, 0, 0, 1),
                    new TimeSpan(0, 0, 0, 0, 100),
                    2,
                    2,
                    new TimeSpan(0, 0, 0, 0, 500),
                    new TimeSpan(0, 0, 0, 2));
                emitter.addParticlesOnBorder(Texture, border, Color.IndianRed);
                Emitters.Add(emitter);
            }
        }
        
        public void objectSinks(List<Entity> sinks)
        {
            foreach (Entity you in sinks)
            {
                Rectangle border = new Rectangle(
                    (int) (m_widthBuffer + you.GetComponent<Position>().Coordinates.X * you.GetComponent<Sprite>().Size.X),
                    (int) (m_heightBuffer +
                           you.GetComponent<Position>().Coordinates.Y * you.GetComponent<Sprite>().Size.Y),
                    (int) you.GetComponent<Sprite>().Size.X,
                    (int) you.GetComponent<Sprite>().Size.Y);
                ParticleEmitter emitter = new ParticleEmitter(
                    EmitterTypeEnum.Win,
                    new TimeSpan(0, 0, 0, 1),
                    new TimeSpan(0, 0, 0, 0, 100),
                    2,
                    2,
                    new TimeSpan(0, 0, 0, 0, 500),
                    new TimeSpan(0, 0, 0, 2));
                emitter.addParticlesOnBorder(Texture, border, Color.Aquamarine);
                Emitters.Add(emitter);
            }
        }
    }
}