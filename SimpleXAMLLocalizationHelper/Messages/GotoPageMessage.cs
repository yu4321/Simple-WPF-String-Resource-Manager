using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleXAMLLocalizationHelper.Messages
{
    public enum PageName { Start, Core, Setting, History}
    public class GotoPageMessage
    {
        public PageName NextPage;
        public GotoPageMessage(PageName _nextPage)
        {
            NextPage = _nextPage;
        }
    }
}
