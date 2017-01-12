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
    class Particle
    {
        static List<Particle> All = new List<Particle>();

        Vector2 position;
        float frameTime;
        int angle;

        static Texture2D texture = Game1.contentLoader.Load<Texture2D>("Graphics/GunImpact1");

        public Particle(Vector2 newPos)
        {
            position = newPos;
            frameTime = 0;
            All.Add(this);
        }

        public static void DrawAll()
        {
            foreach (Particle p in Particle.All.ToList())
            {
                p.Draw();
            }
        }

        public void Draw()
        {
            Game1.spriteBatch.Draw(texture, position, new Rectangle(32 * (int)Math.Floor(frameTime), 0, 32, 32), Color.White, angle, new Vector2(0, 16), 1, SpriteEffects.None, 0);
            frameTime += 0.1f;
            if (frameTime >= 4)
                All.Remove(this);
        }

        public static void Spawn(Vector2 newPos)
        {
            All.Add(new Particle(newPos));
        }
    }
}
