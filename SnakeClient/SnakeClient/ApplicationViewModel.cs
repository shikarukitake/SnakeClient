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
                          timer.Interval = TimeSpan.FromMilliseconds(_map.TimeUntilNextTurnMS);
                          Width = _map.Width;
                          Height = _map.Height;
                          timer.Tick += timer_Tick;
                          Index = 0;
                          //Request is ok
                          if (true)
                          {
                              GameIsStarted = true;
                              timer.Start();
                          }
                      }

                  }));
            }
        }

        public ICommand KeyPressedCommand { get; set; }
        
        private void KeyPressed(object parameter)
        {
            var key = (string)parameter;
            switch (key)
            {
                case "Up":
                    //request
                    MessageBox.Show("Up");
                    break;
                case "Down":
                    //request
                    MessageBox.Show("Down");
                    break;
                case "Right":
                    //request
                    break;
                case "Left":
                    //request
                    break;
            }

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

        }

        void timer_Tick(object sender, EventArgs e)
        {
            
            Cells = new ObservableCollection<Cell>();
            Index++;
            for (int height = 0; height < Height; height++)
            {
                for (int width = 0; width < Width; width++)
                {
                    if (height == 0 && width == Index)
                        Cells.Add(new Cell("Black"));
                    else
                        Cells.Add(new Cell("Green"));
                    
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
