using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleXAMLLocalizationHelper.Messages
{
    public class ResetMessage
    {
        public bool isSoftReset;
        public ResetMessage(bool _isSoftReset = false)
        {
            isSoftReset = _isSoftReset;
        }
    }
}
