using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using SimpleXAMLLocalizationHelper.Messages;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.IO;
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
        public const string defaultSetting = "{\"LAST_LOGIN\": \"2019-01-01T00:00:00\",\"USE_LANGUAGES\": [\"Korean\",\"English\"]}";

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
            catch
            {

            }
        }

        private void ExecuteStartCoreCommand()
        {
            try
            {
                LoadSettings();
                Messenger.Default.Send<GotoPageMessage>(new GotoPageMessage(PageName.Core));
            }
            catch
            {

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
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            using (FileStream fs=new FileStream("Setting.json", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    StreamReader sr = new StreamReader(fs);
                    newSetting = JsonConvert.DeserializeObject<SettingModel>(sr.ReadToEnd());
                    App.LanguageList = newSetting.USE_LANGUAGES;
                }
                catch(Exception e)
                {
                    WriteDefaultSettings();
                    MessageBox.Show("설정 파일이 손상되었습니다. 설정을 초기화하였습니다.");
                    App.LoggerEx.Warn(e.Message);
                    throw;
                }
            }
            WriteTimeStamptoSetting();
        }
        
        private void WriteTimeStamptoSetting()
        {
            using (StreamWriter sw = new StreamWriter("Setting.json"))
            {
                try
                {
                    if (newSetting != null)
                    {
                        newSetting.LAST_LOGIN = DateTime.Now.ToLocalTime();
                        LastUsed = newSetting.LAST_LOGIN.ToString();
                        string serialized = JsonConvert.SerializeObject(newSetting,Formatting.Indented);
                        sw.Write(serialized);
                    }
                }
                catch
                {

                }
            }
        }

        private void WriteDefaultSettings()
        {
            using (StreamWriter sw = new StreamWriter("Setting.json"))
            {
                try
                {
                    sw.Write(defaultSetting.ToString());
                }
                catch
                {
                    
                }
            }
        }
    }
}