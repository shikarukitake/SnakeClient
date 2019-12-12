using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SnakeClient.Models
{
    public class GameBoard
    {
        public int TurnNumber { get; set; }
        public int TimeUntilNextTurnMS { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActive { get; set; }//map created or not
        public bool WantPlaying { get; set; }
        private DateTime GameTime { get; set; }
        public Snake _Snake { get; set; }

        public GameBoard(int timeUntilNextTurnMS, int width, int height)
        {
            TimeUntilNextTurnMS = timeUntilNextTurnMS;
            Width = width;
            Height = height;
        }
    }
}