using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inkArenaGame
{
    class Map
    {
        List<int[,]> levels;
        List<Texture2D> levelTextures;
        public static int[,] currentLevel;
        Texture2D currentTexture;

        public Map()
        {
            levels = new List<int[,]>();
            levelTextures = new List<Texture2D>();
            for (int i = 0; i < 1; i++)
            {
                levels.Add(LoadLevel("Levels/level" + (i + 1) + ".txt", 60, 33));
                levelTextures.Add(Game1.contentLoader.Load<Texture2D>("map" + (i + 1)));
            }

            ChangeLevel(1);
        }

        public void ChangeLevel(int level)
        {
            currentLevel = levels[level - 1];
            currentTexture = levelTextures[level - 1];
        }

        public void Draw()
        {
            for (int y = 0; y < 33; y++)
            {
                for (int x = 0; x < 60; x++)
                {
                    if (currentLevel[x,y] == 1) Game1.spriteBatch.Draw(Game1.pixel, new Rectangle(x * 32, y * 32, 32, 32), Color.White);
                }
            }
            //Game1.spriteBatch.Draw(currentTexture, Vector2.Zero, Color.White);
        }

        public int[,] LoadLevel(string fileName, int width, int height)
        {
            int counter = 0;
            int[,] myArray = new int[width, height];
            string line;
            fileName = System.IO.File.Exists(fileName) ? fileName : "C:\\Users\\kristoffer.franzon\\Desktop\\level1.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            
            while ((line = file.ReadLine()) != null)
            {

                line = line.Replace(",", "");
                for (int i = 0; i < line.Length; i++)
                {

                    myArray[i, counter] = Convert.ToInt32(line[i].ToString());
                }


                counter++;
            }

            file.Close();
            return myArray;
        }
    }
}
