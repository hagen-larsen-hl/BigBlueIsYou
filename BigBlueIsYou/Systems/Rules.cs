using System;
using System.Collections.Generic;
using System.Data;
using CS5410.Components;
using CS5410.Objects;
using CS5410.Particles;
using Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rule = System.Data.Rule;

namespace CS5410.Systems
{
    public class Rules
    {
        public List<Objects.Rule> m_rules;
        private ContentManager m_contentManager;

        public Rules(List<Objects.Rule> rules, ContentManager contentManager)
        {
            m_rules = rules;
            m_contentManager = contentManager;
        }

        public List<Objects.Rule> scanRules(List<List<List<Entity>>> map)
        {
            List<Objects.Rule> res = new List<Objects.Rule>();
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    foreach (Entity entity in map[i][j])
                    {
                        if (entity.ContainsComponent<Text>() && entity.GetComponent<Text>().Type == Texts.Verb)
                        {
                            Entity pre = null;
                            Entity post = null;
                            if (map[i - 1][j].Count > 0)
                            {
                                foreach (Entity neighbor in map[i-1][j])
                                {
                                    if (neighbor.ContainsComponent<Text>() && neighbor.GetComponent<Text>().Type == Texts.Noun)
                                    {
                                        pre = neighbor;
                                    }
                                }
                            }
                            if (pre != null && map[i + 1][j].Count > 0)
                            {
                                foreach (Entity neighbor in map[i + 1][j])
                                {
                                    if (neighbor.ContainsComponent<Text>())
                                    {
                                        post = neighbor;
                                        Objects.Rule rule = new Objects.Rule(entity, pre, post);
                                        res.Add(rule);
                                    }
                                }
                            }

                            pre = null;
                            post = null;

                            if (map[i][j-1].Count > 0)
                            {
                                foreach (Entity neighbor in map[i][j-1])
                                {
                                    if (neighbor.ContainsComponent<Text>() && neighbor.GetComponent<Text>().Type == Texts.Noun)
                                    {
                                        pre = neighbor;
                                    }
                                }
                            }
                            if (pre != null && map[i][j + 1].Count > 0)
                            {
                                foreach (Entity neighbor in map[i][j + 1])
                                {
                                    if (neighbor.ContainsComponent<Text>())
                                    {
                                        post = neighbor;
                                        Objects.Rule rule = new Objects.Rule(entity, pre, post);
                                        res.Add(rule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return res;
        }

        public List<Objects.Rule> getOutdatedRules(List<Objects.Rule> newRules)
        {
            List<Objects.Rule> res = new List<Objects.Rule>();
            foreach (Objects.Rule oldRule in m_rules)
            {
                bool match = false;
                foreach (Objects.Rule newRule in newRules)
                {
                    if (newRule.Verb == oldRule.Verb && newRule.Predecessor == oldRule.Predecessor &&
                        newRule.Successor == oldRule.Successor)
                    {
                        match = true;
                    }
                }

                if (!match)
                {
                    res.Add(oldRule);
                }
            }

            return res;
        }
        
        public List<Objects.Rule> getBrandNewRules(List<Objects.Rule> newRules)
        {
            List<Objects.Rule> res = new List<Objects.Rule>();
            foreach (Objects.Rule newRule in newRules)
            {
                bool match = false;
                foreach (Objects.Rule oldRule in m_rules)
                {
                    if (newRule.Verb == oldRule.Verb && newRule.Predecessor == oldRule.Predecessor &&
                        newRule.Successor == oldRule.Successor)
                    {
                        match = true;
                    }
                }

                if (!match)
                {
                    res.Add(newRule);
                }
            }

            return res;
        }

        public Dictionary<uint, Entity> setRules(Dictionary<uint, Entity> entities, List<List<List<Entity>>> map, ParticleSystem particleSystem, SoundEffect winChanged, SoundEffect youChanged)
        {
            List<Objects.Rule> removeRules = new List<Objects.Rule>();
            List<Objects.Rule> addRules = new List<Objects.Rule>();
            List<Objects.Rule> newRules = scanRules(map);

            foreach (Objects.Rule outdated in getOutdatedRules(newRules))
            {
                removeRules.Add(outdated);
                m_rules.Remove(outdated);
            }
            foreach (Objects.Rule brandNew in getBrandNewRules(newRules))
            {
                addRules.Add(brandNew);
                m_rules.Add(brandNew);
            }
            return addRemoveRules(removeRules, addRules, entities, particleSystem, winChanged, youChanged);
        }
        
        public Dictionary<uint, Entity> addRemoveRules(List<Objects.Rule> removeRules, List<Objects.Rule> addRules, Dictionary<uint, Entity> entities, ParticleSystem particleSystem, SoundEffect winChanged, SoundEffect youChanged)
        {
            Dictionary<uint, Entity> res = new Dictionary<uint, Entity>();
            bool winChange = false;
            bool youChange = false;
            foreach (KeyValuePair<uint, Entity> entity in entities)
            {
                bool changed = false;
                foreach (Objects.Rule rule in removeRules)
                {
                    if (rule.Predecessor != null && rule.Predecessor.ContainsComponent<Noun>() &&
                        rule.Successor != null && rule.Successor.ContainsComponent<Adjective>())
                    {
                        Nouns nounType = rule.Predecessor.GetComponent<Noun>().Type;
                        Adjectives adjType = rule.Successor.GetComponent<Adjective>().Type;
                        if (!entity.Value.ContainsComponent<Text>() && entity.Value.ContainsComponent<Noun>() &&
                            entity.Value.GetComponent<Noun>().Type == nounType)
                        {
                            if (adjType == Adjectives.Kill)
                            {
                                entity.Value.Remove<Kills>();
                                entity.Value.ContainsComponent<Kills>();
                                changed = true;
                            }

                            if (adjType == Adjectives.Push)
                            {
                                entity.Value.Remove<Pushable>();
                                changed = true;
                            }

                            if (adjType == Adjectives.Sink)
                            {
                                entity.Value.Remove<Sinks>();
                                changed = true;
                            }

                            if (adjType == Adjectives.Stop)
                            {
                                entity.Value.Remove<Stop>();
                                changed = true;
                            }

                            if (adjType == Adjectives.You)
                            {
                                entity.Value.Remove<Moveable>();
                                changed = true;
                            }

                            if (adjType == Adjectives.Win)
                            {
                                entity.Value.Remove<Win>();
                                changed = true;
                            }
                        }
                    }
                }
                
                foreach (Objects.Rule rule in addRules)
                {
                    if (rule.Predecessor != null && rule.Predecessor.ContainsComponent<Noun>() &&
                        rule.Successor != null && rule.Successor.ContainsComponent<Adjective>())
                    {
                        Nouns nounType = rule.Predecessor.GetComponent<Noun>().Type;
                        Adjectives adjType = rule.Successor.GetComponent<Adjective>().Type;
                        if (!entity.Value.ContainsComponent<Text>() && entity.Value.ContainsComponent<Noun>() &&
                            entity.Value.GetComponent<Noun>().Type == nounType)
                        {
                            if (adjType == Adjectives.Kill)
                            {
                                if (!entity.Value.ContainsComponent<Kills>())
                                {
                                    entity.Value.Remove<Burnable>();
                                    entity.Value.Add(new Kills());
                                    changed = true;
                                }
                            }

                            if (adjType == Adjectives.Push)
                            {
                                if (!entity.Value.ContainsComponent<Pushable>())
                                {
                                    entity.Value.Add(new Pushable());
                                    changed = true;
                                }
                            }

                            if (adjType == Adjectives.Sink)
                            {
                                if (!entity.Value.ContainsComponent<Sinks>())
                                {
                                    entity.Value.Remove<Sinkable>();
                                    entity.Value.Add(new Sinks());
                                    changed = true;
                                }
                            }

                            if (adjType == Adjectives.Stop)
                            {
                                if (!entity.Value.ContainsComponent<Stop>())
                                {
                                    entity.Value.Add(new Stop());
                                    changed = true;
                                }
                            }

                            if (adjType == Adjectives.You)
                            {
                                if (!entity.Value.ContainsComponent<Moveable>())
                                {
                                    entity.Value.Add(new Moveable());
                                    changed = true;
                                    particleSystem.youChanged(new List<Entity> {entity.Value});
                                    youChange = true;
                                }
                            }

                            if (adjType == Adjectives.Win)
                            {
                                if (!entity.Value.ContainsComponent<Win>())
                                {
                                    entity.Value.Add(new Win());
                                    changed = true;
                                    particleSystem.winChanged(new List<Entity> {entity.Value});
                                    winChange = true;
                                }
                            }
                        }
                    }
                    else if (rule.Predecessor != null && rule.Predecessor.ContainsComponent<Noun>() &&
                             rule.Successor != null && rule.Successor.ContainsComponent<Noun>())
                    {
                        Nouns nounType = rule.Predecessor.GetComponent<Noun>().Type;
                        Nouns secondNounType = rule.Successor.GetComponent<Noun>().Type;
                        if (!entity.Value.ContainsComponent<Text>() && entity.Value.ContainsComponent<Noun>() &&
                            entity.Value.GetComponent<Noun>().Type == nounType)
                        {
                            if (secondNounType == Nouns.Flag)
                            {
                                int[] time = {500, 500, 500};
                                Texture2D texture = m_contentManager.Load<Texture2D>("Images/flag");
                                Vector2 size = entity.Value.GetComponent<Sprite>().Size;
                                entity.Value.Remove<Sprite>();
                                entity.Value.Add(new Sprite(texture, size, Color.Yellow, time));
                                entity.Value.Remove<Noun>();
                                entity.Value.Add(new Noun(Nouns.Flag));
                                changed = true;
                            }
                            if (secondNounType == Nouns.Lava)
                            {
                                int[] time = {500, 500, 500};
                                Texture2D texture = m_contentManager.Load<Texture2D>("Images/lava");
                                Vector2 size = entity.Value.GetComponent<Sprite>().Size;
                                entity.Value.Remove<Sprite>();
                                entity.Value.Add(new Sprite(texture, size, Color.Orange, time));
                                entity.Value.Remove<Noun>();
                                entity.Value.Add(new Noun(Nouns.Lava));
                                changed = true;
                            }
                            if (secondNounType == Nouns.Rock)
                            {
                                int[] time = {500, 500, 500};
                                Texture2D texture = m_contentManager.Load<Texture2D>("Images/rock");
                                Vector2 size = entity.Value.GetComponent<Sprite>().Size;
                                entity.Value.Remove<Sprite>();
                                entity.Value.Add(new Sprite(texture, size, Color.SaddleBrown, time));
                                entity.Value.Remove<Noun>();
                                entity.Value.Add(new Noun(Nouns.Rock));
                                changed = true;
                            }
                            if (secondNounType == Nouns.Wall)
                            {
                                int[] time = {500, 500, 500};
                                Texture2D texture = m_contentManager.Load<Texture2D>("Images/wall");
                                Vector2 size = entity.Value.GetComponent<Sprite>().Size;
                                entity.Value.Remove<Sprite>();
                                entity.Value.Add(new Sprite(texture, size, Color.Gray, time));
                                entity.Value.Remove<Noun>();
                                entity.Value.Add(new Noun(Nouns.Wall));
                                changed = true;
                            }
                            if (secondNounType == Nouns.Water)
                            {
                                int[] time = {500, 500, 500};
                                Texture2D texture = m_contentManager.Load<Texture2D>("Images/water");
                                Vector2 size = entity.Value.GetComponent<Sprite>().Size;
                                entity.Value.Remove<Sprite>();
                                entity.Value.Add(new Sprite(texture, size, Color.Blue, time));
                                entity.Value.Remove<Noun>();
                                entity.Value.Add(new Noun(Nouns.Water));
                                changed = true;
                            }
                            if (secondNounType == Nouns.BigBlue)
                            {
                                int[] time = {750};
                                Texture2D texture = m_contentManager.Load<Texture2D>("Images/BigBlue");
                                Vector2 size = entity.Value.GetComponent<Sprite>().Size;
                                entity.Value.Remove<Sprite>();
                                entity.Value.Add(new Sprite(texture, size, Color.White, time));
                                entity.Value.Remove<Noun>();
                                entity.Value.Add(new Noun(Nouns.BigBlue));
                                changed = true;
                            }
                        }
                    }
                }

                if (changed)
                {
                    res.Add(entity.Key, entity.Value);
                }
            }

            if (winChange)
            {
                winChanged.Play();
            }

            if (youChange)
            {
                youChanged.Play();
            }
            return res;
        }
    }
}