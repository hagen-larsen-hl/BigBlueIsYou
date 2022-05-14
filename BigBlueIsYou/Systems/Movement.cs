using System;
using System.Collections.Generic;
using System.Xml;
using CS5410.Components;
using CS5410.Particles;
using Entities;
using Microsoft.Xna.Framework;

namespace CS5410.Systems
{
    public class Movement : System
    {
        public override Type[] ComponentTypes { get; set; }
        public List<List<List<Entity>>> EntityMap;
        public Movement(Type[] componentTypes, List<List<List<Entity>>> entityMap)
        {
            ComponentTypes = componentTypes;
            EntityMap = entityMap;
        }

        public Dictionary<Entity, Position> moveRight(List<List<List<Entity>>> newMap)
        {
            EntityMap = newMap;
            Dictionary<Entity, Position> updated = new Dictionary<Entity, Position>();
            foreach (KeyValuePair<uint, Entity> entity in m_entities)
            {
                if (entity.Value.ContainsComponent<Moveable>())
                {
                    moveEntityRight(entity.Value, updated);
                }
            }

            return updated;
        }

        public bool moveEntityRight(Entity entity, Dictionary<Entity, Position> updated)
        {
            Vector2 current = entity.GetComponent<Position>().Coordinates;
            Vector2 desired = new Vector2(current.X + 1, current.Y);
            bool canMove = true;
            foreach (Entity neighbor in EntityMap[(int) desired.X][(int) desired.Y])
            {
                if (neighbor.ContainsComponent<Pushable>())
                {
                    canMove = moveEntityRight(neighbor, updated);
                }
                else if (neighbor.ContainsComponent<Stop>())
                {
                    return false;
                }
            }
            if (canMove)
            {
                try
                {
                    updated.Add(entity, new Position(new Vector2(desired.X, desired.Y)));
                }
                catch (ArgumentException)
                {
                    
                }
            }
            return canMove;
        }
        
        public Dictionary<Entity, Position> moveLeft(List<List<List<Entity>>> newMap)
        {
            EntityMap = newMap;
            Dictionary<Entity, Position> updated = new Dictionary<Entity, Position>();
            foreach (KeyValuePair<uint, Entity> entity in m_entities)
            {
                if (entity.Value.ContainsComponent<Moveable>())
                {
                    moveEntityLeft(entity.Value, updated);
                }
            }

            return updated;
        }

        public bool moveEntityLeft(Entity entity, Dictionary<Entity, Position> updated)
        {
            Vector2 current = entity.GetComponent<Position>().Coordinates;
            Vector2 desired = new Vector2(current.X - 1, current.Y);
            bool canMove = true;
            foreach (Entity neighbor in EntityMap[(int) desired.X][(int) desired.Y])
            {
                if (neighbor.ContainsComponent<Pushable>())
                {
                    canMove = moveEntityLeft(neighbor, updated);
                }
                else if (neighbor.ContainsComponent<Stop>())
                {
                    return false;
                }
            }
            if (canMove)
            {
                try
                {
                    updated.Add(entity, new Position(new Vector2(desired.X, desired.Y)));
                }
                catch (ArgumentException)
                {
                    
                }
            }
            return canMove;
        }
        
        public Dictionary<Entity, Position> moveUp(List<List<List<Entity>>> newMap)
        {
            EntityMap = newMap;
            Dictionary<Entity, Position> updated = new Dictionary<Entity, Position>();
            foreach (KeyValuePair<uint, Entity> entity in m_entities)
            {
                if (entity.Value.ContainsComponent<Moveable>())
                {
                    moveEntityUp(entity.Value, updated);
                }
            }
            
            return updated;
        }

        public bool moveEntityUp(Entity entity, Dictionary<Entity, Position> updated)
        {
            Vector2 current = entity.GetComponent<Position>().Coordinates;
            Vector2 desired = new Vector2(current.X, current.Y - 1);
            bool canMove = true;
            foreach (Entity neighbor in EntityMap[(int) desired.X][(int) desired.Y])
            {
                if (neighbor.ContainsComponent<Pushable>())
                {
                    canMove = moveEntityUp(neighbor, updated);
                }
                else if (neighbor.ContainsComponent<Stop>())
                {
                    return false;
                }
            }
            if (canMove)
            {
                try
                {
                    updated.Add(entity, new Position(new Vector2(desired.X, desired.Y)));
                }
                catch (ArgumentException)
                {
                    
                }
            }
            return canMove;
        }
        
        public Dictionary<Entity, Position> moveDown(List<List<List<Entity>>> newMap)
        {
            EntityMap = newMap;
            Dictionary<Entity, Position> updated = new Dictionary<Entity, Position>();
            foreach (KeyValuePair<uint, Entity> entity in m_entities)
            {
                if (entity.Value.ContainsComponent<Moveable>())
                {
                    moveEntityDown(entity.Value, updated);
                }
            }

            return updated;
        }

        public bool moveEntityDown(Entity entity, Dictionary<Entity, Position> updated)
        {
            Vector2 current = entity.GetComponent<Position>().Coordinates;
            Vector2 desired = new Vector2(current.X, current.Y + 1);
            bool canMove = true;
            foreach (Entity neighbor in EntityMap[(int) desired.X][(int) desired.Y])
            {
                if (neighbor.ContainsComponent<Pushable>())
                {
                    canMove = moveEntityDown(neighbor, updated);
                }
                else if (neighbor.ContainsComponent<Stop>())
                {
                    return false;
                }
            }
            if (canMove)
            {
                try
                {
                    updated.Add(entity, new Position(new Vector2(desired.X, desired.Y)));
                }
                catch (ArgumentException)
                {
                    
                }
            }
            return canMove;
        }

        public override Dictionary<uint, Entity> Update(GameTime gameTime)
        {
            return new Dictionary<uint, Entity>();
        }
    }
}