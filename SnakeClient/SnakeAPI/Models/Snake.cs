using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnakeAPI.Models
{
    public interface ISnake
    {
        bool Move(int Width, int Height);
    }

    public class Snake : ISnake
    {
        public List<Cords> PartsOfSnake { get; set; }
        public bool IsAlive { get; set; }
        public string Direction { get; set; }
        public Cords Food { get; set; }

        public Snake()
        {
            IsAlive = true;
            Direction = "Up";
            PartsOfSnake = new List<Cords>();
        }

        //comparing too cords. True if similar or false if not
        public bool CordsCmp(Cords First, Cords Second)
        {
            if (First.Y == Second.Y && First.X == Second.X)
                return (true);
            else
                return (false);
        }

        //Move snake!
        public bool Move(int Width, int Height)
        {
            if (Food == null)
                GenerateFood(Height, Width);

            int X = PartsOfSnake[0].X;
            int Y = PartsOfSnake[0].Y;

            //Creating new head's coordinates according to direction
            switch (Direction)
            {
                case "Up":
                    Y--;
                    if (Y < 1)
                        return false;
                    break;
                case "Down":
                    Y++;
                    if (Y > Height)
                        return false;
                    break;
                case "Right":
                    X++;
                    if (X > Width)
                        return false;
                    break;
                case "Left":
                    X--;
                    if (X < 1)
                        return false;
                    break;
            }

            //Check eated food or non. If eated we don't delete snake's tail
            if (FoodIsEated())
                PartsOfSnake.RemoveAt(PartsOfSnake.Count - 1);
            else
                GenerateFood(Height, Width);

            //Pushing new head into top of PartsOfSnake
            PartsOfSnake.Insert(0, new Cords(X, Y));

            if (!CheckCollision(PartsOfSnake[0]))
                return (false);

            return (true);
        }

        //Return false if head move to yourself
        public bool CheckCollision(Cords Head)
        {
            for (int i = 1; i < PartsOfSnake.Count; ++i)
            {
                if (CordsCmp(Head, PartsOfSnake[i]))
                    return (false);
            }
            return (true);
        }

        //Check collision between Food and Parts of snake
        public bool FoodIsEated()
        {
            foreach (var cord in PartsOfSnake)
            {
                if (CordsCmp(Food, cord))
                    return (false);
            }
            return (true);
        }

        //Random generation of food on the map
        public void GenerateFood(int Height, int Width)
        {
            Random R = new Random();
            Food = new Cords(R.Next(1, Width), R.Next(1, Height));
            //Generating random cords for food while its don't collision with some part of snake
            while (!FoodIsEated())
            {
                Food.X = R.Next(1, Width);
                Food.Y = R.Next(1, Height);
            }
        }
    }
}
