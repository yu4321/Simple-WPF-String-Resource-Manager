using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.Messages;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// PreviewView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PreviewView : Window
    {
        public PreviewView(string s)
        {
            InitializeComponent();
        }

        public PreviewView(ObservableCollection<ReplaceModel> param)
        {
            InitializeComponent();
            if(param!=null)
                dg.ItemsSource = param;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Send<ReplaceSelectedMessage>(new ReplaceSelectedMessage(dg.ItemsSource as ObservableCollection<ReplaceModel>));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var checkedItems = (dg.ItemsSource as ObservableCollection<ReplaceModel>).Where(x => x.IsChecked).ToList();
            var uncheckedItems = (dg.ItemsSource as ObservableCollection<ReplaceModel>).Where(x => x.IsChecked == false).ToList();
            if ((dg.ItemsSource as ObservableCollection<ReplaceModel>).Where(x => x.IsChecked).Count()>0)
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
