using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// DataGridWindowView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DataGridWindowView : Window
    {

        public DataGridWindowView(DataTable DI)
        {
            InitializeComponent();
            DG.ItemsSource = DI.AsDataView();
        }
    }
}
