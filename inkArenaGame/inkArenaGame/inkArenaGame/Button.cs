using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inkArenaGame
{
    class Button
    {
        public int buttonX, buttonY;
        public Action act;
        public Texture2D texture;

        public Button(int x, int y, Texture2D text, Action newAct)
        {
            buttonX = x;
            buttonY = y;
            texture = text;
            act = newAct;
        }

        public void Draw(Color color)
        {
            Game1.spriteBatch.Draw(texture, new Vector2(buttonX - 64, buttonY), color);
        }
    }
}
