using GalaSoft.MvvmLight;

namespace SimpleXAMLLocalizationHelper.Model
{
    public class ReplacePreviewItem : ObservableObject
    {
        private bool _isChecked = true;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                Set(nameof(IsChecked), ref _isChecked, value);
            }
        }

        private bool _isAdd = false;

        public bool IsAdd
        {
            get
            {
                return _isAdd;
            }
            set
            {
                Set(nameof(IsAdd), ref _isAdd, value);
            }
        }

        private string _id;

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                Set(nameof(ID), ref _id, value);
            }
        }

        private string _oldValue;

        public string OldValue
        {
            get
            {
                return _oldValue;
            }
            set
            {
                Set(nameof(OldValue), ref _oldValue, value);
            }
        }

        private string _newValue;

        public string NewValue
        {
            get
            {
                return _newValue;
            }
            set
            {
                Set(nameof(NewValue), ref _newValue, value);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}- {1}", IsChecked ? "v" : "x", ID);
        }
    }
}