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
            TB1.Text = s;
        }
    }
}
