﻿using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace SimpleXAMLLocalizationHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}