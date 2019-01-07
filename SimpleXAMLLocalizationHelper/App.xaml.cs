using GalaSoft.MvvmLight.Threading;
using log4net;
using System.Collections.Generic;
using System.Windows;

namespace SimpleXAMLLocalizationHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog Logger = Utils.Logger.LogWriterMessage;
        public static readonly ILog LoggerEx = Utils.Logger.LogWriterException;
        public static List<string> LanguageList = new List<string>();

        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}