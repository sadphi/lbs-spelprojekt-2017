using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace inkArenaGame
{
    class Player
    {
        Vector2 position;
        Vector2 velocity;

        PlayerIndex index;
        GamePadState newstate;
        GamePadState oldstate;

        private float acc = 1.0f;
        private float deAcc = 0.8f;
        private const float GRAVITY = 0.5f;

        private bool grounded = false;

        private const int WIDTH = 32;
        private const int HEIGHT = 64;

        private float angle;

        Texture2D standing;
        Texture2D runing;
        Texture2D jumping;
        Texture2D arm;

        private float frameClock;
        private int currentFrame;
        private bool flipFrame;

        State state;

        public int lives = 3;

        enum State
        {
            standing = 0,
            walking = 1,
            jumping = 2
        }

        public Player(PlayerIndex playerIndex, int nx)
        {
            index = playerIndex;
            position = new Vector2(nx, 200);
            velocity = new Vector2(0, 0);

            LoadContent();
        }

        private void LoadContent()
        {
            //if (index == 0)
            {
                standing = Game1.contentLoader.Load<Texture2D>("Graphics/Players/Player1Standing");
                runing = Game1.contentLoader.Load<Texture2D>("Graphics/Players/Player1Run");
                jumping = Game1.contentLoader.Load<Texture2D>("Graphics/Players/Player1Jump");

                arm = Game1.contentLoader.Load<Texture2D>("Graphics/Players/PlayerArm");
            }
        }

        public void Update()
        {
            newstate = GamePad.GetState(index, GamePadDeadZone.Circular);

            #region Movement And Collision

            int dirx = DirX();
            velocity.X += newstate.ThumbSticks.Left.X;
            if (grounded) velocity.X *= 0.9f;
            velocity.Y += GRAVITY;

            velocity.X = MathHelper.Clamp(velocity.X, -10, 10);

            if (ButtonHit(Buttons.LeftShoulder) && grounded)
            {
                grounded = false;
                velocity.Y = -12F;
            }

            float futureY = position.Y;

            int dir = Math.Sign(velocity.Y);
            for (int i = 0; i < Math.Floor(Math.Abs(velocity.Y)); i++)
            {
                grounded = false;
                futureY += dir;
                if (IntersectsWithMap(position.X, futureY))
                {
                    futureY -= dir;
                    velocity.Y = 0;
                    if (dir == 1) grounded = true;
                    break;
                }
            }

            float futureX = position.X;

            dir = Math.Sign(velocity.X);
            for (int i = 0; i < Math.Floor(Math.Abs(velocity.X)); i++)
            {
                futureX += dir;
                if (IntersectsWithMap(futureX, futureY))
                {
                    futureX -= dir;
                    velocity.X = 0;
                    break;
                }
            }

            position.X = futureX;
            position.Y = futureY;

            #endregion

            #region State Check

            if (Math.Floor(Math.Abs(velocity.X)) == 0)
            {
                state = State.standing;
                currentFrame = 0;
                frameClock = 0;
            }
            else
            {
                frameClock += Math.Abs(velocity.X) * 0.02f;
                currentFrame = (int)Math.Floor(frameClock);

                state = State.walking;
            }

            if (!grounded)
            {
                state = State.jumping;
                currentFrame = 0;
                frameClock = 0;
            }

            #endregion

            #region Arm Angle
            Vector2 rightThumb = newstate.ThumbSticks.Right;

            if (rightThumb.Length() > 0.5)
            {
                rightThumb.Normalize();

                if (rightThumb.X > 0) flipFrame = false;
                if (rightThumb.X < 0) flipFrame = true;

                angle = (float)Math.Atan2(-rightThumb.Y, (rightThumb.X));
            }
            else
            {
                rightThumb.X = (float)Math.Cos(angle);
                rightThumb.Y = 0;
                angle = (float)Math.Atan2(0, (rightThumb.X));
            }

            #endregion

            #region Shooting

            if (ButtonHit(Buttons.X))
            {
                velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 32;
            }

            if (ButtonHit(Buttons.RightShoulder))
                Bullet.Spawn(index, new Vector2(position.X + 7 + (flipFrame ? 18 : 0), position.Y + 15) + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * arm.Width, new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10);

            #endregion

            foreach (Bullet b in Bullet.All)
            {
                if (index != b.index)
                {
                    if (Collision(position.X, position.Y, b.position.X, b.position.Y, WIDTH, HEIGHT, 1, 1))
                    {
                        lives -= 1;
                    }
                }
            }

            oldstate = newstate;
        }

        private int DirX()
        {
            if (newstate.DPad.Right == ButtonState.Pressed) return 1;
            if (newstate.DPad.Left == ButtonState.Pressed) return -1;
            return 0;
        }

        private int DirY()
        {
            if (newstate.DPad.Down == ButtonState.Pressed) return 1;
            if (newstate.DPad.Up == ButtonState.Pressed) return -1;
            return 0;
        }

        private bool ButtonHit(Buttons button)
        {
            return newstate.IsButtonDown(button) && oldstate.IsButtonUp(button);
        }

        private bool IntersectsWithMap(float px, float py)
        {
            int bx;
            int by;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    bx = (int)Math.Floor(px / 32.0f + x);
                    by = (int)Math.Floor(py / 32.0f + y);

                    if (Map.currentLevel[bx, by] == 1)
                    {
                        if (Collision(px, py, bx * 32, by * 32, WIDTH, HEIGHT, 32, 32))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool Collision(float x1, float y1, float x2, float y2,
                       float w1, float h1, float w2, float h2)
        {
            if (x1 + w1 > x2 && x1 < x2 + w2 &&
                y1 + h1 > y2 && y1 < y2 + h2)
            {
                return true;
            }

            return false;
        }

        public void Draw()
        {
            #region DebugDraw
            if (false)
            {
                int bx;
                int by;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        bx = (int)Math.Floor(position.X / 32.0f + x);
                        by = (int)Math.Floor(position.Y / 32.0f + y);

                        if (Map.currentLevel[bx, by] == 1)
                        {
                            if (Collision(position.X, position.Y, bx * 32, by * 32, WIDTH, HEIGHT, 32, 32))
                            {
                                Game1.spriteBatch.Draw(Game1.pixel, new Rectangle((int)Math.Floor(position.X / 32.0f + x) * 32, (int)Math.Floor(position.Y / 32.0f + y) * 32, 32, 32), Color.Red);
                            }
                            else
                            {
                                Game1.spriteBatch.Draw(Game1.pixel, new Rectangle((int)Math.Floor(position.X / 32.0f + x) * 32, (int)Math.Floor(position.Y / 32.0f + y) * 32, 32, 32), Color.Yellow);
                            }
                        }
                        else
                        {
                            Game1.spriteBatch.Draw(Game1.pixel, new Rectangle((int)Math.Floor(position.X / 32.0f + x) * 32, (int)Math.Floor(position.Y / 32.0f + y) * 32, 32, 32), Color.Green);

                        }
                    }
                }
            }
            #endregion

            Game1.spriteBatch.Draw(state == 0 ? standing : (state == (State)1 ? runing : jumping), new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT), new Rectangle(32 * (currentFrame % 6), 0, 32, 64), Color.White, 0, Vector2.Zero, flipFrame ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            int armXOffset = 7 * (WIDTH / 32) + (flipFrame ? 18 * (WIDTH / 32) : 0);

            Game1.spriteBatch.Draw(arm, new Vector2(position.X + armXOffset, position.Y + 18 * (WIDTH / 32)), null, Color.White, angle, new Vector2(3, 5), 1, flipFrame ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);

        }
    }
}
