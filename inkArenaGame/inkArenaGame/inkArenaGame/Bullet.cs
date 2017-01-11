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
    class Bullet
    {
        Vector2 position;
        Vector2 velocity;

        public Bullet(Vector2 newPos, Vector2 newVel)
        {
            position = newPos;
            velocity = newVel;
        }

        public void Update()
        {
            position += velocity;
        }

        public void Draw(Texture2D bullet)
        {
            Game1.spriteBatch.Draw(bullet, position, null, Color.White, ((float)Math.Atan2(velocity.Y, velocity.X)), Vector2.One / 2.0f, Vector2.One, SpriteEffects.None, 0);
        }
    }
}
