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
        public MainView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
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
    }
}