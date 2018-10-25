using GalaSoft.MvvmLight;

namespace SimpleXAMLLocalizationHelper.Model
{
    public class DataItem : ObservableObject
    {
        private string _ID;
        private string _Kor;
        private string _Eng;
        private string _Jpn;
        private string _Chns;
        //private string _Chnt;

        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                Set(nameof(ID), ref _ID, value);
            }
        }

        public string Kor
        {
            get
            {
                return _Kor;
            }
            set
            {
                //value=value.Replace("\r\n", "&#xA;");
                Set(nameof(Kor), ref _Kor, value);
            }
        }

        public string Eng
        {
            get
            {
                return _Eng;
            }
            set
            {
                //value=value.Replace("\r\n", "&#xA;");
                Set(nameof(Eng), ref _Eng, value);
            }
        }

        public string Jpn
        {
            get
            {
                return _Jpn;
            }
            set
            {
                //value=value.Replace("\r\n", "&#xA;");
                Set(nameof(Jpn), ref _Jpn, value);
            }
        }

        public string Chns
        {
            get
            {
                return _Chns;
            }
            set
            {
                //value=value.Replace("\r\n", "&#xA;");
                Set(nameof(Chns), ref _Chns, value);
            }
        }

        /*
        public string Chnt
        {
            get
            {
                return _Chnt;
            }
            set
            {
                value=value.Replace("\r\n", "&#xA;");
                Set(nameof(Chnt), ref _Chnt, value);
            }
        }
        */
    }
}