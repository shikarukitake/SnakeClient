using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SnakeAPI.Models
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

        public GameBoard() { }

        public GameBoard(bool isActive) { IsActive = isActive; }

        public GameBoard(int timeUntilNextTurnMS) { TimeUntilNextTurnMS = timeUntilNextTurnMS; }

        public GameBoard(int turnNumber, int timeUntilNextTurnMS)
        {
            TurnNumber = turnNumber;
            TimeUntilNextTurnMS = timeUntilNextTurnMS;
        }

        public GameBoard (int timeUntilNextTurnMS, int width, int height)
        {
            TimeUntilNextTurnMS = timeUntilNextTurnMS;
            Width = width;
            Height = height;
        }
        //Change direction if u can rotate to this direction
        public bool ChangeDirectionOfSnake(string Direction)
        {
            if (_Snake.Direction == "Up" && Direction == "Down")
                return (false);
            if (_Snake.Direction == "Right" && Direction == "Left")
                return (false);
            if (_Snake.Direction == "Left" && Direction == "Right")
                return (false);
            if (_Snake.Direction == "Down" && Direction == "Up")
                return (false);
            _Snake.Direction = Direction;
            return (true);
        }

        //Snake initialization
        public void SnakeInitialization()
        {
            _Snake = new Snake();
            int MidOfHeight = Height % 2 == 0 ? Height / 2 : (Height / 2) + 1;
            int MidOfWidth = (Width / 2) + 1;
            _Snake.PartsOfSnake.Add(new Cords(MidOfWidth, MidOfHeight));
            _Snake.PartsOfSnake.Add(new Cords(MidOfWidth, MidOfHeight + 1));
            _Snake.Food = null;
        }

        //Starting Game 
        public async void StartGame()
        {
            IsActive = true;
            WantPlaying = true;
            
            await Task.Run(() => 
            {
                while (WantPlaying)
                {
                    TurnNumber = 0;
                    GameTime = DateTime.Now;
                    SnakeInitialization();
                    _Snake.GenerateFood(Height, Width);
                    while (_Snake.IsAlive && WantPlaying)
                    {
                        TimeSpan Interval = DateTime.Now.Subtract(GameTime);
                        Int64 IntervalMS = Convert.ToInt64(Interval.TotalMilliseconds);
                        if (IntervalMS >= TimeUntilNextTurnMS)
                        {
                            if (!_Snake.Move(Width, Height))
                                _Snake.IsAlive = false;
                            GameTime = DateTime.Now;
                            TurnNumber++;
                        }

                    }

                    Task.Delay(3000);
                }
            });
        }
    }
}