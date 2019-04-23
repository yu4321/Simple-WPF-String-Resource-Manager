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