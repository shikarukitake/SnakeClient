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
        private Map selectedPhone;
        public ObservableCollection<Map> Phones { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Index { get; set; }
        public bool GameIsStarted { get; set; }
        DispatcherTimer timer { get; set; }
        public Client APISnakeClient { get; set; }
        public string SNAKE { get; set; }
        private string createMap;
        public string CreateMap { get { return createMap; } set {
                OnPropertyChanged("CreateMap");
            } }


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
                          if (GameIsStarted)
                              APISnakeClient.StartGame().ContinueWith(t => { timer.Stop(); });
                          timer.Interval = TimeSpan.FromMilliseconds(_map.TimeUntilNextTurnMS);
                          Width = _map.Width;
                          Height = _map.Height;
                          timer.Tick += timer_Tick;

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

                  }));
            }
        }

        private RelayCommand getCommand;
        public RelayCommand GetCommand
        {
            get
            {
                return getCommand ??
                  (getCommand = new RelayCommand(obj =>
                  {
                      APISnakeClient.StartGame().ContinueWith(t => { timer.Start(); });
                  }));
            }
        }

        public ICommand KeyPressedCommand { get; set; }
        
        private void KeyPressed(object parameter)
        {
            var key = (string)parameter;
            APISnakeClient.DirectionChange(key).ContinueWith(t => { });
        }

        public Map SelectedPhone
        {
            get { return selectedPhone; }
            set
            {
                selectedPhone = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        
        public ApplicationViewModel()
        {
            Phones = new ObservableCollection<Map>();
            SelectedPhone = new Map();
            KeyPressedCommand = new RelayCommand((parameter) => KeyPressed(parameter));
            timer = new DispatcherTimer();
            APISnakeClient = new Client("https://localhost:44317");
            CreateMap = "CreateMap";
            //
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Snake snake = APISnakeClient.GetSnake();

            if (snake != null)
            {
                Cells = new ObservableCollection<Cell>();
                Cords cord = new Cords(0, 0);

                for (cord.Y = 1; cord.Y <= Height; cord.Y++)
                {
                    for (cord.X = 1; cord.X <= Width; cord.X++)
                    {
                        if (snake.CheckCollision(cord))
                            Cells.Add(new Cell("White"));
                        else if (snake.CordsCmp(snake.Food, cord))
                            Cells.Add(new Cell("Black"));
                        else
                            Cells.Add(new Cell("Green"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
