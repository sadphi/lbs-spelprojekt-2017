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

        public Player(PlayerIndex playerIndex)
        {
            index = playerIndex;
            position = new Vector2(100, 100);
            velocity = new Vector2(0, 0);
        }

        public void Update()
        {
            newstate = GamePad.GetState(index);

            int dir = DirX();

            velocity.Y += GRAVITY;

            if (dir != 0)
            {
                velocity.X += dir * acc;
                velocity.X = MathHelper.Clamp(velocity.X, -4, 4);
            }
            else
            {
                velocity.X *= deAcc;
            }

            if (ButtonHit(Buttons.A)) velocity.Y = -12;

            position += velocity;

            if (position.Y > 1056 - 64) position.Y = 1056 - 64;

            oldstate = newstate;
        }

        private int DirX()
        {
            if (newstate.DPad.Right == ButtonState.Pressed) return 1;
            if (newstate.DPad.Left == ButtonState.Pressed) return -1;
            return 0;
        }

        private bool ButtonHit(Buttons button)
        {
            return newstate.IsButtonDown(button) && oldstate.IsButtonUp(button);
        }

        public void Draw(Texture2D texture)
        {
            Game1.spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
