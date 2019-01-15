using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleXAMLLocalizationHelper.Messages
{
    public class ChangeWindowStateMessage
    {
        public WindowState state;
        public ChangeWindowStateMessage(WindowState _state)
        {
            state = _state;
        }

        public ChangeWindowStateMessage()
        {

        }
    }
}
