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

        Random rnd;

        public static SpriteFont font;

        List<Player> players;

        Texture2D bulletTexture;
        Texture2D playerTexture;
        Texture2D menuTexture;
        Texture2D p1BigArm;
        Texture2D p2BigArm;
        Texture2D creditsTexture;

        GamePadState newGamePadState;
        GamePadState oldGamePadState;

        KeyboardState newKeyboardState;
        KeyboardState oldKeyboardState;

        Button[] btnMainArray;
        Button[] btnPauseArray;

        Map map;

        int currentButton;
        int currentLevelHighlighted;

        GameState currentState;
        GameState oldState;

        int numOfPlayers;

        SpriteFont countDownFont;
        Vector2 countDownPos;
        Vector2 countDownOrigin;
        Vector2 countDownSize;
        string countDownText;
        float countDownTimer;
        float countDownScale;
        float countDownAngle;

        enum GameState
        {
            MainMenu,
            LevelSelect,
            Credits,
            Playing,
            Paused,
            GameOver
        }

        Song dabSong;
        Song levelSelect;
        Song mainMenu;
        Song credits;
        Song stonelevel;
        Song lavalevel;

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

            this.Window.Title = "Hillbilly Havoc";
            this.IsMouseVisible = false;

            font = Game1.contentLoader.Load<SpriteFont>("Fonts/Font");
            countDownFont = Game1.contentLoader.Load<SpriteFont>("Fonts/CountDownFont");

            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1056;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferMultiSampling = true;
            this.graphics.ApplyChanges();

            countDownPos = new Vector2(1920 / 2, 1056 / 2);

            currentState = GameState.MainMenu;
            rnd = new Random();

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

            btnMainArray = new Button[3];
            btnMainArray[0] = new Button(960, 350, Content.Load<Texture2D>("Graphics/Buttons/Playbutton"), () => { currentState = GameState.LevelSelect; MediaPlayer.Stop(); });
            btnMainArray[1] = new Button(960, 550, Content.Load<Texture2D>("Graphics/Buttons/Creditsbutton"), () => { currentState = GameState.Credits; MediaPlayer.Stop(); });
            btnMainArray[2] = new Button(960, 750, Content.Load<Texture2D>("Graphics/Buttons/Quitbutton"), () => { Environment.Exit(0); });

            btnPauseArray = new Button[3];
            btnPauseArray[0] = new Button(960, 350, Content.Load<Texture2D>("Graphics/Buttons/Resumebutton"), () => { currentState = GameState.Playing; MediaPlayer.Resume(); });
            btnPauseArray[1] = new Button(960, 550, Content.Load<Texture2D>("Graphics/Buttons/levelbutton"), () => { currentState = GameState.LevelSelect; MediaPlayer.Stop(); });
            btnPauseArray[2] = new Button(960, 750, Content.Load<Texture2D>("Graphics/Buttons/Quitbutton"), () => { currentState = GameState.MainMenu; currentButton = 0; MediaPlayer.Stop(); });
            
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
            menuTexture = Content.Load<Texture2D>("Graphics/Menu3NoArms");
            p1BigArm = Content.Load<Texture2D>("Graphics/MenuArm1");
            p2BigArm = Content.Load<Texture2D>("Graphics/MenuArm2");
            bulletTexture = Content.Load<Texture2D>("Graphics/GunProjectile1");
            creditsTexture = Content.Load<Texture2D>("Graphics/credits");

            dabSong = Content.Load<Song>("Sounds/dabSong");
            levelSelect = Content.Load<Song>("Sounds/levelselect");
            mainMenu = Content.Load<Song>("Sounds/mainmenu");
            credits = Content.Load<Song>("Sounds/credits");
            stonelevel = Content.Load<Song>("Sounds/stonelevel");
            lavalevel = Content.Load<Song>("Sounds/lavalevel");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Stop();            
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
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            newGamePadState = GamePad.GetState(PlayerIndex.One);
            newKeyboardState = Keyboard.GetState();

            switch (currentState)
            {
                case GameState.MainMenu:
                    if (MediaPlayer.State == MediaState.Stopped)
                    {
                        MediaPlayer.Play(mainMenu);
                    }

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

                    if (currentButton >= btnMainArray.Length)
                    {
                        currentButton = 0;
                    }

                    if (currentButton <= -1)
                    {
                        currentButton = btnMainArray.Length - 1;
                    }

                    if ((newGamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter)))
                    {
                        oldState = currentState;
                        btnMainArray[currentButton].act();
                    }
                    break;

                case GameState.LevelSelect:
                    if (MediaPlayer.State == MediaState.Stopped)
                    {
                        MediaPlayer.Play(levelSelect);
                    }

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

                    if (currentLevelHighlighted > Map.levelTextures.Count() - 1)
                    {
                        currentLevelHighlighted = 0;
                    }

                    if (currentLevelHighlighted < 0)
                    {
                        currentLevelHighlighted = Map.levelTextures.Count() - 1;
                    }

                    if ((newGamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter)))
                    {
                        MediaPlayer.Stop();
                        if (currentLevelHighlighted == 0)
                        {
                            MediaPlayer.Play(stonelevel);
                        }
                        else
                        {
                            MediaPlayer.Play(lavalevel);
                        }
                        Map.ChangeLevel(currentLevelHighlighted);
                        countDownTimer = 3;
                        countDownScale = 1;
                        countDownAngle = 0;
                        countDownText = countDownTimer + "!";
                        countDownSize = countDownFont.MeasureString(countDownText);
                        countDownOrigin = countDownSize / 2;
                        oldState = currentState;
                        currentState = GameState.Playing;
                        players.Clear();
                        //players.Add(new Player((PlayerIndex)2, 980));
                        for (int i = 0; i < numOfPlayers; i++)
                        {
                            players.Add(new Player((PlayerIndex)i, 100 + i * 1720));
                        }
                    }

                    if ((newGamePadState.Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape)))
                    {
                        currentState = oldState;
                        MediaPlayer.Stop();
                    }
                    break;

                case GameState.Credits:
                    if (MediaPlayer.State == MediaState.Stopped)
                    {
                        MediaPlayer.Play(credits);
                    }

                    if ((newGamePadState.Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape)))
                    {
                        MediaPlayer.Stop();
                        currentState = oldState;
                    }
                    break;

                case GameState.Playing:
                    if (countDownTimer >= -0.8)
                    {
                        countDownTimer -= 0.02f;
                        countDownText = (int)Math.Ceiling(countDownTimer) + "!";
                        if (countDownTimer < 0) countDownText = "FIGHT";
                        countDownSize = countDownFont.MeasureString(countDownText);
                        countDownOrigin = countDownSize / 2;

                        countDownScale = 1.25f + (float)Math.Sin(countDownTimer * 6) * 0.5f;
                        countDownAngle = (float)Math.Sin(countDownTimer * 3f) * 0.3f;
                    }

                    bool hit = false;

                    foreach (Bullet b in Bullet.All.ToArray())
                        b.Update();

                    if (countDownTimer <= 0)
                        foreach (Player p in players)
                            p.Update(ref hit);

                    if (hit)
                    {
                        Bullet.All.Clear();
                        int i = 0;
                        foreach (Player p in players.ToList())
                        {
                            if (p.lives == 0)
                            {
                                currentState = GameState.GameOver;
                                players.Remove(p);
                                players[0].position = new Vector2(1920 / 2 - 16, 1056 / 2 - 15);
                                MediaPlayer.Play(dabSong);
                                MediaPlayer.Pause();
                                break;
                            }
                            p.Respawn();
                            i++;
                        }

                        countDownTimer = 3;
                        countDownScale = 1;
                        countDownAngle = 0;
                        countDownText = "3!";
                        countDownSize = countDownFont.MeasureString(countDownText);
                        countDownOrigin = countDownSize / 2;
                    }

                    if ((newGamePadState.Buttons.Start == ButtonState.Pressed && oldGamePadState.Buttons.Start == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape)))
                    {
                        MediaPlayer.Pause();
                        currentState = GameState.Paused;
                    }
                    break;

                case GameState.Paused:
                    if ((newGamePadState.Buttons.Start == ButtonState.Pressed && oldGamePadState.Buttons.Start == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape)))
                    {
                        MediaPlayer.Resume();
                        currentState = GameState.Playing;
                    }

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

                    if (currentButton >= btnPauseArray.Length)
                    {
                        currentButton = 0;
                    }

                    if (currentButton <= -1)
                    {
                        currentButton = btnPauseArray.Length - 1;
                    }

                    if ((newGamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter)))
                    {
                        oldState = GameState.MainMenu;
                        btnPauseArray[currentButton].act();
                    }

                    break;
                case GameState.GameOver:

                    players[0].DabState();

                    if (players[0].state == Player.State.dabing)
                    {
                        if (MediaPlayer.State != MediaState.Playing)
                            MediaPlayer.Resume();
                    }
                    else
                    {
                        MediaPlayer.Pause();
                    }

                    if ((newGamePadState.Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B == ButtonState.Released) ||
                        (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape)))
                    {
                        currentState = GameState.MainMenu;

                        MediaPlayer.Stop();
                    }


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
                    for (int i = 0; i < btnMainArray.Length; i++)
                    {
                        btnMainArray[i].Draw(Color.White);
                    }

                    btnMainArray[currentButton].Draw(new Color(255, 255, 255, 100));

                    Vector2 arm1Pos = new Vector2(140, 508);
                    Vector2 arm1Origin = new Vector2(60, 76);
                    Vector2 buttonPos = new Vector2(btnMainArray[currentButton].buttonX + btnMainArray[currentButton].texture.Width / 2, btnMainArray[currentButton].buttonY + btnMainArray[currentButton].texture.Height / 2);
                    float angle1 = (float)Math.Atan2(buttonPos.Y - arm1Pos.Y, buttonPos.X - arm1Pos.X);
                    float angle2 = (float)Math.Atan2(buttonPos.Y - arm1Pos.Y, -buttonPos.X - arm1Pos.X);
                    spriteBatch.Draw(p1BigArm, arm1Pos, null, Color.White, angle1, arm1Origin, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(p2BigArm, new Vector2(1920 - arm1Pos.X, arm1Pos.Y), null, Color.White, -angle1, new Vector2(p1BigArm.Width - arm1Origin.X, arm1Origin.Y), 1, SpriteEffects.FlipHorizontally, 0);
                    break;
                case GameState.LevelSelect:
                    spriteBatch.Draw(Map.levelTextures[currentLevelHighlighted], new Vector2(1920 / 2, 1056 / 2), null, Color.White, 0, new Vector2(Map.levelTextures[currentLevelHighlighted].Width / 2, Map.levelTextures[currentLevelHighlighted].Height / 2), 0.8f, SpriteEffects.None, 0);
                    break;

                case GameState.Credits:
                    spriteBatch.Draw(creditsTexture, new Vector2(0, 0), Color.White);
                    break;

                case GameState.Playing:
                    spriteBatch.Draw(playerTexture, new Vector2(0, 0), Color.White);
                    map.Draw();
                    
                    if (countDownTimer > -0.8f)
                    {
                        spriteBatch.DrawString(countDownFont, countDownText, countDownPos, Color.White, countDownAngle, countDownOrigin, countDownScale, SpriteEffects.None, 0);
                    }


                    foreach (Player p in players)
                    {
                        p.Draw();
                    }

                    Particle.DrawAll();

                    foreach (Bullet b in Bullet.All)
                    {
                        b.Draw(bulletTexture);
                    }

                    break;
                case GameState.Paused:
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

                    spriteBatch.Draw(pixel, new Rectangle(0, 0, 1920, 1056), new Color(0f, 0f, 0f, 0.5f));
                    for (int i = 0; i < btnPauseArray.Length; i++)
                    {
                        btnPauseArray[i].Draw(Color.White);
                    }

                    btnPauseArray[currentButton].Draw(new Color(255, 255, 255, 100));
                    break;

                case GameState.GameOver:
                    spriteBatch.End();

                    int isDab = (players[0].state == Player.State.dabing ? 1 : 0);

                    float zoom = 4 + (float)(rnd.NextDouble() - 0.5) * isDab;
                    Vector2 offSet = new Vector2((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1) * 3 * isDab;

                    Matrix _transform = Matrix.CreateTranslation(new Vector3(-players[0].position.X - 16 + offSet.X, -players[0].position.Y + offSet.Y, 0)) *
                                                    Matrix.CreateRotationZ((float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds) * 0.05f * isDab) *
                                                    Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                                                    Matrix.CreateTranslation(new Vector3(1920 * 0.5f, 1056 * 0.5f, 0));


                    spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null,
                        null,
                        null, _transform);

                    map.Draw();

                    foreach (Player p in players)
                    {
                        p.Draw();
                    }

                    string text = "Player " + ((int)players[0].index + 1) + " won!";
                    string dabText = "Hold Y to DAB";
                    spriteBatch.DrawString(countDownFont, text, new Vector2(players[0].position.X + 16, players[0].position.Y - 100), Color.White, 0, new Vector2(countDownFont.MeasureString(text).X / 2, 0), 0.2f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(countDownFont, dabText, new Vector2(players[0].position.X + 16, players[0].position.Y - 50), Color.White, 0, new Vector2(countDownFont.MeasureString(dabText).X / 2, 0), 0.1f, SpriteEffects.None, 0);
                    
                    break;
                default:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
