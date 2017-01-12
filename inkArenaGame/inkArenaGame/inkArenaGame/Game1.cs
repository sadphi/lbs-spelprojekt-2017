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

        GamePadState newState;
        GamePadState oldState;

        Button[] btnArray;

        Map map;

        int currentButton;
        GameState gameState;

        enum GameState
        {
            MainMenu,
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

            this.Window.Title = "A game by Inkognito";
            this.IsMouseVisible = false;

            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1056;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferMultiSampling = true;
            this.graphics.ApplyChanges();

            gameState = GameState.MainMenu;

            players = new List<Player>();

            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    players.Add(new Player((PlayerIndex)i, 100 + i * 1720));
                }
            }

            map = new Map();

            currentButton = 0;

            btnArray = new Button[3];
            btnArray[0] = new Button(960, 350, Content.Load<Texture2D>("Graphics/Buttons/Playbutton"), () => { gameState = GameState.Playing; });
            btnArray[1] = new Button(960, 550, Content.Load<Texture2D>("Graphics/Buttons/Creditsbutton"), () => { gameState = GameState.Credits; });
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

            newState = GamePad.GetState(PlayerIndex.One);

            foreach (Player p in players)
                p.Update();

            foreach (Bullet b in Bullet.All.ToArray())
                b.Update();

            switch (gameState)
            {
                case GameState.MainMenu:
                    if (newState.DPad.Down == ButtonState.Pressed && oldState.DPad.Down == ButtonState.Released)
                    {
                        currentButton++;
                    }

                    if (newState.DPad.Up == ButtonState.Pressed && oldState.DPad.Up == ButtonState.Released)
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

                    if (newState.Buttons.A == ButtonState.Pressed && oldState.Buttons.A == ButtonState.Released)
                    {
                        btnArray[currentButton].act();
                    }
                    break;
                case GameState.Credits:
                    break;
                case GameState.Playing:
                    break;
                case GameState.Paused:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }

            oldState = newState;

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
            switch (gameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(menuTexture, new Vector2(0, 0), Color.White);
                    for (int i = 0; i < btnArray.Length; i++)
                    {
                        btnArray[i].Draw(Color.White);
                    }
                    
                    btnArray[currentButton].Draw(new Color(255, 255, 255, 100));


                    break;
                case GameState.Credits:
                    break;
                case GameState.Playing:
                    spriteBatch.Draw(playerTexture, new Vector2(0, 0), Color.White);
                    map.Draw();
                    foreach (Player p in players)
                    {
                        p.Draw(playerTexture);
                    }

                    foreach (Bullet b in bullets)
                    {
                        b.Draw(bulletTexture);
                    }
                    break;
                case GameState.Paused:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
