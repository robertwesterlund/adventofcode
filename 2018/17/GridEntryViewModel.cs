using System.ComponentModel;

namespace _17
{
    public class GridEntryViewModel : INotifyPropertyChanged
    {
        public int X { get; set; }
        public int Y { get; set; }

        private GridEntryType _type;
        public GridEntryType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        public int ViewBoxX { get; internal set; }
        public int ViewBoxY { get; internal set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}