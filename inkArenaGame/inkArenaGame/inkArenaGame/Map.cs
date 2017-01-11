using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inkArenaGame
{
    class Map
    {
        public int[,] LoadLevel(string fileName, int width, int height)
        {
            int counter = 0;
            int[,] myArray = new int[width, height];
            string line;

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

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(myArray[x, y]);
                }
            }

            file.Close();
            return myArray;
        }
    }
}
