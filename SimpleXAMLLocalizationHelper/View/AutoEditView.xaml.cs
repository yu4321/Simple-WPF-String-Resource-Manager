using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleXAMLLocalizationHelper.View
{
    /// <summary>
    /// AutoEditView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AutoEditView : Window
    {

        public AutoEditView()
        {
            InitializeComponent();
            Messenger.Default.Register<ChangeWindowStateMessage>(this, (x) => ReceiveWindowStateMessage());
            Loaded += AutoEditView_Loaded;
        }

        private void AutoEditView_Loaded(object sender, RoutedEventArgs e)
        {
            ResetTBScrolls();

        }

        private void ResetTBScrolls()
        {
            if (TB1 != null)
            {
                TB1.CaretIndex = TB1.Text.Length;
                var rect = TB1.GetRectFromCharacterIndex(TB1.CaretIndex);
                TB1.ScrollToHorizontalOffset(rect.Right);
            }
            if (TB2 != null)
            {
                TB2.CaretIndex = TB2.Text.Length;
                var rect = TB2.GetRectFromCharacterIndex(TB2.CaretIndex);
                TB2.ScrollToHorizontalOffset(rect.Right);
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetTBScrolls();
        }

        private void ReceiveWindowStateMessage()
        {
            //isoncedl = !isoncedl;
            //if (isoncedl)
            //{
            //    var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            //    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            //}
            //else
            //{
            //    this.WindowStyle = WindowStyle.ToolWindow;
            //}
        }
    }
}
