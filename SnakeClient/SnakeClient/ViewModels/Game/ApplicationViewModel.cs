using RestSharp;
using SnakeClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SnakeClient
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Map map;
        public ObservableCollection<Map> Phones { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Turn { get; set; }
        public int TimeTo { get; set; }
        public bool GameIsStarted { get; set; }
        public bool WantPlaying { get; set; }
        DispatcherTimer timer { get; set; }
        public Client APISnakeClient { get; set; }

        private string createMap;
        public string CreateMap { get { return createMap; } set {
                createMap = value;
                OnPropertyChanged("CreateMap");
            } }
        GameBoard Game;

        public ObservableCollection<Cell> Cells
        {
            get => _cells;
            private set { _cells = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Cell> _cells = new ObservableCollection<Cell>();
        
        // команда добавления нового объекта
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      Map _map = obj as Map;
                      if (_map != null)
                      {
                          /*if (GameIsStarted)
                              APISnakeClient.StartGame().ContinueWith(t => { timer.Stop(); });*/
                          if (WantPlaying)
                          {
                              APISnakeClient.StartGame().ContinueWith(t =>
                                 { WantPlaying = !WantPlaying; });
                              //timer.Stop();
                          }
                          Width = _map.Width;
                          Height = _map.Height;
                          TimeTo = _map.TimeUntilNextTurnMS / 10;
                          //timer.Interval = TimeSpan.FromMilliseconds(_map.TimeUntilNextTurnMS);
                          //timer.Tick += timer_Tick;
                          System.Net.HttpStatusCode StatusCode = System.Net.HttpStatusCode.BadRequest;
                          APISnakeClient.CreateMap(_map, GameIsStarted).ContinueWith(t =>
                          {
                              StatusCode = t.Result;
                              if (StatusCode == System.Net.HttpStatusCode.OK)
                              {
                                  GameIsStarted = true;
                                  MessageBox.Show("Map created!");
                                  CreateMap = "RecreateMap";
                              }
                              else
                                  MessageBox.Show("Something wrong...");
                          });
                      }

                  }, c => (Width > 3 && Height > 3) && (Height == Width)));
            }
        }

        private RelayCommand playCommand;
        public RelayCommand PlayCommand
        {
            get
            {
                return playCommand ??
                  (playCommand = new RelayCommand(obj =>
                  {
                      APISnakeClient.StartGame().ContinueWith(t => 
                      {
                          WantPlaying = !WantPlaying;
                          if (WantPlaying)
                              UpdateView();
                            //timer.Start();
                          else
                              WantPlaying = !WantPlaying;

                              //timer.Stop();
                          Turn = -1;

                      });
                  }, c => WantPlaying == false));
            }
        }

        public ICommand KeyPressedCommand { get; set; }
        
        private void KeyPressed(object parameter)
        {
            var key = (string)parameter;
            APISnakeClient.DirectionChange(key).ContinueWith(t => { });
        }

        public Map SelectedMap
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        
        public ApplicationViewModel()
        {
            Phones = new ObservableCollection<Map>();
            SelectedMap = new Map(20, 20, 1000);
            KeyPressedCommand = new RelayCommand((parameter) => KeyPressed(parameter));
            timer = new DispatcherTimer();
            APISnakeClient = new Client("https://localhost:44317");
            CreateMap = "CreateMap";
            Game = null;
        }
        /*
        void timer_Tick(object sender, EventArgs e)
        {
            Snake snake = null;
            GameBoard Game = APISnakeClient.GetBoard();
            if (Game != null)
                snake = Game._Snake;

            if (snake != null)//&& Turn != Game.TurnNumber)
            {
                try
                {
                    //Turn = Game.TurnNumber;
                    Cells = new ObservableCollection<Cell>();
                    Cords cord = new Cords(0, 0);

                    for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                    {
                        for (cord.X = 1; cord.X <= Width; cord.X++)
                        {
                            if (!snake.CheckCollision(cord))
                                Cells.Add(new Cell("Green"));
                            else if (snake.CordsCmp(snake.Food, cord))
                                Cells.Add(new Cell("Red"));
                            else
                                Cells.Add(new Cell("White"));
                        }
                    }
                }
                catch (Exception E) { }
            }
        }*/
            /*
            void timer_Tick(object sender, EventArgs e)
            {

                APISnakeClient.GetSnakeAsync().ContinueWith(t =>
                {
                    Game = t.Result;
                });
                if (Game != null)
                {
                    try
                    {
                        if (Turn != Game.TurnNumber)
                        {
                            Snake snake = Game._Snake;
                            Turn = Game.TurnNumber;
                            ObservableCollection<Cell> _Cells = new ObservableCollection<Cell>();
                            Cords cord = new Cords(0, 0);

                            for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                            {
                                for (cord.X = 1; cord.X <= Width; cord.X++)
                                {
                                    if (!snake.CheckCollision(cord))
                                        _Cells.Add(new Cell("Green"));
                                    else if (snake.CordsCmp(snake.Food, cord))
                                        _Cells.Add(new Cell("Red"));
                                    else
                                        _Cells.Add(new Cell("White"));

                                }
                            }
                            Cells = _Cells;
                        }
                    }
                    catch (Exception E) { }
                }
            }*/

        public async void UpdateView()
        {
            await Task.Run(() => {
                DateTime GameTime = DateTime.Now;
                int TurnNumber = -1;
                while (WantPlaying)
                {
                    TimeSpan Interval = DateTime.Now.Subtract(GameTime);
                    Int64 IntervalMS = Convert.ToInt64(Interval.TotalMilliseconds);
                    if (IntervalMS >= TimeTo)
                    {
                        try
                        {
                            Snake snake = null;
                            GameBoard Game = APISnakeClient.GetBoard();
                            //APISnakeClient.GetSnakeAsync().ContinueWith(t => { TurnNumber = t.Result.TurnNumber; });
                            if (Game != null)
                                TurnNumber = Game.TurnNumber;
                            if (Turn != TurnNumber && Game != null)
                            {

                                snake = Game._Snake;
                                Turn = TurnNumber;
                                ObservableCollection<Cell> _Cells = new ObservableCollection<Cell>();
                                Cords cord = new Cords(0, 0);

                                for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                                {
                                    for (cord.X = 1; cord.X <= Width; cord.X++)
                                    {
                                        if (!snake.CheckCollision(cord))
                                            _Cells.Add(new Cell("Green"));
                                        else if (snake.CordsCmp(snake.Food, cord))
                                            _Cells.Add(new Cell("Red"));
                                        else
                                            _Cells.Add(new Cell("White"));

                                    }
                                }
                                Cells = _Cells;
                                
                            }
                            GameTime = DateTime.Now;
                        }
                        catch (Exception e) { }
                    }
                }
            });
        }

        /*
    void timer_Tick(object sender, EventArgs e)
    {
        Snake snake = null;
        GameBoard Game = APISnakeClient.GetBoard();
        if (Game != null)
            snake = Game._Snake;

        if (snake != null )//&& Turn != Game.TurnNumber)
        {
            try
            {
                //Turn = Game.TurnNumber;
                Cells = new ObservableCollection<Cell>();
                Cords cord = new Cords(0, 0);

                for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                {
                    for (cord.X = 1; cord.X <= Width; cord.X++)
                    {
                        if (!snake.CheckCollision(cord))
                            Cells.Add(new Cell("Green"));
                        else if (snake.CordsCmp(snake.Food, cord))
                            Cells.Add(new Cell("Red"));
                        else
                            Cells.Add(new Cell("White"));
                    }
                }
            }
            catch (Exception E) { }
        }
    }*/
        /*
                //FinalMB
            void timer_Tick(object sender, EventArgs e)
            {

                APISnakeClient.GetSnakeAsync().ContinueWith(t =>
                {
                    Game = t.Result;
                });
                if (Game != null)
                {
                    try
                    {
                        if (Turn != Game.TurnNumber)
                        {
                            Snake snake = Game._Snake;
                            Turn = Game.TurnNumber;
                            ObservableCollection<Cell> _Cells = new ObservableCollection<Cell>();
                            Cords cord = new Cords(0, 0);

                            for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                            {
                                for (cord.X = 1; cord.X <= Width; cord.X++)
                                {
                                    if (!snake.CheckCollision(cord))
                                        _Cells.Add(new Cell("Green"));
                                    else if (snake.CordsCmp(snake.Food, cord))
                                        _Cells.Add(new Cell("Red"));
                                    else
                                        _Cells.Add(new Cell("White"));

                                }
                            }
                            Cells = _Cells;
                        }
                    }
                    catch (Exception E) { }
                }
            }*/
        /*
        void timer_Tick(object sender, EventArgs e)
        {
            Snake snake = null;
            GameBoard Game = null;
            APISnakeClient.GetSnakeAsync().ContinueWith(t =>
            {
                Game = t.Result;
                if (Game != null)
                {
                    try
                    { 
                        snake = Game._Snake;
                        if (Turn != Game.TurnNumber)
                        {
                            Turn = Game.TurnNumber;
                            ObservableCollection<Cell> _Cells = new ObservableCollection<Cell>();
                            Cords cord = new Cords(0, 0);

                            for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                            {
                                for (cord.X = 1; cord.X <= Width; cord.X++)
                                {
                                    if (!snake.CheckCollision(cord))
                                        _Cells.Add(new Cell("Green"));
                                    else if (snake.CordsCmp(snake.Food, cord))
                                        _Cells.Add(new Cell("Red"));
                                    else
                                        _Cells.Add(new Cell("White"));

                                }
                            }
                            Cells = _Cells;
                        }
                    }
                    catch (Exception E) { }
                }
            });
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
