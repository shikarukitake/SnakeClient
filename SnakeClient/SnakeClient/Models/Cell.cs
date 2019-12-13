using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeClient
{
    public class Cell
    {
        public string Color { get; set; }
        public Cell(string Color)
        {
            this.Color = Color;
        }
    }
}
