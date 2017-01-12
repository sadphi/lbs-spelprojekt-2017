using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace inkArenaGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Texture2D pixel;
        public static ContentManager contentLoader;

        List<Player> players;

        Texture2D bulletTexture;
        Texture2D playerTexture;
        Texture2D menuTexture;
        Texture2D credits;

        GamePadState newGamePadState;
        GamePadState oldGamePadState;

        KeyboardState newKeyboardState;
        KeyboardState oldKeyboardState;

        Button[] btnArray;

        Map map;

        int currentButton;
        int currentLevelHighlighted;
        GameState currentState;

        int numOfPlayers;

        enum GameState
        {
            MainMenu,
            LevelSelect,
            Credits,
            Playing,
            Paused,
            GameOver
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            contentLoader = Content;

            this.Window.Title = "Hillbilly Havok";
            this.IsMouseVisible = false;

            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1056;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferMultiSampling = true;
            this.graphics.ApplyChanges();

            currentState = GameState.MainMenu;

            players = new List<Player>();

            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    numOfPlayers++;
                }
            }


            map = new Map();

            currentButton = 0;
            currentLevelHighlighted = 0;

            btnArray = new Button[3];
            btnArray[0] = new Button(960, 350, Content.Load<Texture2D>("Graphics/Buttons/Playbutton"), () => { currentState = GameState.LevelSelect; });
            btnArray[1] = new Button(960, 550, Content.Load<Texture2D>("Graphics/Buttons/Creditsbutton"), () => { currentState = GameState.Credits; });
            btnArray[2] = new Button(960, 750, Content.Load<Texture2D>("Graphics/Buttons/Quitbutton"), () => { Environment.Exit(0); });

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            // TODO: use this.Content to load your game content here
            playerTexture = Content.Load<Texture2D>("Graphics/Players/Player1Standing");
            menuTexture = Content.Load<Texture2D>("Graphics/Menu2");
            bulletTexture = Content.Load<Texture2D>("Graphics/GunProjectile1");
            credits = Content.Load<Texture2D>("Graphics/credits");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            newGamePadState = GamePad.GetState(PlayerIndex.One);
            newKeyboardState = Keyboard.GetState();
            
            switch (currentState)
            {
                case GameState.MainMenu:
                    if ((newGamePadState.DPad.Down == ButtonState.Pressed && oldGamePadState.DPad.Down == ButtonState.Released) || 
                        (newKeyboardState.IsKeyDown(Keys.Down) && oldKeyboardState.IsKeyUp(Keys.Down)))
                    {
                        currentButton++;
                    }

                    if ((newGamePadState.DPad.Up == ButtonState.Pressed && oldGamePadState.DPad.Up == ButtonState.Released) || 
                        (newKeyboardState.IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyUp(Keys.Up)))
                    {
                        currentButton--;
                    }

                    if (currentButton >= btnArray.Length)
                    {
                        currentButton = 0;
                    }

                    if (currentButton <= -1)
                    {
                        currentButton = btnArray.Length - 1;
                    }

                    if ((newGamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released) || 
                        (newKeyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter)))
                    {
                        btnArray[currentButton].act();
                    }
                    break;

                case GameState.LevelSelect:
                    if ((newGamePadState.DPad.Right == ButtonState.Pressed && oldGamePadState.DPad.Right == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Right) && oldKeyboardState.IsKeyUp(Keys.Right)))
                    {
                        currentLevelHighlighted++;
                    }

                    if ((newGamePadState.DPad.Left == ButtonState.Pressed && oldGamePadState.DPad.Left == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Left) && oldKeyboardState.IsKeyUp(Keys.Left)))
                    {
                        currentLevelHighlighted--;
                    }

                    if (currentLevelHighlighted >= Map.levelTextures.Count())
                    {
                        currentLevelHighlighted = 0;
                    }

                    if (currentButton <= -1)
                    {
                        currentLevelHighlighted = Map.levelTextures.Count() - 1;
                    }

                    if ((newGamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter)))
                    {
                        Map.ChangeLevel(currentLevelHighlighted);
                        currentState = GameState.Playing;
                        players.Add(new Player((PlayerIndex)2, 980));
                        for (int i = 0; i < numOfPlayers; i++)
                        {
                            players.Add(new Player((PlayerIndex)i, 100 + i * 1720));
                        }
                    }
                    break;

                case GameState.Credits:
                    break;

                case GameState.Playing:
                    bool hit = false;

                    foreach (Bullet b in Bullet.All.ToArray())
                        b.Update();

                    foreach (Player p in players)
                        p.Update(ref hit);

                    if (hit)
                    {
                        Bullet.All.Clear();
                        int i = 0;
                        foreach (Player p in players.ToList())
                        {
                            p.state = Player.State.dabing;
                            

                            if (p.lives == 0)
                            {
                                currentState = GameState.GameOver;
                                players[1 - i].state = Player.State.dabing;
                                players.Remove(p);
                                break;
                            }
                            p.Respawn();
                            i++;
                        }
                    }
                    
                    break;

                case GameState.Paused:
                    break;

                case GameState.GameOver:
                    break;

                default:
                    break;
            }

            oldGamePadState = newGamePadState;
            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            switch (currentState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(menuTexture, new Vector2(0, 0), Color.White);
                    for (int i = 0; i < btnArray.Length; i++)
                    {
                        btnArray[i].Draw(Color.White);
                    }

                    btnArray[currentButton].Draw(new Color(255, 255, 255, 100));
                    break;
                case GameState.LevelSelect:

                    break;

                case GameState.Credits:
                    spriteBatch.Draw(credits, new Vector2(0, 0), Color.White);
                    break;

                case GameState.Playing:
                    spriteBatch.Draw(playerTexture, new Vector2(0, 0), Color.White);
                    map.Draw();
                    foreach (Player p in players)
                    {
                        p.Draw();
                    }

                    foreach (Bullet b in Bullet.All)
                    {
                        b.Draw(bulletTexture);
                    }

                    Particle.DrawAll();
                    break;

                case GameState.Paused:
                    break;

                case GameState.GameOver:
                    map.Draw();
                    foreach (Player p in players)
                    {
                        p.Draw();
                    }
                    break;

                default:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
