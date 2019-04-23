using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.Messages;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using static SimpleXAMLLocalizationHelper.Utils.Common;

namespace SimpleXAMLLocalizationHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AutoEditExecuteViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the AutoEditExecuteViewModel class.
        /// </summary>
        private double _progress;

        private double _progressMax;
        private string _log;
        private ReplaceItem workList;
        private bool _canexit;

        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                Set(nameof(Progress), ref _progress, value);
            }
        }

        public double ProgressMax
        {
            get
            {
                return _progressMax;
            }
            set
            {
                Set(nameof(ProgressMax), ref _progressMax, value);
            }
        }

        public string Log
        {
            get
            {
                return _log;
            }
            set
            {
                Set(nameof(Log), ref _log, value);
            }
        }

        public bool CanExit
        {
            get
            {
                return _canexit;
            }
            set
            {
                Set(nameof(CanExit), ref _canexit, value);
            }
        }

        private List<string> LangList = new List<string>();

        public ICommand CloseCommand { get; set; }

        public AutoEditExecuteViewModel(ReplaceItem _worklist)
        {
            workList = _worklist;
            CloseCommand = new RelayCommand(() =>
              {
                  Messenger.Default.Send<ResetMessage>(new ResetMessage(true));
                  GC.Collect();
              });
            ReadyWork();
            new Thread(new ThreadStart(MainWork)).Start();
        }

        public void MainWork()
        {
            if (workList.isfoldermode)
            {
                for (int i = 0; i < workList.replaceTable.Rows.Count; i++)
                {
                    Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();
                    DataRow org = workList.originalTable.Rows[i];
                    DataRow rep = workList.replaceTable.Rows[i];
                    try
                    {
                        foreach (var name in LangList)
                        {
                            XDocument xDoc = null;
                            ModifyElementwithIDandValueFolderPath(workList.orgpath, name, (string)org["ID"], (string)rep[name], ref xDoc);
                            xDocs.Add(name, xDoc);
                        }
                        Progress++;
                        SaveFiles(xDocs);
                        Progress++;

                        foreach (var x in xDocs)
                        {
                            string logtext = string.Format("저장 성공 {0}/{1}.xaml ID: {2}, {3}으로 변경.", workList.orgpath, x.Key, (string)org["ID"], (string)rep[x.Key]);
                            Log += logtext + "\n";
                            App.Logger.Info(logtext);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        Log += string.Format("갱신 불가 {0} - 값이 NULL입니다. \n", org["ID"]);
                        App.Logger.Info(string.Format("{0}의 {1} - NULL값 미처리", workList.langmode, org["ID"]));
                        Progress += 2;
                    }
                    catch (Exception e)
                    {
                        Log += string.Format("갱신 실패 {0} - {1} \n", org["ID"], e.Message);
                        App.Logger.Warn(string.Format("갱신 실패 {0} {1} - {2} \n", workList.langmode, org["ID"], e.Message));
                    }
                }
            }
            else
            {
                for (int i = 0; i < workList.replaceTable.Rows.Count; i++)
                {
                    DataRow org = workList.originalTable.Rows[i];
                    DataRow rep = workList.replaceTable.Rows[i];
                    try
                    {
                        XDocument xDoc = null;
                        ModifyElementwithIDandValueFolderPath(workList.orgpath, workList.langmode, (string)org["ID"], (string)rep["NewItem"], ref xDoc);
                        Progress++;
                        ReplaceCarriageReturns_XDoc(ref xDoc);
                        SaveFileFullPath(workList.orgpath, workList.langmode, xDoc.ToString());
                        Progress++;
                        string logtext = string.Format("저장 성공 {0}/{1}.xaml - {2}, {3}으로 변경", workList.orgpath, workList.langmode, (string)org["ID"], (string)rep["NewItem"]);
                        App.Logger.Info(logtext);
                        Log += logtext + "\n";
                    }
                    catch (Exception e)
                    {
                        Log += string.Format("갱신 실패 {0} - {1} \n", org["ID"], e.Message);
                        App.Logger.Warn(string.Format("갱신 실패 {0} {1} - {2} \n", workList.langmode, org["ID"], e.Message));
                    }
                }
            }

            CanExit = true;
        }

        public void SaveFiles(Dictionary<string, XDocument> xDocs)
        {
            List<string> param = new List<string>();
            try
            {
                foreach (var x in LangList)
                {
                    xDocs[x] = ReplaceCarriageReturns_XDoc(xDocs[x]);
                    param.Add(xDocs[x].ToString());
                }
            }
            catch (Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }

            for (int i = 0; i < LangList.Count; i++)
            {
                SaveFileFullPath(workList.orgpath, LangList[i], param[i].ToString());
            }
        }

        public void ReadyWork()
        {
            Progress = 0;
            ProgressMax = workList.replaceTable.Rows.Count * 2;
            for (int i = 1; i < workList.originalTable.Columns.Count; i++)
            {
                DataColumn x = workList.originalTable.Columns[i];
                LangList.Add(x.ColumnName);
            }
        }

        private void SaveFileFullPath(string path, string location, string xmlstr)
        {
            try
            {
                using (StreamWriter wr = new StreamWriter(path + location + ".xaml"))
                {
                    ReplaceCarriageReturns_String(ref xmlstr);
                    wr.Write(xmlstr);
                }
            }
            catch
            {
                Log += string.Format("저장 실패 {0} \n", path + location + ".xaml");
                App.Logger.Warn(string.Format("저장 실패 {0} \n", path + location + ".xaml"));
            }
        }
    }

    public class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            return ((int)(d / 2)).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}