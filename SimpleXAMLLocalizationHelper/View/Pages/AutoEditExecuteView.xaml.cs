using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.Messages;
using System.Windows.Controls;

namespace SimpleXAMLLocalizationHelper.View
{
    /// <summary>
    /// AutoEditExcuteView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AutoEditExecuteView : UserControl
    {
        public AutoEditExecuteView()
        {
            InitializeComponent();
            Messenger.Default.Send<ChangeWindowStateMessage>(new ChangeWindowStateMessage());
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Log.ScrollToEnd();
        }
    }
}