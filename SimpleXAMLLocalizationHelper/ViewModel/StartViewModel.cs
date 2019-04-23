using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.Messages;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace SimpleXAMLLocalizationHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class StartViewModel : ViewModelBase
    {
        private string _verText;

        public string VerText
        {
            get
            {
                return _verText;
            }
            set
            {
                Set(nameof(VerText), ref _verText, value);
            }
        }

        private string _lastUsed;

        public string LastUsed
        {
            get
            {
                return _lastUsed;
            }
            set
            {
                Set(nameof(LastUsed), ref _lastUsed, value);
            }
        }

        private SettingModel newSetting;

        public ICommand StartCoreCommand { get; set; }
        public ICommand OpenSettingCommand { get; set; }
        public ICommand ShowHistoryCommand { get; set; }
        public ICommand ResetSettingCommand { get; set; }

        public StartViewModel()
        {
            StartCoreCommand = new RelayCommand(() => ExecuteStartCoreCommand());
            OpenSettingCommand = new RelayCommand(() => ExecuteOpenSettingCommand());
            ShowHistoryCommand = new RelayCommand(() => ExecuteShowHistoryCommand());
            ResetSettingCommand = new RelayCommand(() => ExecuteResetSettingCommand());
            VerText = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            try
            {
                LoadSettings();
            }
            catch (Exception e)
            {
                App.LoggerEx.Error("프로그램 초기화 실패 " + e);
                Application.Current.Shutdown();
            }
        }

        private void ExecuteStartCoreCommand()
        {
            try
            {
                LoadSettings();
                Messenger.Default.Send<GotoPageMessage>(new GotoPageMessage(PageName.Core));
            }
            catch (Exception e)
            {
                MessageBox.Show("설정이 올바르게 로드되지 않았습니다. \n" + e.Message);
            }
        }

        private void ExecuteOpenSettingCommand()
        {
            LoadSettings();
            Messenger.Default.Send<GotoPageMessage>(new GotoPageMessage(PageName.Setting));
        }

        private void ExecuteShowHistoryCommand()
        {
            Messenger.Default.Send<GotoPageMessage>(new GotoPageMessage(PageName.History));
        }

        private void ExecuteResetSettingCommand()
        {
            MessageBox.Show("성공적으로 설정을 초기화하였습니다.\n(초기 설정: Korean.xaml, English.xaml)");
            WriteDefaultSettings();
        }

        private void LoadSettings()
        {
            try
            {
                newSetting = App.LoadSettings();
            }
            catch
            {
                newSetting = App.LoadSettings();
                App.Logger.Info("기본 설정 재로드 완료.");
            }
            LastUsed = App.LastUsed;
        }

        private void WriteDefaultSettings()
        {
            App.WriteDefaultSettings();
        }
    }
}