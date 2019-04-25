using System.Data;
using System.Windows;

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