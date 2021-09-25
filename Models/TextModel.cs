using System.ComponentModel;

namespace TextConvert.Models
{
    class TextModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string BeforeText
        {
            get { return _BeforeText; }
            set
            {
                if (_BeforeText != value)
                {
                    _BeforeText = value;
                    OnPropertyChanged(nameof(BeforeText));
                }
            }
        }
        private string _BeforeText;

        public string AfterText
        {
            get { return _AfterText; }
            set
            {
                if(_AfterText != value)
                {
                    _AfterText = value;
                    OnPropertyChanged(nameof(AfterText));
                }
            }
        }
        private string _AfterText;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
