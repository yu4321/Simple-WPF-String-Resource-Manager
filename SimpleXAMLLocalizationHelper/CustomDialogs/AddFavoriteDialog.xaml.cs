using System.Windows;

namespace SimpleXAMLLocalizationHelper.CustomDialogs
{
    /// <summary>
    /// AddFavoriteDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddFavoriteDialog : Window
    {
        public string key;
        public string value;
        public bool result = false;

        public AddFavoriteDialog(string param)
        {
            value = param;
            InitializeComponent();
            valueBox.Text = value;
        }

        public bool? ShowDialog(Window owner)
        {
            this.Owner = owner;
            return ShowDialog();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (keyBox.Text.Length > 0)
            {
                try
                {
                    key = keyBox.Text;
                    App.lastSetting.FAVORITES.Add(key, value);
                    App.SaveSetting();
                    result = true;
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("겹치는 이름이 존재합니다!");
                }
            }
            else
            {
                MessageBox.Show("이름을 입력해주세요!");
            }
        }
    }
}