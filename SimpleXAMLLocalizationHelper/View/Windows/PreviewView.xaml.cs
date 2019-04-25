using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.Messages;
using SimpleXAMLLocalizationHelper.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SimpleXAMLLocalizationHelper.View
{
    /// <summary>
    /// PreviewView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PreviewView : Window
    {
        public PreviewView(string s)
        {
            InitializeComponent();
        }

        public PreviewView(ObservableCollection<ReplacePreviewItem> param)
        {
            InitializeComponent();
            if (param != null)
                dg.ItemsSource = param;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Send<ReplaceSelectedMessage>(new ReplaceSelectedMessage(dg.ItemsSource as ObservableCollection<ReplacePreviewItem>));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var checkedItems = (dg.ItemsSource as ObservableCollection<ReplacePreviewItem>).Where(x => x.IsChecked && !x.IsAdd).ToList();
            var uncheckedItems = (dg.ItemsSource as ObservableCollection<ReplacePreviewItem>).Where(x => !x.IsChecked && !x.IsAdd).ToList();
            if ((dg.ItemsSource as ObservableCollection<ReplacePreviewItem>).Where(x => x.IsChecked && !x.IsAdd).Count() > 0)
            {
                checkedItems.ForEach(x => x.IsChecked = false);
            }
            else
            {
                uncheckedItems.ForEach(x => x.IsChecked = true);
            }
        }
    }
}