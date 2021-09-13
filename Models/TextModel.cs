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
                    AfterText = Convert(value);
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

        //改行を全て消し、そのあとピリオドごとに改行を入れる
        public string Convert(string x)
        {
            if (x is null)
            {
                return "";
            }
            else
            {
                x = x.Replace("\r", "");
                x = x.Replace("\n", "");
                x = x.Replace(".", ".\n\n");
                return x;
            }
        }
    }
}
