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
        public bool AllowNonFileSystemItems { get; set; }
        //
        // 요약:
        //     Gets a collection of the selected file names.
        //
        // 설명:
        //     This property should only be used when the Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog.Multiselect
        //     property is true.
        public IEnumerable<string> FileNames { get; }
        //
        // 요약:
        //     Gets a collection of the selected items as ShellObject objects.
        //
        // 설명:
        //     This property should only be used when the Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog.Multiselect
        //     property is true.
        public ICollection<ShellObject> FilesAsShellObject { get; }
        //
        // 요약:
        //     Gets or sets a value that determines whether the user can select folders or files.
        //     Default value is false.
        public bool IsFolderPicker { get; set; }
        //
        // 요약:
        //     Gets or sets a value that determines whether the user can select more than one
        //     file.
        public bool Multiselect { get; set; }

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
            Console.WriteLine(((KeyValuePair < string, string > )listBox.SelectedItem).Key);
            FileName = ((KeyValuePair<string, string>)listBox.SelectedItem).Value;
            this.Close();
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

        public void Dispose()
        {
            return;
        }

        public bool? ShowDialog(Window owner)
        {
            this.Owner = owner;
            return base.ShowDialog();
        }
    }
}
