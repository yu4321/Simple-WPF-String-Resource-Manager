using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using static SimpleXAMLLocalizationHelper.Utils.Common;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using SimpleXAMLLocalizationHelper.View;
using System.Data;
using SimpleXAMLLocalizationHelper.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace SimpleXAMLLocalizationHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AutoEditViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the AutoEditViewModel class.
        /// </summary>
        #region properties and variables

        private DataTable _newItemsDG = new DataTable();

        public DataTable NewItemsDG
        {
            get
            {
                return _newItemsDG;
            }

            set
            {
                Set(nameof(NewItemsDG), ref _newItemsDG, value);
            }
        }


        private ObservableCollection<ReplaceModel> _selectedToReplace = new ObservableCollection<ReplaceModel>();

        public ObservableCollection<ReplaceModel> SelectedToReplace
        {
            get
            {
                return _selectedToReplace;
            }
            set
            {
                Set(nameof(SelectedToReplace), ref _selectedToReplace, value);
            }
        }



        private bool _isSetComplete;

        public bool IsSetComplete
        {
            get
            {
                return _isSetComplete;
            }

            set
            {
                Set(nameof(IsSetComplete), ref _isSetComplete, value);
            }
        }

        private bool _isfoldermode;
        public bool IsFolderMode
        {
            get
            {
                return _isfoldermode;
            }
            set
            {
                Set(nameof(IsFolderMode), ref _isfoldermode, value);
            }
        }

        private string _currentBase = "현재 비교 대상 없음";
        public string CurrentBase
        {
            get
            {
                return _currentBase;
            }

            set
            {
                Set(nameof(CurrentBase), ref _currentBase, value);
            }
        }

        private string _currentNewPath = "현재 선택된 대상 없음";
        public string CurrentNewPath
        {
            get
            {
                return _currentNewPath;
            }

            set
            {
                Set(nameof(CurrentNewPath), ref _currentNewPath, value);
            }
        }

        private string _langMode = "Korean";
        public string LangMode
        {
            get
            {
                return _langMode;
            }

            set
            {
                Set(nameof(LangMode), ref _langMode, value);
            }
        }

        private DataTable _dataItems = null;
        public DataTable DataItems
        {
            get
            {
                return _dataItems;
            }
            set
            {
                Set(nameof(DataItems), ref _dataItems, value);
            }
        }

        private string _currentFolderPath = "현재 선택된 폴더 없음";
        public string CurrentFolderPath
        {
            get
            {
                return _currentFolderPath;
            }

            set
            {
                Set(nameof(CurrentFolderPath), ref _currentFolderPath, value);
            }
        }

        private ViewModelBase _workerViewModel = null;
        public ViewModelBase WorkerViewModel
        {
            get
            {
                return _workerViewModel;
            }
            set
            {
                Set(nameof(WorkerViewModel), ref _workerViewModel, value);
            }
        }

        private ObservableCollection<string> _langList = new ObservableCollection<string>();

        public ObservableCollection<string> LangList
        {
            get
            {
                return _langList;
            }
            set
            {
                Set(nameof(LangList), ref _langList, value);
            }
        }

        public ICommand OpenDataGridCommand { get; set; }
        public ICommand OpenBaseFileCommand { get; set; }
        public ICommand OpenBaseFolderCommand { get; set; }
        public ICommand ChangeLangCommand { get; set; }
        public ICommand PreviewEditCommand { get; set; }
        public ICommand BeginAutoEditCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        private ReplaceItem toreplace;

        #endregion

        #region methods

        public AutoEditViewModel()
        {
            CurrentFolderPath = Global.CurrentSelectedPath;
            DataItems = Global.CurrentSelectedItems;
            LangList = new ObservableCollection<string>(App.LanguageList);
            OpenDataGridCommand = new RelayCommand<string>(ExecuteOpenDataGridCommand);
            OpenBaseFileCommand = new RelayCommand(() => ExecuteOpenBaseFileCommand());
            OpenBaseFolderCommand = new RelayCommand(() => ExecuteOpenBaseFolderCommand());
            PreviewEditCommand = new RelayCommand(() => ExecutePreviewEditCommand());
            BeginAutoEditCommand = new RelayCommand(() => ExecuteBeginAutoEditCommand());
            ChangeLangCommand = new RelayCommand<string>(ExecuteChangeLangCommand);
            ResetCommand = new RelayCommand(() => ExecuteResetCommand());
            Messenger.Default.Register<ReplaceSelectedMessage>(this, (x) => ReceiveMessage(x));
            Messenger.Default.Register<ResetMessage>(this, (x) => ReceiveMessage(x));
        }

        private void InitializebyFile(string path, DataTable newitems)
        {
            List<string> filenames = App.LanguageList;
            List<XElement> elements = null;
            XDocument xDoc = null;
            try
            {
                using (var sr = new StreamReader(path))
                {
                    xDoc = XDocument.Load(sr);
                    elements = (from w in xDoc.Descendants()
                                where w.Attribute(xmn + "Key") != null && w.HasElements != true
                                select w).ToList();
                }
            }
            catch
            {
                MessageBox.Show("올바른 xaml 파일을 선택해주세요! 파일의 각 요소는 모두 어트리뷰트 \"x:Key\"를 고유한 값으로 지니고 있어야 합니다.");
                throw;
            }

            if (newitems.Columns.Count <= 0)
            {
                newitems.Columns.Add("ID");
                newitems.PrimaryKey = new DataColumn[] { newitems.Columns["ID"] };
            }
            else
            {
                newitems.Rows.Clear();
            }

            newitems.Columns.Add("NewItem");
            foreach (var x in elements)
            {
                var selectresult = newitems.Select(string.Format("ID = '{0}'", x.Attribute(xmn + "Key").Value));
                if (selectresult.Count() <= 0)
                {
                    DataRow dr = newitems.NewRow();
                    dr["ID"] = x.Attribute(xmn + "Key").Value;
                    dr["NewItem"] = x.Value;
                    newitems.Rows.Add(dr);
                }
                else
                {
                    newitems.Rows.Find((string)x.Attribute(xmn + "Key").Value)["NewItem"] = x.Value;
                }
            }
        }

        private void InitializebyFolder(string path, DataTable newitems)
        {
            List<string> filenames = App.LanguageList;
            Dictionary<string, XElement[]> elements = new Dictionary<string, XElement[]>();
            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();

            if (CurrentFolderPath.Contains(path) || path.Contains(CurrentFolderPath))
            {
                MessageBox.Show("완전히 동일한 폴더를 선택했습니다!");
                throw new Exception();
            }

            try
            {
                foreach (string name in filenames)
                {
                    using (var sr = new StreamReader(path + name + ".xaml"))
                    {
                        XDocument xDoc = XDocument.Load(sr);
                        XElement[] xArray = (from w in xDoc.Descendants()
                                             where w.Attribute(xmn + "Key") != null && w.HasElements != true
                                             select w).ToArray();
                        xDocs.Add(name, xDoc);
                        elements.Add(name, xArray);
                    }
                }
            }
            catch
            {
                MessageBox.Show("올바른 폴더를 선택해주세요!\n폴더 안에 들어가는 각 파일 명은 설정과 동일해야 합니다. 또한 각 요소는 모두 어트리뷰트 \"x:Key\"를 고유한 값으로 지니고 있어야 합니다.");
                throw;
            }

            if (newitems.Columns.Count <= 0)
            {
                newitems.Columns.Add("ID");
                newitems.PrimaryKey = new DataColumn[] { newitems.Columns["ID"] };
            }
            else
            {
                newitems.Rows.Clear();
            }

            foreach (var name in filenames)
            {
                if (newitems.Columns[name] == null) newitems.Columns.Add(name);
                foreach (var x in elements[name])
                {
                    var selectresult = newitems.Select(string.Format("ID = '{0}'", x.Attribute(xmn + "Key").Value));
                    if (selectresult.Count() <= 0)
                    {
                        DataRow dr = newitems.NewRow();
                        dr["ID"] = x.Attribute(xmn + "Key").Value;
                        dr[name] = x.Value;
                        newitems.Rows.Add(dr);
                    }
                    else
                    {
                        newitems.Rows.Find((string)x.Attribute(xmn + "Key").Value)[name] = x.Value;
                    }
                }
            }
        }

        private ReplaceItem MakeModifyList(DataTable org, DataTable rep)
        {
            ReplaceItem result = new ReplaceItem();
            DataTable orgtoreplace = new DataTable();
            DataTable reptoreplace = new DataTable();
            orgtoreplace.Columns.Add("ID");
            reptoreplace.Columns.Add("ID");
            orgtoreplace.PrimaryKey = new DataColumn[] { orgtoreplace.Columns["ID"] };
            reptoreplace.PrimaryKey = new DataColumn[] { reptoreplace.Columns["ID"] };
            foreach (var name in App.LanguageList)
            {
                orgtoreplace.Columns.Add(name);
            }

            if (IsFolderMode)
            {
                foreach (var name in App.LanguageList)
                {
                    reptoreplace.Columns.Add(name);
                }
            }
            else
            {
                reptoreplace.Columns.Add("NewItem");
            }

            foreach (DataRow i in org.Rows)
            {
                try
                {
                    var x = rep.Select(string.Format("ID = '{0}'", i["ID"]));
                    if (x.Count() > 0)
                    {
                        DataRow next = x.First();
                        bool isoktoadd = false;
                        if (IsFolderMode)
                        {
                            if (org.Columns.Count == rep.Columns.Count)
                            {
                                try
                                {
                                    isoktoadd = false;
                                    foreach (var lang in App.LanguageList)
                                    {
                                        var lefts = (string)next[lang];
                                        var rights = (string)i[lang];
                                        if (lefts != rights)
                                        {
                                            isoktoadd = true;
                                        }
                                    }
                                }
                                catch
                                {
                                    isoktoadd = false;
                                }
                            }
                        }
                        else
                        {
                            if (next["NewItem"] != null)
                            {
                                if ((string)next["NewItem"] != (string)i[LangMode]) isoktoadd = true;
                            }
                        }

                        if (isoktoadd)
                        {
                            reptoreplace.ImportRow(next);
                            orgtoreplace.ImportRow(i);
                        }
                    }
                }
                catch (Exception e)
                {
                    App.LoggerEx.Warn(e.Message);
                    continue;
                }
            }
            result.isfoldermode = IsFolderMode;
            result.path = CurrentNewPath;
            result.orgpath = CurrentFolderPath + "/";
            result.originalTable = orgtoreplace;
            result.replaceTable = reptoreplace;
            result.langmode = LangMode;

            return result;
        }


        #endregion

        #region command methods

        private void ExecuteOpenDataGridCommand(string param)
        {
            if (param == "DG1")
            {
                new DataGridWindowView(DataItems).ShowDialog();
            }
            else
            {
                new DataGridWindowView(NewItemsDG).ShowDialog();
            }
        }

        private void ExecuteOpenBaseFileCommand()
        {
            try
            {
                string folderpath = Utils.Common.GetFilePath((from x in Application.Current.Windows.OfType<Window>() where x.Title == "AutoEditView" select x).First());
                InitializebyFile(folderpath.Substring(0, folderpath.Length - 1), NewItemsDG);
                IsSetComplete = true;
                CurrentBase = "현재 비교: 파일";
                IsFolderMode = false;
                CurrentNewPath = folderpath.Substring(0, folderpath.Length - 1);
                toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            catch (Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }
        }

        private void ExecuteOpenBaseFolderCommand()
        {
            try
            {
                string folderpath = Utils.Common.GetFolderPath((from x in Application.Current.Windows.OfType<Window>() where x.Title == "AutoEditView" select x).First());
                InitializebyFolder(folderpath, NewItemsDG);
                IsSetComplete = true;
                CurrentBase = "현재 비교: 폴더";
                IsFolderMode = true;
                CurrentNewPath = folderpath;
                toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            catch (Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }
        }

        private void ReopenBaseFile(string oldpath)
        {
            try
            {
                string folderpath = oldpath;
                InitializebyFile(folderpath.Substring(0, folderpath.Length - 1), NewItemsDG);
                IsSetComplete = true;
                CurrentBase = "현재 비교: 파일";
                IsFolderMode = false;
                CurrentNewPath = folderpath.Substring(0, folderpath.Length - 1);
                toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            catch (Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }
        }

        private void ReopenBaseFolder(string oldpath)
        {
            try
            {
                string folderpath = oldpath;
                InitializebyFolder(folderpath, NewItemsDG);
                IsSetComplete = true;
                CurrentBase = "현재 비교: 폴더";
                IsFolderMode = true;
                CurrentNewPath = folderpath;
                toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            catch (Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }
        }

        private void ReceiveMessage(ReplaceSelectedMessage param)
        {
            foreach (var x in param.data)
            {
                if (x.IsChecked == false)
                {
                    try
                    {
                        toreplace.originalTable.AsEnumerable().Where(dr => (string)dr[0] == x.ID).Single().Delete();//.Select($"ID={x.ID}").Single().Delete();
                        toreplace.replaceTable.AsEnumerable().Where(dr => (string)dr[0] == x.ID).Single().Delete();
                    }
                    catch (Exception e)
                    {
                        App.Logger.Error(e);
                    }
                }
            }
        }

        private void ReceiveMessage(ResetMessage action)
        {
            WorkerViewModel = null;
            string oldpath = CurrentNewPath;
            ExecuteResetCommand();
               
            if (IsFolderMode)
            {
                ReopenBaseFolder(oldpath);
            }
            else
            {
                ReopenBaseFile(oldpath + "/");
            }
        }

        private void ExecutePreviewEditCommand()
        {
            if (LangMode != toreplace.langmode) toreplace = MakeModifyList(DataItems, NewItemsDG);
            new PreviewView(new ObservableCollection<ReplaceModel>(toreplace.ToModelList())).ShowDialog();
        }

        private void ExecuteBeginAutoEditCommand()
        {
            if (toreplace == null)
            {
                toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            if (toreplace.replaceTable.Rows.Count <= 0)
            {
                MessageBox.Show("수정할 사항이 존재하지 않습니다.");
                return;
            }
            WorkerViewModel = new AutoEditExecuteViewModel(toreplace);
        }

        private void ExecuteChangeLangCommand(string param)
        {
            if (IsFolderMode)
            {
                MessageBox.Show("폴더 선택시에는 언어 선택은 사용 불가능합니다.");
                return;
            }
            LangMode = param;
            toreplace = MakeModifyList(DataItems, NewItemsDG);
        }

        private void ExecuteResetCommand()
        {
            IsSetComplete = false;
            NewItemsDG = new DataTable();
            CurrentBase = "현재 비교 대상 없음";
            toreplace = null;
            CurrentNewPath = "현재 선택된 대상 없음"; ;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _newItemsDG.Clear();
                    _newItemsDG.Dispose();
                    toreplace.Dispose();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~AutoEditViewModel() {
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

        #endregion
    }
}