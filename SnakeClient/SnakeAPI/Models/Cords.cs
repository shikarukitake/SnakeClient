using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnakeAPI.Models
{
    public class Cords
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Cords(int x, int y) { X = x; Y = y; }
    }
}
