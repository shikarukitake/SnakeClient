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
        #region Public props
        public int Width { get; set; }
        public int Height { get; set; }
        public int Turn { get; set; }
        public int TimeTo { get; set; }
        public bool GameIsStarted { get; set; }
        public bool WantPlaying { get; set; }
        DispatcherTimer timer { get; set; }
        public Client APISnakeClient { get; set; }
        #endregion

        #region props OnPropertyChanged
        private Map map;
        public Map SelectedMap
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChanged("SelectedPhone");
            }
        }
        private string createMap;
        public string CreateMap
        {
            get { return createMap; }
            set
            {
                createMap = value;
                OnPropertyChanged("CreateMap");
            }
        }

        //From presentation
        public ObservableCollection<Cell> Cells
        {
            get => _cells;
            private set { _cells = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Cell> _cells = new ObservableCollection<Cell>();
        #endregion

        #region constructor
        public ApplicationViewModel()
        {
            SelectedMap = new Map(20, 20, 1000);
            KeyPressedCommand = new RelayCommand((parameter) => KeyPressed(parameter));
            APISnakeClient = new Client("https://localhost:44317");
            CreateMap = "CreateMap";
        }
        #endregion

        #region commands
        // команда создания карты
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
                          if (WantPlaying)
                          {
                              APISnakeClient.StartGame().ContinueWith(t =>
                                 { WantPlaying = !WantPlaying; });
                          }
                          Width = _map.Width;
                          Height = _map.Height;
                          TimeTo = _map.TimeUntilNextTurnMS / 10;
                          System.Net.HttpStatusCode StatusCode;
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

                  }, c => Height == Width));
            }
        }

        // команда начала игры
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
                          else
                              WantPlaying = !WantPlaying;
                          Turn = -1;
                      });
                  }, c => WantPlaying == false));
            }
        }

        // команда закрытия окна
        private RelayCommand quitCommand;
        public RelayCommand QuitCommand
        {
            get
            {
                return quitCommand ??
                    (quitCommand = new RelayCommand(obj =>
                    {
                        var window = obj as Window;
                        window.Close();
                    })
                );
            }
        }

        // команды для стрелок чтобы перемещаться
        public ICommand KeyPressedCommand { get; set; }
        private void KeyPressed(object parameter)
        {
            var key = (string)parameter;
            APISnakeClient.DirectionChange(key).ContinueWith(t => { });
        }
        #endregion

        // Метод для обновления содержимого отображаемой игровой карты 
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
                                            _Cells.Add(new Cell("White"));
                                        else if (snake.CordsCmp(snake.Food, cord))
                                            _Cells.Add(new Cell("SpringGreen"));
                                        else
                                            _Cells.Add(new Cell("Black"));
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
