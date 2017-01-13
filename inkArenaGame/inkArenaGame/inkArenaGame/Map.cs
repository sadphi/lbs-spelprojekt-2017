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
        static List<int[,]> levels;
        public static List<Texture2D> levelTextures;
        private static int[,] currentLevel;
        
        static Texture2D currentTexture;

        public Map()
        {
            levels = new List<int[,]>();
            levelTextures = new List<Texture2D>();
            for (int i = 0; i < 2; i++)
            {
                levels.Add(LoadLevel("Leveldatas/level" + (i + 1) + ".txt", 60, 33));
                levelTextures.Add(Game1.contentLoader.Load<Texture2D>("Graphics/Levels/Map" + (i + 1) + "Background"));
            }

            ChangeLevel(0);
        }

        public static void ChangeLevel(int level)
        {
            currentLevel = levels[level];
            currentTexture = levelTextures[level];
        }

        public static int CurrentIndex(int x, int y)
        {
            x = (int)MathHelper.Clamp(x, 0, 59);
            y = (int)MathHelper.Clamp(y, 0, 32);

            return currentLevel[x, y];
        }

        public void Draw()
        {
            //for (int y = 0; y < 33; y++)
            //{
            //    for (int x = 0; x < 60; x++)
            //    {
            //        if (currentLevel[x,y] == 1) Game1.spriteBatch.Draw(Game1.pixel, new Rectangle(x * 32, y * 32, 32, 32), Color.White);
            //    }
            //}
            Game1.spriteBatch.Draw(currentTexture, Vector2.Zero, Color.White);
        }

        public int[,] LoadLevel(string fileName, int width, int height)
        {
            int counter = 0;
            int[,] myArray = new int[width, height];
            string line;
           // fileName = System.IO.File.Exists(fileName) ? fileName : "C:/sers/kristoffer.franzon/Documents/GitHub/lbs-spelprojekt-2017/inkArenaGame/inkArenaGame/inkArenaGameContent/Levels/level1.txt";//"C:\\Users\\philip.sadrian\\Desktop\\level1.txt";
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
