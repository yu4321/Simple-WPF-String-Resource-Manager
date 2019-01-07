using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace SimpleXAMLLocalizationHelper.View
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : UserControl
    {
        bool isopened = false;
        public MainView()
        {
            InitializeComponent();
            mainGrid.ColumnDefinitions[2].Width = new System.Windows.GridLength(0);
            btn_addattr.Content = ">>>>";
            isopened = false;
        }

        private void CloseAttrPanel()
        {
            App.Current.MainWindow.Width -= 320;
            mainGrid.ColumnDefinitions[2].Width = new System.Windows.GridLength(0);
            btn_addattr.Content = ">>>>";
            isopened = false;
        }

        private void OpenAttrPanel()
        {
            App.Current.MainWindow.Width += 320;
            mainGrid.ColumnDefinitions[2].Width = new System.Windows.GridLength(320);
            btn_addattr.Content = "<<<<";
            isopened = true;
        }

        private void DataGridSortReset(object sender, System.Windows.RoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(DG.ItemsSource);
            if (view != null)
            {
                view.SortDescriptions.Clear();
                foreach (DataGridColumn column in DG.Columns)
                {
                    column.SortDirection = null;
                }
            }
        }

        private void OpenandCloseAttributePage(object sender, System.Windows.RoutedEventArgs e)
        {
            if (isopened) CloseAttrPanel();
            else OpenAttrPanel();
        }

        private void checkBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (checkBox.IsChecked == false && isopened) CloseAttrPanel();
        }

        protected void Datagrid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;
        }
      
    }
}