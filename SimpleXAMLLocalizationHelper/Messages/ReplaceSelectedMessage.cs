using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleXAMLLocalizationHelper.Messages
{
    public class ReplaceSelectedMessage
    {
        public ObservableCollection<ReplaceModel> data = null;
        public ReplaceSelectedMessage(ObservableCollection<ReplaceModel> param)
        {
            data = param;
        }
    }
}
