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

namespace SimpleXAMLLocalizationHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public partial class CoreViewModel : ViewModelBase
    {
        #region properties and variables
        private ObservableCollection<DataItem> _newItems;

        public ObservableCollection<DataItem> NewItems

        {
            get
            {
                return _newItems;
            }

            set
            {
                Set(nameof(NewItems), ref _newItems, value);
            }
        }

        private DataTable _newItemsDG;

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

        private string _currentBase= "현재 비교 대상 없음";
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

        private string _langMode="Korean";
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

        private ViewModelBase _workerViewModel;
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

        public ICommand OpenAutoEditCommand { get; set; }
        public ICommand OpenFolderSelectCommand { get; set; }
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


        private void InitializebyFile(string path, DataTable newitems)
        {
            List<string> filenames = LangList.ToList(); ;
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
            List<string> filenames = LangList.ToList(); ;
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
            foreach (var name in LangList)
            {
                orgtoreplace.Columns.Add(name);
            }

            if (IsFolderMode)
            {
                foreach(var name in LangList)
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
                                    foreach (var lang in LangList)
                                    {
                                        if((string)next[LangMode] != (string)i[LangMode])
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
                catch(Exception e)
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
                new DataGridWindowView(Items).ShowDialog();
            }
            else
            {
                new DataGridWindowView(NewItems).ShowDialog();
            }
        }

        private void ExecuteOpenBaseFileCommand()
        {
            try
            {
                string folderpath = GetFilePath((from x in Application.Current.Windows.OfType<Window>() where x.Title == "AutoEditView" select x).First());
                InitializebyFile(folderpath.Substring(0,folderpath.Length-1), NewItemsDG);
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
                string folderpath = GetFolderPath((from x in Application.Current.Windows.OfType<Window>() where x.Title == "AutoEditView" select x).First());
                InitializebyFolder(folderpath, NewItemsDG);
                IsSetComplete = true;
                CurrentBase = "현재 비교: 폴더";
                IsFolderMode = true;
                CurrentNewPath = folderpath;
                toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            catch(Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }
        }

        private void ExecutePreviewEditCommand()
        {
            if (toreplace != null)
            {
                if (LangMode != toreplace.langmode) toreplace = MakeModifyList(DataItems, NewItemsDG);
            }
            new PreviewView(toreplace.ToString()).ShowDialog() ;
        }

        private void ExecuteBeginAutoEditCommand()
        {
            toreplace = MakeModifyList(DataItems, NewItemsDG);
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
            NewItems = new ObservableCollection<DataItem>();
            NewItemsDG = new DataTable();
            CurrentBase = "현재 비교 대상 없음";
            toreplace = null;
            CurrentNewPath = "현재 선택된 대상 없음"; ;
        }

        #endregion
    }
}