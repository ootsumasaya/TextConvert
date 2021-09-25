using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextConvert.Models
{
    class ConvertModel
    {



        public event PropertyChangedEventHandler PropertyChanged;
        public string BeforeConvertItem
        {
            get { return _BeforeConvertItem; }
            set
            {
                if (_BeforeConvertItem != value)
                {
                    _BeforeConvertItem = value;
                    OnPropertyChanged(nameof(BeforeConvertItem));
                }
            }
        }
        private string _BeforeConvertItem;

        public string AfterConvertItem
        {
            get { return _AfterConvertItem; }
            set
            {
                if (_AfterConvertItem != value)
                {
                    _AfterConvertItem = value;
                    OnPropertyChanged(nameof(AfterConvertItem));
                }
            }
        }
        private string _AfterConvertItem;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
