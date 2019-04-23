using SimpleXAMLLocalizationHelper.Model;
using System.Collections.ObjectModel;

namespace SimpleXAMLLocalizationHelper.Messages
{
    public class ReplaceSelectedMessage
    {
        public ObservableCollection<ReplacePreviewItem> data = null;

        public ReplaceSelectedMessage(ObservableCollection<ReplacePreviewItem> param)
        {
            data = param;
        }
    }
}