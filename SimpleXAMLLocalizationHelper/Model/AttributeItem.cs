using GalaSoft.MvvmLight;
using System.Xml.Linq;

namespace SimpleXAMLLocalizationHelper.Model
{
    public class AttributeItem : ObservableObject
    {
        private string _Name;
        private string _Value;
        private XAttribute _Content;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                Set(nameof(Name), ref _Name, value);
            }
        }

        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                Set(nameof(Value), ref _Value, value);
            }
        }

        public XAttribute Content
        {
            get
            {
                return _Content;
            }
            set
            {
                Set(nameof(Content), ref _Content, value);
                Set(nameof(Name), ref _Name, value.Name.ToString());
                Set(nameof(Value), ref _Value, value.Value);
            }
        }

        public override string ToString()
        {
            if (Name != null && Content != null)
            {
                return $"{Name} | {Content}";
            }
            return base.ToString();
        }
    }
}