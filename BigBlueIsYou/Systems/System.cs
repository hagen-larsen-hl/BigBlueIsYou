using Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CS5410.Systems
{
    /// <summary>
    /// The base class for all systems in this ECS environment.
    /// <para>Derived classes should override <see cref="ComponentTypes"/> to specify the types of <see cref="Component"/> an <see cref="Entity"/> must have in order for the derived class to work with it, and <see cref="UpdateEntity(GameTime, Entity)"/> and/or <see cref="DrawEntity(GameTime, Entity)"/> to implement system-specific behavior with matching entities.</para>
    /// </summary>
    public abstract class System
    {
        // Contains the entities this system is interested in, keyed by their IDs.
        protected Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();

        /// <summary>
        /// Constructs a new system that should be interested in the given component types.
        /// </summary>
        /// <param name="componentTypes">The types of components an entity should have for this system to be interested in it.</param>
        public System(params Type[] componentTypes)
        {
            this.ComponentTypes = componentTypes;
        }

        /// <summary>
        /// An array containing the types of components an entity must have for this system to be interested in it.
        /// </summary>
        public abstract Type[] ComponentTypes { get; set; }

        /// <summary>
        /// Returns whether this system is interested in the given <see cref="Entity"/>.
        /// <para>The default behavior checks whether the <see cref="Entity"/> contains a component for each type listed in <see cref="ComponentTypes"/>, or simply returns true if <see cref="ComponentTypes"/> is empty or null.</para>
        /// </summary>
        /// <param name="entity">The entity to be checked for interest.</param>
        public bool IsInterested(Entity entity)
        {
            foreach (Type type in ComponentTypes)
            {
                if (entity.ContainsComponent(type))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds an entity to this system, if <see cref="IsInterested(Entity)"/> returns true for <paramref name="entity"/>.
        /// </summary>
        /// <returns>Returns true if the given entity was added to this system. Otherwise returns false.</returns>
        public bool Add(Entity entity)
        {
            if (IsInterested(entity))
            {
                m_entities.Add(entity.Id, entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the <see cref="Entity"/> with the given ID from this system if it is found in it.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Entity"/> to be removed.</param>
        /// <returns>Returns true if the entity was found and removed, otherwise returns false.</returns>
        public bool Remove(uint id)
        {
            return m_entities.Remove(id);
        }

        /// <summary>
        /// Derived classes must override this method to perform update logic specific to their type of system.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public abstract Dictionary<uint, Entity> Update(GameTime gameTime);
    }
}
