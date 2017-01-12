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
        List<Bullet> bullets;

        Texture2D bulletTexture;
        Texture2D playerTexture;

        Button[] btn = new Button[3];

        Map map;
        Button button;
        enum GameState
        {
            StartMenu,
            Playing,
            Paused
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
            this.IsMouseVisible = true;

            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1056;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferMultiSampling = true;
            this.graphics.ApplyChanges();

            players = new List<Player>();
            bullets = new List<Bullet>();

            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    players.Add(new Player((PlayerIndex)i));
                }
            }

            map = new Map();

            
            btn[0] = new Button(960, 250, Content.Load<Texture2D>("Graphics/PortalProjectile1"));
            btn[1] = new Button(960, 450, Content.Load<Texture2D>("Graphics/PortalProjectile1"));
            btn[2] = new Button(960, 650, Content.Load<Texture2D>("Graphics/PortalProjectile1"));

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
            bulletTexture = Content.Load<Texture2D>("Graphics/PortalProjectile1");
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

            foreach (Player p in players)
                p.Update();

            foreach (Bullet b in bullets)
                b.Update();

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

            btn[0].Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
