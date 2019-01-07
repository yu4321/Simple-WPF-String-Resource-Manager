using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleXAMLLocalizationHelper.Model
{
    public class LanguageBoxItem : INotifyPropertyChanged
    {
        private string _language;
        private string _content;


        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
                RaisePropertyChanged(nameof(Language));
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged(nameof(Content));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
