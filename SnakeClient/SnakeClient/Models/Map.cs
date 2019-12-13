using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnakeClient
{
    public class Map : INotifyPropertyChanged
    {
        private int width;
        private int height;
        private int timeUntilNextTurnMS;

        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged("Width");
            }
        }
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }
        public int TimeUntilNextTurnMS
        {
            get { return timeUntilNextTurnMS; }
            set
            {
                timeUntilNextTurnMS = value;
                OnPropertyChanged("Price");
            }
        }

        public Map(int Width, int Height, int TimeUntilNextTurnMS)
        {
            this.Width = Width;
            this.Height = Height;
            this.TimeUntilNextTurnMS = TimeUntilNextTurnMS;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

}