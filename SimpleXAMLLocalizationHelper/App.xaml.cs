using GalaSoft.MvvmLight.Threading;
using log4net;
using Newtonsoft.Json;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static Dictionary<string, string> Favorites = new Dictionary<string, string>();
        public static SettingModel lastSetting;
        public static string LastUsed;
        public const string defaultSetting = "{\"LAST_LOGIN\": \"2019-01-01T00:00:00\",\"USE_LANGUAGES\": [\"Korean\",\"English\"],\"FAVORITES\": {\"Default\": \"C://Languages\"}}";

        static App()
        {
            DispatcherHelper.Initialize();
        }

        public static void SaveSetting()
        {
            using (StreamWriter sw = new StreamWriter("Setting.json"))
            {
                try
                {
                    if (lastSetting != null)
                    {
                        lastSetting.LAST_LOGIN = DateTime.Now.ToLocalTime();
                        LastUsed = lastSetting.LAST_LOGIN.ToString();
                        string serialized = JsonConvert.SerializeObject(lastSetting, Formatting.Indented);
                        sw.Write(serialized);
                    }
                }
                catch
                {
                    App.Logger.Info("설정 저장 실패");
                }
            }
        }

        public static SettingModel LoadSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            SettingModel newSetting ;

            try
            {
                using (FileStream fs = new FileStream("Setting.json", FileMode.OpenOrCreate, FileAccess.Read))
                //using(StreamReader sr=new StreamReader("Setting.json"))
                {
                    StreamReader sr = new StreamReader(fs);
                    //var xxxx = sr.ReadToEnd();
                    newSetting = JsonConvert.DeserializeObject<SettingModel>(sr.ReadToEnd());
                    App.LanguageList = newSetting.USE_LANGUAGES;
                    App.Favorites = newSetting.FAVORITES;
                }
            }
            catch (Exception e)
            {
                WriteDefaultSettings();
                MessageBox.Show("설정 파일이 손상되었습니다. 설정을 초기화하였습니다.");
                App.LoggerEx.Warn(e.Message);
                throw;
            }

            WriteTimeStamptoSetting(newSetting);
            lastSetting = newSetting;
            return newSetting;
        }

        public static void WriteTimeStamptoSetting(SettingModel newSetting)
        {
            using (StreamWriter sw = new StreamWriter("Setting.json"))
            {
                try
                {
                    if (newSetting != null)
                    {
                        newSetting.LAST_LOGIN = DateTime.Now.ToLocalTime();
                        LastUsed = newSetting.LAST_LOGIN.ToString();
                        string serialized = JsonConvert.SerializeObject(newSetting, Formatting.Indented);
                        sw.Write(serialized);
                    }
                }
                catch(Exception e)
                {
                    App.Logger.Info("마지막 로그인 시간 저장 실패 "+e.Message);
                }
            }
        }

        public static void WriteDefaultSettings()
        {
            using (StreamWriter sw = new StreamWriter("Setting.json"))
            {
                try
                {
                    sw.Write(defaultSetting.ToString());
                }
                catch(Exception e)
                {
                    App.Logger.Info("기본 설정 저장 실패 " + e.Message);
                }
            }
        }
    }
}