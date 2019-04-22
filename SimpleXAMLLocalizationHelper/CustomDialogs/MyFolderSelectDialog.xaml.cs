using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Shell;
using System.Collections.ObjectModel;

namespace SimpleXAMLLocalizationHelper.CustomDialogs
{
    /// <summary>
    /// FolderSelectDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MyFolderSelectDialog : Window, IDisposable
    {

        //
        // 요약:
        //     Gets or sets a value that determines whether the user can select folders or files.
        //     Default value is false.
        public bool IsFolderPicker { get; set; }

        public string FileName
        {
            get;set;
        }

        public ObservableCollection<KeyValuePair<string, string>> Favorites = new ObservableCollection<KeyValuePair<string, string>>();
        public KeyValuePair<string, string> selected = new KeyValuePair<string, string>();
        public MyFolderSelectDialog()
        {
            Favorites = new ObservableCollection<KeyValuePair<string, string>>(App.Favorites);
            InitializeComponent();
            listBox.ItemsSource = Favorites;
        }

        private void selectfav_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                Console.WriteLine(((KeyValuePair<string, string>)listBox.SelectedItem).Key);
                FileName = ((KeyValuePair<string, string>)listBox.SelectedItem).Value;
                this.Close();
            }
        }

        private void selectnew_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "언어 리소스 파일들이 들어있는 폴더를 선택해주세요.";
                dialog.ShowDialog(this);
                try
                {
                    FileName = dialog.FileName;
                }
                catch
                {
                    FileName = "";
                }
            }
            this.Close();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectfav_Click(sender, e);
        }

        public bool? ShowDialog(Window owner)
        {
            this.Owner = owner;
            return base.ShowDialog();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Favorites.Clear();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~MyFolderSelectDialog() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
