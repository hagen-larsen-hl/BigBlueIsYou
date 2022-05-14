using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using CS5410.Objects;
using CS5410.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Particles
{
    public class ParticleEmitter
    {

        public Dictionary<int, Particle> m_particles = new Dictionary<int, Particle>();
        private CustomRandom m_random = new CustomRandom();

        private TimeSpan m_rate;
        private int m_sarticleSize;
        private int m_speed;
        private TimeSpan m_lifetime;
        private TimeSpan m_switchover;
        public TimeSpan m_lifeRemaining;
        private TimeSpan m_accumulated = TimeSpan.Zero;
        public EmitterTypeEnum m_type;

        public Vector2 Gravity { get; set; }

        public ParticleEmitter(EmitterTypeEnum type, TimeSpan lifeRemaining, TimeSpan rate, int size, int speed, TimeSpan lifetime, TimeSpan wwitchover)
        {
            m_type = type;
            m_lifeRemaining = lifeRemaining;
            m_rate = rate;
            m_sarticleSize = size;
            m_speed = speed;
            m_lifetime = lifetime;
            m_switchover = wwitchover;
            
            this.Gravity = new Vector2(0, 0);
        }

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public void update(GameTime gameTime)
        {
            m_lifeRemaining -= gameTime.ElapsedGameTime;
            updateParticles(gameTime);
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, m_sarticleSize, m_sarticleSize);
            foreach (Particle p in m_particles.Values)
            {
                r.X = (int)p.position.X;
                r.Y = (int)p.position.Y;
                spriteBatch.Draw(
                    p.texture,
                    r,
                    null,
                    p.color,
                    p.rotation,
                    new Vector2(p.texture.Width / 2, p.texture.Height / 2),
                    SpriteEffects.None,
                    0);
            }
        }

        public void addParticles(GameTime gameTime, Texture2D tex, Rectangle region)
        {
            m_accumulated += gameTime.ElapsedGameTime;
            // while (m_accumulated > m_rate)
            for (int i = 0; i < 300; i++)
            {
                m_accumulated -= m_rate;

                // Particle p = new Particle(
                // m_random.Next(),
                // new Vector2(m_sourceX, m_sourceY),
                // m_random.nextCircleVector(),
                // (float)m_random.nextGaussian(m_speed, 1),
                // m_lifetime);
            }
        }
        
        public void addParticlesAtSource(Texture2D tex, Vector2 point, Color color)
        {
            // while (m_accumulated > m_rate)
            for (int i = 0; i < 300; i++)
            {
                m_accumulated -= m_rate;

                // Particle p = new Particle(
                // m_random.Next(),
                // new Vector2(m_sourceX, m_sourceY),
                // m_random.nextCircleVector(),
                // (float)m_random.nextGaussian(m_speed, 1),
                // m_lifetime);

                Particle p = new Particle(
                    i,
                    new Vector2(
                        point.X,
                        point.Y
                    ),
                    m_random.nextCircleVector(),
                    (float)m_random.nextGaussian(m_speed, 1),
                    new TimeSpan(0, 0, 0, 0, m_random.Next(0, 1000)),
                    tex,
                    color
                );

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }

        }

        public void addParticlesOnBorder(Texture2D tex, Rectangle border, Color color)
        {
            for (int i = 0; i < 50; i++)
            {
                m_accumulated -= m_rate;

                Particle p = new Particle(
                    i,
                    new Vector2(
                        m_random.Next(border.Left, border.Right),
                        border.Top
                    ),
                    m_random.nextCircleVector(),
                    (float) 0.5,
                    new TimeSpan(0, 0, 0, 0, m_random.Next(0, 500)),
                    tex,
                    color
                );

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }
            for (int i = 50; i < 100; i++)
            {
                m_accumulated -= m_rate;

                Particle p = new Particle(
                    i,
                    new Vector2(
                        m_random.Next(border.Left, border.Right),
                        border.Bottom
                    ),
                    m_random.nextCircleVector(),
                    (float) 0.5,
                    new TimeSpan(0, 0, 0, 0, m_random.Next(0, 500)),
                    tex,
                    color
                );

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }
            for (int i = 100; i < 150; i++)
            {
                m_accumulated -= m_rate;

                Particle p = new Particle(
                    i,
                    new Vector2(
                        border.Right,
                        m_random.Next(border.Top, border.Bottom)
                    ),
                    m_random.nextCircleVector(),
                    (float) 0.5,
                    new TimeSpan(0, 0, 0, 0, m_random.Next(0, 500)),
                    tex,
                    color
                );

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }
            for (int i = 150; i < 200; i++)
            {
                m_accumulated -= m_rate;

                Particle p = new Particle(
                    i,
                    new Vector2(
                        border.Left,
                        m_random.Next(border.Top, border.Bottom)
                    ),
                    m_random.nextCircleVector(),
                    (float) 0.5,
                    new TimeSpan(0, 0, 0, 0, m_random.Next(0, 500)),
                    tex,
                    color
                );

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }
        }

        private void updateParticles(GameTime gameTime)
        {
            List<int> removeMe = new List<int>();
            foreach (Particle p in m_particles.Values)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(p.name);
                }
                //
                // Update its position
                p.position += (p.direction * p.speed);
                //
                // Have it rotate proportional to its speed
                p.rotation += p.speed / 50.0f;
                //
                // Apply some gravity
                p.direction += this.Gravity;
            }

            //
            // Remove any expired particles
            foreach (int Key in removeMe)
            {
                m_particles.Remove(Key);
            }
        }
    }
}
