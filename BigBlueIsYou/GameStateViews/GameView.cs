using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CS5410.Components;
using CS5410.Input;
using CS5410.Objects;
using CS5410.Particles;
using CS5410.Systems;
using Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CS5410
{
    public class GameView : GameStateView
    {
        private static GraphicsDevice m_graphicsDevice;
        private ContentManager m_contentManager;
        private GameStateEnum m_nextStateEnum;
        private bool loading;
        private bool selecting;
        private bool registerGameCommands;
        private bool registerLevelCommands;
        private bool enterPressed;
        private bool gameWon;
        private Controls m_keyboardLayout;
        private KeyboardInput m_inputHandler;
        private List<List<List<Entity>>> m_entityMap;
        
        private int windowSize;
        private int gridWidth;
        private int gridHeight;
        private int cellSize;
        private int heightBuffer;
        private int widthBuffer;
        private int m_currentSelection;
        private List<Level> m_levels;
        private Level m_level;

        // Fonts
        private SpriteFont m_fontMenu;
        
        // Sounds
        private SoundEffect m_stepSound;
        private SoundEffect m_winSound;
        private Song m_backgroundSong;
        private SoundEffect m_winChangeSound;
        private SoundEffect m_youChangeSound;

        private Dictionary<uint, Entity> m_entities;

        // Systems
        private List<Systems.System> m_systems;
        private Movement m_movementSystem;
        private AnimatedSprite m_animatedSpriteSystem;
        private RenderSprite m_renderSpriteSystem;
        private ParticleSystem m_particleSystem;
        private Rules m_ruleSystem;
        
        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphicsDevice = graphicsDevice;
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            m_entities = new Dictionary<uint, Entity>();
            m_levels = new List<Level>();
            m_systems = new List<Systems.System>();
            enterPressed = true;
            selecting = true;
            registerGameCommands = false;
            registerLevelCommands = true;
            loadLayout();
            readLevels();

            windowSize = (int) (m_graphics.PreferredBackBufferHeight * 0.9);
            heightBuffer = (m_graphics.PreferredBackBufferHeight - windowSize) / 2;
            widthBuffer = (m_graphics.PreferredBackBufferWidth - windowSize) / 2;

            m_inputHandler = new KeyboardInput();
            m_inputHandler.registerCommand("back", Keys.Escape, true, navigateBack);
            
            resetSystems();
        }
        
        private void resetSystems()
        {
            m_movementSystem = new Movement(new[] {typeof(Moveable), typeof(Stop)}, m_entityMap);
            m_animatedSpriteSystem = new AnimatedSprite(new [] {typeof(Sprite)});
            m_renderSpriteSystem = new RenderSprite(m_spriteBatch, new [] {typeof(Sprite)}, heightBuffer, widthBuffer);
            m_ruleSystem = new Rules(new List<Rule>(), m_contentManager);
            m_systems.Add(m_movementSystem);
            m_systems.Add(m_animatedSpriteSystem);
            m_systems.Add(m_renderSpriteSystem);
        }
        
        private void createEntities()
        {
            m_entities = new Dictionary<uint, Entity>();
            foreach (KeyValuePair<char,Vector2> ent in m_level.Entities)
            {
                Entity entity = new Entity();
                if (ent.Key == 'h')
                {
                    entity = createHedge(ent.Value);
                }
                else if (ent.Key == 'b')
                {
                    entity = createBigBlue(ent.Value);
                }
                else if (ent.Key == 'w')
                {
                    entity = createWall(ent.Value);
                }
                else if (ent.Key == 'r')
                {
                    entity = createRock(ent.Value);
                }
                else if (ent.Key == 'f')
                {
                    entity = createFlag(ent.Value);
                }
                else if (ent.Key == 'l')
                {
                    entity = createFloor(ent.Value);
                }
                else if (ent.Key == 'g')
                {
                    entity = createGrass(ent.Value);
                }
                else if (ent.Key == 'a')
                {
                    entity = createWater(ent.Value);
                }
                else if (ent.Key == 'v')
                {
                    entity = createLava(ent.Value);
                }
                else if (ent.Key == 'W')
                {
                    entity = createWallText(ent.Value);
                }
                else if (ent.Key == 'R')
                {
                    entity = createRockText(ent.Value);
                }
                else if (ent.Key == 'F')
                {
                    entity = createFlagText(ent.Value);
                }
                else if (ent.Key == 'B')
                {
                    entity = createBabaText(ent.Value);
                }
                else if (ent.Key == 'I')
                {
                    entity = createIsText(ent.Value);
                }
                else if (ent.Key == 'S')
                {
                    entity = createStopText(ent.Value);
                }
                else if (ent.Key == 'P')
                {
                    entity = createPushText(ent.Value);
                }
                else if (ent.Key == 'V')
                {
                    entity = createLavaText(ent.Value); 
                }
                else if (ent.Key == 'A')
                {
                    entity = createWaterText(ent.Value);
                }
                else if (ent.Key == 'Y')
                {
                    entity = createYouText(ent.Value);
                }
                else if (ent.Key == 'X')
                {
                    entity = createWinText(ent.Value);
                }
                else if (ent.Key == 'N')
                {
                    entity = createSinkText(ent.Value);
                }
                else if (ent.Key == 'K')
                {
                    entity = createKillText(ent.Value);
                }
                m_entities.Add(entity.Id, entity);
                m_entityMap[(int)entity.GetComponent<Position>().Coordinates.X][(int)entity.GetComponent<Position>().Coordinates.Y].Add(entity);
            }
        }
        
        // Create Game Entities
        public Entity createBigBlue(Vector2 position)
        {
            Entity blue = new Entity();
            int[] time = {750};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/BigBlue");
            blue.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.White, time));
            blue.Add(new Position(new Vector2(position.X, position.Y)));
            blue.Add(new Noun(Nouns.BigBlue));
            blue.Add(new Sinkable());
            blue.Add(new Burnable());
            return blue;
        }
        public Entity createWall(Vector2 position)
        {
            Entity wall = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/wall");
            wall.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Gray, time));
            wall.Add(new Position(new Vector2(position.X, position.Y)));
            wall.Add(new Noun(Nouns.Wall));
            wall.Add(new Burnable());
            wall.Add(new Sinkable());
            return wall;
        }
        public Entity createRock(Vector2 position)
        {
            Entity rock = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/rock");
            rock.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.SaddleBrown, time));
            rock.Add(new Position(new Vector2(position.X, position.Y)));
            rock.Add(new Noun(Nouns.Rock));
            rock.Add(new Sinkable());
            return rock;
        }
        public Entity createFlag(Vector2 position)
        {
            Entity flag = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/flag");
            flag.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Yellow, time));
            flag.Add(new Position(new Vector2(position.X, position.Y)));
            flag.Add(new Noun(Nouns.Flag));
            flag.Add(new Burnable());
            flag.Add(new Sinkable());
            return flag;
        }
        public Entity createFloor(Vector2 position)
        {
            Entity floor = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/floor");
            floor.Add(new Sprite(texture, new Vector2(cellSize, cellSize), new Color(41, 41, 41), time));
            floor.Add(new Position(new Vector2(position.X, position.Y)));
            return floor;
        }
        public Entity createGrass(Vector2 position)
        {
            Entity grass = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/grass");
            grass.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Green, time));
            grass.Add(new Position(new Vector2(position.X, position.Y)));
            return grass;
        }
        public Entity createWater(Vector2 position)
        {
            Entity water = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/water");
            water.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Blue, time));
            water.Add(new Position(new Vector2(position.X, position.Y)));
            water.Add(new Sinks());
            water.Add(new Noun(Nouns.Water));
            return water;
        }
        public Entity createLava(Vector2 position)
        {
            Entity lava = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/lava");
            lava.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Orange, time));
            lava.Add(new Position(new Vector2(position.X, position.Y)));
            lava.Add(new Kills());
            lava.Add(new Noun(Nouns.Lava));
            return lava;
        }
        public Entity createHedge(Vector2 position)
        {
            Entity hedge = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/hedge");
            hedge.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Green, time));
            hedge.Add(new Position(new Vector2(position.X, position.Y)));
            hedge.Add(new Stop());
            return hedge;
        }
        
        // Create Text Tiles
        public Entity createWallText(Vector2 position)
        {
            Entity wallText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-wall");
            wallText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Gray, time));
            wallText.Add(new Position(new Vector2(position.X, position.Y)));
            wallText.Add(new Pushable());
            wallText.Add(new Text(Texts.Noun));
            wallText.Add(new Noun(Nouns.Wall));
            return wallText;
        }
        public Entity createRockText(Vector2 position)
        {
            Entity rockText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-rock");
            rockText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.SaddleBrown, time));
            rockText.Add(new Position(new Vector2(position.X, position.Y)));
            rockText.Add(new Pushable());
            rockText.Add(new Text(Texts.Noun));
            rockText.Add(new Noun(Nouns.Rock));
            return rockText;
        }
        public Entity createFlagText(Vector2 position)
        {
            Entity flagText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-flag");
            flagText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Yellow, time));
            flagText.Add(new Position(new Vector2(position.X, position.Y)));
            flagText.Add(new Pushable());
            flagText.Add(new Text(Texts.Noun));
            flagText.Add(new Noun(Nouns.Flag));
            return flagText;
        }
        public Entity createBabaText(Vector2 position)
        {
            Entity babaText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-baba");
            babaText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.White, time));
            babaText.Add(new Position(new Vector2(position.X, position.Y)));
            babaText.Add(new Pushable());
            babaText.Add(new Text(Texts.Noun));
            babaText.Add(new Noun(Nouns.BigBlue));
            return babaText;
        }
        public Entity createIsText(Vector2 position)
        {
            Entity isText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-is");
            isText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.White, time));
            isText.Add(new Position(new Vector2(position.X, position.Y)));
            isText.Add(new Pushable());
            isText.Add(new Text(Texts.Verb));
            return isText;
        }
        public Entity createStopText(Vector2 position)
        {
            Entity stopText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-stop");
            stopText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Green, time));
            stopText.Add(new Position(new Vector2(position.X, position.Y)));
            stopText.Add(new Pushable());
            stopText.Add(new Text(Texts.Adjective));
            stopText.Add(new Adjective(Adjectives.Stop));
            return stopText;
        }
        public Entity createPushText(Vector2 position)
        {
            Entity pushText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-push");
            pushText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.SaddleBrown, time));
            pushText.Add(new Position(new Vector2(position.X, position.Y)));
            pushText.Add(new Pushable());
            pushText.Add(new Text(Texts.Adjective));
            pushText.Add(new Adjective(Adjectives.Push));
            return pushText;
        }
        public Entity createLavaText(Vector2 position)
        {
            Entity lavaText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-lava");
            lavaText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.OrangeRed, time));
            lavaText.Add(new Position(new Vector2(position.X, position.Y)));
            lavaText.Add(new Pushable());
            lavaText.Add(new Text(Texts.Noun));
            lavaText.Add(new Noun(Nouns.Lava));
            return lavaText;
        }
        public Entity createWaterText(Vector2 position)
        {
            Entity waterText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-water");
            waterText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Blue, time));
            waterText.Add(new Position(new Vector2(position.X, position.Y)));
            waterText.Add(new Pushable());
            waterText.Add(new Text(Texts.Noun));
            waterText.Add(new Noun(Nouns.Water));
            return waterText;
        }
        public Entity createYouText(Vector2 position)
        {
            Entity youText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-you");
            youText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Pink, time));
            youText.Add(new Position(new Vector2(position.X, position.Y)));
            youText.Add(new Pushable());
            youText.Add(new Text(Texts.Adjective));
            youText.Add(new Adjective(Adjectives.You));
            return youText;
        }
        public Entity createWinText(Vector2 position)
        {
            Entity winText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-win");
            winText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Yellow, time));
            winText.Add(new Position(new Vector2(position.X, position.Y)));
            winText.Add(new Pushable());
            winText.Add(new Text(Texts.Adjective));
            winText.Add(new Adjective(Adjectives.Win));
            return winText;
        }
        public Entity createSinkText(Vector2 position)
        {
            Entity sinkText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-sink");
            sinkText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Blue, time));
            sinkText.Add(new Position(new Vector2(position.X, position.Y)));
            sinkText.Add(new Pushable());
            sinkText.Add(new Text(Texts.Adjective));
            sinkText.Add(new Adjective(Adjectives.Sink));
            return sinkText;
        }
        public Entity createKillText(Vector2 position)
        {
            Entity killText = new Entity();
            int[] time = {500, 500, 500};
            Texture2D texture = m_contentManager.Load<Texture2D>("Images/word-kill");
            killText.Add(new Sprite(texture, new Vector2(cellSize, cellSize), Color.Red, time));
            killText.Add(new Position(new Vector2(position.X, position.Y)));
            killText.Add(new Pushable());
            killText.Add(new Text(Texts.Adjective));
            killText.Add(new Adjective(Adjectives.Kill));
            return killText;
        }
        
        public void systemUpdateEntities(Dictionary<uint, Entity> entities)
        {
            foreach (KeyValuePair<uint, Entity> entity in entities)
            {
                foreach (Systems.System system in m_systems)
                {
                    bool removed = system.Remove(entity.Key);
                    bool added = system.Add(entity.Value);
                }
            }
        }
        
        public void systemRemoveEntities(Dictionary<uint, Entity> entities)
        {
            foreach (KeyValuePair<uint, Entity> entity in entities)
            {
                foreach (Systems.System system in m_systems)
                {
                    bool removed = system.Remove(entity.Key);
                }
            }
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_contentManager = contentManager;
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/Font");
            m_particleSystem = new ParticleSystem(m_contentManager.Load<Texture2D>("Images/lava"), heightBuffer, widthBuffer);
            m_stepSound = contentManager.Load<SoundEffect>("Sounds/step");
            m_winSound = contentManager.Load<SoundEffect>("Sounds/win");
            m_backgroundSong = contentManager.Load<Song>("Sounds/background");
            m_winChangeSound = contentManager.Load<SoundEffect>("Sounds/winChanged");
            m_youChangeSound = contentManager.Load<SoundEffect>("Sounds/youChanged");
        }
        
        private void loadLayout()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    loadLayoutAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }
        
        private void readLevels()
        {
            IEnumerable<string> lines = File.ReadLines("Content/levels-all.bbiy");
            IEnumerator<string> lineEnum = lines.GetEnumerator();
            while (lineEnum.MoveNext())
            {
                Level level = new Level();
                level.Name = lineEnum.Current;
                lineEnum.MoveNext();
                level.Width = Int32.Parse(lineEnum.Current.Split("x")[0]);
                level.Height = Int32.Parse(lineEnum.Current.Split("x")[1]);
                lineEnum.MoveNext();
                int row = 0;
                List<KeyValuePair<char,Vector2>> entities = new List<KeyValuePair<char, Vector2>>();
                while (row < (2 * level.Height) - 1)
                {
                    int col = 0;
                    foreach (char c in lineEnum.Current)
                    {
                        if (c != ' ')
                        {
                            entities.Add(new KeyValuePair<char, Vector2>(c, new Vector2(col, row % level.Height)));
                        }
                        col++;
                    }
                    lineEnum.MoveNext();
                    row++;
                }
                level.Entities = entities;
                m_levels.Add(level);
            }
        }

        private async Task loadLayoutAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("layout.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("layout.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(Controls));
                                    m_keyboardLayout = (Controls)mySerializer.Deserialize(fs);
                                }
                            }
                        }
                        else
                        {
                            m_keyboardLayout = new Controls();
                            m_keyboardLayout.Down = Keys.S;
                            m_keyboardLayout.Up = Keys.W;
                            m_keyboardLayout.Right = Keys.D;
                            m_keyboardLayout.Left = Keys.A;
                            m_keyboardLayout.Reset = Keys.R;
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }

                this.loading = false;
            });
        }

        private void checkWin()
        {
            bool won = false;
            for (int i = 0; i < m_entityMap.Count; i++)
            {
                for (int j = 0; j < m_entityMap[i].Count; j++)
                {
                    bool win = false;
                    bool player = false;
                    foreach (Entity entity in m_entityMap[i][j])
                    {
                        if (entity.ContainsComponent<Moveable>())
                        {
                            player = true;
                        }

                        if (entity.ContainsComponent<Win>())
                        {
                            win = true;
                        }
                    }

                    if (win && player)
                    {
                        MediaPlayer.Stop();
                        m_winSound.Play();
                        won = true;
                    }
                }
            }

            if (won)
            {
                gameWon = true;
                m_particleSystem.playerWon(
                    new Rectangle(
                        (m_graphics.PreferredBackBufferWidth - windowSize) / 2, 
                        (m_graphics.PreferredBackBufferHeight - windowSize) / 2,
                        (int) windowSize,
                        (int) windowSize)
                    );
            }
        }

        private void updateRules()
        {
            Dictionary<uint, Entity> updatedEntities = m_ruleSystem.setRules(m_entities, m_entityMap, m_particleSystem, m_winChangeSound, m_youChangeSound);
            if (updatedEntities.Count > 0)
            {
                systemUpdateEntities(updatedEntities);
            }
        }

        private void checkDestroy()
        {
            Dictionary<uint, Entity> updated = new Dictionary<uint, Entity>();
            for (int i = 0; i < m_entityMap.Count; i++)
            {
                for (int j = 0; j < m_entityMap[i].Count; j++)
                {
                    Entity sinks = null;
                    Entity kills = null;
                    foreach (Entity entity in m_entityMap[i][j])
                    {
                        if (entity.ContainsComponent<Sinks>())
                        {
                            sinks = entity;
                        }
                        if (entity.ContainsComponent<Kills>())
                        {
                            kills = entity;
                        }
                    }

                    if (sinks != null)
                    {
                        bool sunk = false;
                        List<Entity> removeMe = new List<Entity>();
                        foreach (Entity entity in m_entityMap[i][j])
                        {
                            if (entity.ContainsComponent<Sinkable>())
                            { 
                                sunk = true;
                                if (!updated.ContainsKey(sinks.Id))
                                {
                                    updated.Add(sinks.Id, sinks);
                                }

                                if (!updated.ContainsKey(entity.Id))
                                {
                                    updated.Add(entity.Id, entity);
                                }
                                removeMe.Add(entity);
                                removeMe.Add(sinks);
                            }
                        }

                        foreach (Entity entity in removeMe)
                        {
                            m_entityMap[i][j].Remove(entity);
                            m_entities.Remove(entity.Id);
                        }

                        if (sunk)
                        {
                            m_particleSystem.objectSinks(new List<Entity> {sinks});
                        }
                    }
                    
                    if (kills != null)
                    {
                        bool killed = false;
                        List<Entity> removeMe = new List<Entity>();
                        foreach (Entity entity in m_entityMap[i][j])
                        {
                            if (entity.ContainsComponent<Burnable>())
                            {
                                killed = true;
                                if (!updated.ContainsKey(entity.Id))
                                {
                                    updated.Add(entity.Id, entity);
                                }
                                removeMe.Add(entity);
                            }
                        }

                        foreach (Entity entity in removeMe)
                        {
                            m_entityMap[i][j].Remove(entity);
                            m_entities.Remove(entity.Id);
                        }

                        if (killed)
                        {
                            m_particleSystem.objectDeath(new List<Entity> {kills});
                        }
                    }
                }
            }
            systemRemoveEntities(updated);
        }
        
        
        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_nextStateEnum = GameStateEnum.GamePlay;
            if (Keyboard.GetState().GetPressedKeyCount() == 0)
            {
                enterPressed = false;
            }

            if (!enterPressed)
            {
                m_inputHandler.Update(gameTime);
            }

            if (registerLevelCommands)
            {
                m_inputHandler.registerCommand("up", Keys.Up, true, navigateUp);
                m_inputHandler.registerCommand("down", Keys.Down, true, navigateDown);
                m_inputHandler.registerCommand("enter", Keys.Enter, true, startGame);
                registerLevelCommands = false;
            }
            if (registerGameCommands)
            {
                m_inputHandler.registerCommand("right", m_keyboardLayout.Right, true, moveRight);
                m_inputHandler.registerCommand("left", m_keyboardLayout.Left, true, moveLeft);
                m_inputHandler.registerCommand("up", m_keyboardLayout.Up, true, moveUp);
                m_inputHandler.registerCommand("down", m_keyboardLayout.Down, true, moveDown);
                m_inputHandler.registerCommand("reset", m_keyboardLayout.Reset, true, resetLevel);
                registerGameCommands = false;
            }
            
            return m_nextStateEnum;
        }
        
        public override void update(GameTime gameTime)
        {
            m_animatedSpriteSystem.Update(gameTime);
            m_particleSystem.Update(gameTime);
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            if (selecting)
            {
                float bottom = drawMenuItem(m_fontMenu, "Select a Level", 200, Color.LimeGreen);
                int index = 0;
                foreach (Level level in m_levels)
                {
                    bottom = drawMenuItem(m_fontMenu, level.Name, bottom, m_currentSelection == index ? Color.LimeGreen : Color.Green);
                    index++;
                }
                m_spriteBatch.End();
                return;
            }
            m_particleSystem.Draw(m_spriteBatch);
            m_renderSpriteSystem.Update(gameTime);
            m_spriteBatch.End();


        }
        
        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color);

            return y + stringSize.Y;
        }


        // Callback Functions
        public void navigateBack(GameTime gameTime)
        {
            MediaPlayer.Stop();
            if (!selecting)
            {
                selecting = true;
                registerLevelCommands = true;
            }
            else
            {
                m_nextStateEnum = GameStateEnum.MainMenu;
            }
        }
        
        private void navigateUp(GameTime gameTime)
        {
            if (m_currentSelection > 0)
            {
                m_currentSelection -= 1;
            }
        }

        private void navigateDown(GameTime gameTime)
        {
            if (m_currentSelection < m_levels.Count - 1)
            {
                m_currentSelection += 1;
            }
        }

        private void startGame(GameTime gameTime)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(m_backgroundSong);
            selecting = false;
            gameWon = false;
            m_level = m_levels[m_currentSelection];
            gridHeight = m_level.Height;
            gridWidth = m_level.Width;
            cellSize = (windowSize / gridWidth);
            m_entityMap = new List<List<List<Entity>>>();
            for (int i = 0; i < gridWidth; i++)
            {
                m_entityMap.Add(new List<List<Entity>>());
                for (int j = 0; j < gridHeight; j++)
                {
                    m_entityMap[i].Add(new List<Entity>());
                }
            }
            createEntities();
            resetSystems();
            systemUpdateEntities(m_entities);
            Dictionary<uint, Entity> updatedEntities = m_ruleSystem.setRules(m_entities, m_entityMap, m_particleSystem, m_winChangeSound, m_youChangeSound);
            if (updatedEntities.Count > 0)
            {
                systemUpdateEntities(updatedEntities);
            }
            registerGameCommands = true;
        }

        public void moveUp(GameTime gameTime)
        {
            if (!gameWon)
            {
                Dictionary<Entity, Position> updated = m_movementSystem.moveUp(m_entityMap);
                foreach (KeyValuePair<Entity, Position> update in updated)
                {
                    if (update.Value.GetType() == typeof(Position))
                    {
                        m_stepSound.Play();
                        m_entityMap[(int) update.Value.Coordinates.X][(int) update.Value.Coordinates.Y + 1].Remove(update.Key);
                        update.Key.GetComponent<Position>().Coordinates = new Vector2(
                            update.Value.Coordinates.X,
                            update.Value.Coordinates.Y);
                        m_entityMap[(int) update.Value.Coordinates.X][(int) update.Value.Coordinates.Y].Add(update.Key);
                    }
                }
                checkDestroy();
                checkWin();
                updateRules();
            }
        }

        public void moveDown(GameTime gameTime)
        {
            if (!gameWon)
            {
                Dictionary<Entity, Position> updated = m_movementSystem.moveDown(m_entityMap);
                foreach (KeyValuePair<Entity, Position> update in updated)
                {
                    m_stepSound.Play();
                    if (update.Value.GetType() == typeof(Position))
                    {
                        m_entityMap[(int) update.Value.Coordinates.X][(int) update.Value.Coordinates.Y - 1].Remove(update.Key);
                        update.Key.GetComponent<Position>().Coordinates = new Vector2(
                            update.Value.Coordinates.X,
                            update.Value.Coordinates.Y);
                        m_entityMap[(int) update.Value.Coordinates.X][(int) update.Value.Coordinates.Y].Add(update.Key);
                    }
                }
                checkDestroy();
                checkWin();
                updateRules();
            }
        }

        public void moveRight(GameTime gameTime)
        {
            if (!gameWon)
            {
                Dictionary<Entity, Position> updated = m_movementSystem.moveRight(m_entityMap);
                foreach (KeyValuePair<Entity, Position> update in updated)
                {
                    if (update.Value.GetType() == typeof(Position))
                    {
                        m_stepSound.Play();
                        m_entityMap[(int) update.Value.Coordinates.X - 1][(int) update.Value.Coordinates.Y].Remove(update.Key);
                        update.Key.GetComponent<Position>().Coordinates = new Vector2(
                            update.Value.Coordinates.X,
                            update.Value.Coordinates.Y);
                        m_entityMap[(int) update.Value.Coordinates.X][(int) update.Value.Coordinates.Y].Add(update.Key);
                    }
                }
                checkDestroy();
                checkWin();
                updateRules();
            }
        }

        public void moveLeft(GameTime gameTime)
        {
            if (!gameWon)
            {
                Dictionary<Entity, Position> updated = m_movementSystem.moveLeft(m_entityMap);
                foreach (KeyValuePair<Entity, Position> update in updated)
                {
                    if (update.Value.GetType() == typeof(Position))
                    {
                        m_stepSound.Play();
                        m_entityMap[(int) update.Value.Coordinates.X + 1][(int) update.Value.Coordinates.Y].Remove(update.Key);
                        update.Key.GetComponent<Position>().Coordinates = new Vector2(
                            update.Value.Coordinates.X,
                            update.Value.Coordinates.Y);
                        m_entityMap[(int) update.Value.Coordinates.X][(int) update.Value.Coordinates.Y].Add(update.Key);
                    }
                }
                checkDestroy();
                checkWin();
                updateRules();
            }
        }

        public void resetLevel(GameTime gameTime)
        {
            resetSystems();
            startGame(gameTime);
        }
    }
}
