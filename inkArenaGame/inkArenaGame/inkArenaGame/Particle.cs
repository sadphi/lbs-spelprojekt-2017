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
        float angle;

        static Texture2D texture = Game1.contentLoader.Load<Texture2D>("Graphics/GunImpact");

        public Particle(Vector2 newPos, float newAng)
        {
            position = newPos;
            frameTime = 0;
            angle = 0;
            All.Add(this);
        }

        public static void DrawAll()
        {
            foreach (Particle p in All.ToArray())
            {
                p.Draw();
            }
        }

        public void Draw()
        {
            Game1.spriteBatch.Draw(texture, position, new Rectangle(64 * (int)Math.Floor(frameTime), 0, 64, 64), Color.HotPink, angle, new Vector2(32, 32), 1f, SpriteEffects.None, 0);
            frameTime += 0.1f;
            if (frameTime >= 4)
                All.Remove(this);
        }

        public static void Spawn(Vector2 newPos, float newAng)
        {
            All.Add(new Particle(newPos, newAng));
        }
    }
}
