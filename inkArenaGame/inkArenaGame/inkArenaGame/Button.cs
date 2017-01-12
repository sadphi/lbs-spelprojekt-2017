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
        int buttonX, buttonY;
        public Action act;
        Texture2D texture;

        public Button(int x, int y, Texture2D text)
        {
            buttonX = x;
            buttonY = y;
            texture = text;
        }

        public void Draw()
        {
            Game1.spriteBatch.Draw(texture, new Vector2(buttonX, buttonY), Color.White);
        }
    }
}
