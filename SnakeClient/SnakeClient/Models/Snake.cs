using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnakeClient.Models
{
    public class Snake
    {
        public List<Cords> PartsOfSnake { get; set; }
        public bool IsAlive { get; set; }
        public string Direction { get; set; }
        public Cords Food { get; set; }

        public bool CheckCollision(Cords cord)
        {
            for (int i = 0; i < PartsOfSnake.Count; ++i)
            {
                if (CordsCmp(cord, PartsOfSnake[i]))
                    return (false);
            }
            return (true);
        }

        public bool CordsCmp(Cords First, Cords Second)
        {
            if (First.Y == Second.Y && First.X == Second.X)
                return (true);
            else
                return (false);
        }
    }
}
