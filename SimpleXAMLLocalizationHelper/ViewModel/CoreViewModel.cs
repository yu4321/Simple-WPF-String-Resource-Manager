using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleXAMLLocalizationHelper.CustomDialogs;
using SimpleXAMLLocalizationHelper.Messages;
using SimpleXAMLLocalizationHelper.Model;
using SimpleXAMLLocalizationHelper.Utils;
using SimpleXAMLLocalizationHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace SimpleXAMLLocalizationHelper.ViewModel
{
    /// <summary>
    /// 주 뷰모델 - 메인 윈도우 관련 요소들
    /// </summary>
    public partial class CoreViewModel : ViewModelBase, IDisposable
    {
        #region properties and variables

        private ObservableCollection<LanguageBoxItem> _inputBoxes = new ObservableCollection<LanguageBoxItem>();

        public ObservableCollection<LanguageBoxItem> InputBoxes
        {
            get
            {
                return _inputBoxes;
            }
            set
            {
                Set(nameof(InputBoxes), ref _inputBoxes, value);
            }
        }

        private string _ID;

        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                Set(nameof(ID), ref _ID, value);
            }
        }

        private DataRowView _currentDataGridRow;

        public DataRowView CurrentDataGridRow
        {
            get
            {
                return _currentDataGridRow;
            }
            set
            {
                Set(nameof(CurrentDataGridRow), ref _currentDataGridRow, value);
            }
        }

        private AttributeItem _currentAttributeUpperSelected;

        public AttributeItem CurrentAttributeUpperSelected
        {
            get
            {
                return _currentAttributeUpperSelected;
            }
            set
            {
                Set(nameof(CurrentAttributeUpperSelected), ref _currentAttributeUpperSelected, value);
            }
        }

        private AttributeItem _currentAttributeUtterSelected;

        public AttributeItem CurrentAttributeUtterSelected
        {
            get
            {
                return _currentAttributeUtterSelected;
            }
            set
            {
                Set(nameof(CurrentAttributeUtterSelected), ref _currentAttributeUtterSelected, value);
            }
        }

        private ObservableCollection<AttributeItem> _attributeCollectionUpper;

        public ObservableCollection<AttributeItem> AttributeCollectionUpper
        {
            get
            {
                return _attributeCollectionUpper;
            }

            set
            {
                Set(nameof(AttributeCollectionUpper), ref _attributeCollectionUpper, value);
            }
        }

        private ObservableCollection<AttributeItem> _attributeCollectionUtter;

        public ObservableCollection<AttributeItem> AttributeCollectionUtter
        {
            get
            {
                return _attributeCollectionUtter;
            }

            set
            {
                Set(nameof(AttributeCollectionUtter), ref _attributeCollectionUtter, value);
            }
        }

        private bool _isAttrsUsing;

        public bool IsAttrsUsing
        {
            get
            {
                return _isAttrsUsing;
            }

            set
            {
                Set(nameof(IsAttrsUsing), ref _isAttrsUsing, value);
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

        private DataTable _mainDataTable = new DataTable();

        public DataTable MainDataTable
        {
            get
            {
                return _mainDataTable;
            }
            set
            {
                Set(nameof(MainDataTable), ref _mainDataTable, value);
                Global.CurrentSelectedItems = _mainDataTable;
            }
        }

        private string lastSelectedPath;
        public string ResourcePath { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ClickCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ICommand AddCommandA1 { get; set; }
        public ICommand DeleteCommandA1 { get; set; }

        public ICommand AddFavoriteCommand { get; set; }

        public ICommand HomeCommand { get; set; }

        public ICommand OpenAutoEditCommand { get; set; }
        public ICommand OpenFolderSelectCommand { get; set; }

        #endregion properties and variables

        #region constructor

        public CoreViewModel()
        {
            AttributeCollectionUpper = new ObservableCollection<AttributeItem>();
            AttributeCollectionUtter = new ObservableCollection<AttributeItem>();
            AttributeCollectionUtter.Add(new AttributeItem()
            {
                Content = new XAttribute(XNamespace.Xml + "space", "preserve")
            });
            AttributeCollectionUtter.Add(new AttributeItem()
            {
                Content = new XAttribute(XNamespace.Xml + "space", "default")
            });
            AddCommand = new RelayCommand(() => ExecuteAddCommand());
            ModifyCommand = new RelayCommand(() => ExecuteModifyCommand());
            SearchCommand = new RelayCommand(() => ExecuteSearchCommand());
            ClickCommand = new RelayCommand(() => ExecuteClickCommand());
            DeleteCommand = new RelayCommand(() => ExecuteDeleteCommand());
            AddCommandA1 = new RelayCommand(() => ExecuteAddCommandA1());
            DeleteCommandA1 = new RelayCommand(() => ExecuteDeleteCommandA1());
            OpenAutoEditCommand = new RelayCommand(() => ExecuteOpenAutoEditCommand());
            OpenFolderSelectCommand = new RelayCommand(() =>
              {
                  SetFolderPath();
                  MakeTables(true);
                  //Messenger.Default.Send<ResetMessage>(new ResetMessage(true));
              });
            HomeCommand = new RelayCommand(() => Messenger.Default.Send<GotoPageMessage>(new GotoPageMessage(PageName.Start)));
            AddFavoriteCommand = new RelayCommand(() => ExecuteAddFavoriteCommand());
            Messenger.Default.Register<ResetMessage>(this, (x) => MakeTables(x.isSoftReset));
            if (!IsInDesignMode)
            {
                SetFolderPath(true);
                MakeTables(true); // 최초 테이블 생성
            }
        }

        #endregion constructor

        #region methods

        private void MakeInputBoxes()
        {
            InputBoxes.Clear();
            foreach (var x in App.LanguageList)
            {
                InputBoxes.Add(new LanguageBoxItem
                {
                    Language = x,
                    Content = ""
                });
            }
        }

        private void MakeTables(bool isSoftReset = false)
        {
            List<string> filenames = App.LanguageList;
            Dictionary<string, XElement[]> elements = new Dictionary<string, XElement[]>();
            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();
            string lastfind;
            try
            {
                foreach (string name in filenames)
                {
                    lastfind = name;
                    using (var sr = new StreamReader(ResourcePath + name + ".xaml"))
                    {
                        XDocument xDoc = XDocument.Load(sr);
                        XElement[] xArray = (from w in xDoc.Descendants()
                                             where w.Attribute(Common.xmn + "Key") != null && w.HasElements != true
                                             select w).ToArray();
                        xDocs.Add(name, xDoc);
                        elements.Add(name, xArray);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("파일을 찾을 수 없습니다! ");
                return;
            }
            catch
            {
                MessageBox.Show("올바른 폴더를 선택해주세요!\n폴더 안에 들어가는 각 파일 명은 설정과 동일해야 합니다. 또한 각 요소는 모두 어트리뷰트 \"x:Key\"를 고유한 값으로 지니고 있어야 합니다.");
                return;
            }

            if (isSoftReset)
            {
                ResetParameters();
            }

            if (MainDataTable.Columns.Count <= 0)
            {
                MainDataTable.Columns.Add("ID");
                MainDataTable.PrimaryKey = new DataColumn[] { MainDataTable.Columns["ID"] };
            }
            else
            {
                MainDataTable.Rows.Clear();
            }

            foreach (var name in filenames)
            {
                if (MainDataTable.Columns[name] == null) MainDataTable.Columns.Add(name);
                foreach (var x in elements[name])
                {
                    var selectresult = MainDataTable.Select(string.Format("ID = '{0}'", x.Attribute(Common.xmn + "Key").Value));
                    if (selectresult.Count() <= 0)
                    {
                        DataRow dr = MainDataTable.NewRow();
                        dr["ID"] = x.Attribute(Common.xmn + "Key").Value;
                        dr[name] = x.Value;
                        MainDataTable.Rows.Add(dr);
                    }
                    else
                    {
                        MainDataTable.Rows.Find((string)x.Attribute(Common.xmn + "Key").Value)[name] = x.Value;
                    }
                }
            }
            Global.CurrentSelectedItems = MainDataTable;
            if (InputBoxes.Count <= 0)
            {
                MakeInputBoxes();
            }
        }

        private void ResetParameters()
        {
            AttributeCollectionUpper.Clear();
            CurrentDataGridRow = null;
            CurrentAttributeUpperSelected = null;
            CurrentAttributeUtterSelected = null;
            ID = null;
            CurrentFolderPath = lastSelectedPath;
            InputBoxes.Clear();
        }

        private void SetFolderPath(bool isfirst = false)
        {
            if (!IsInDesignMode)
            {
                ResourcePath = Utils.Common.GetFolderPath(isfirst);
                lastSelectedPath = ResourcePath;
            }
        }

        private void GetAttributesFromID(string ID)
        {
            AttributeCollectionUpper.Clear();
            CurrentAttributeUpperSelected = null;
            CurrentAttributeUtterSelected = null;
            try
            {
                List<XAttribute> allattributes = Common.GetElementAttributesbyID(ResourcePath, Properties.Settings.Default.KorFileName, ID);
                foreach (var x in allattributes)
                {
                    AttributeCollectionUpper.Add(new AttributeItem()
                    {
                        Content = x
                    });
                }
            }
            catch
            {
            }
        }

        #endregion methods

        #region command methods

        private void ExecuteAddCommand()
        {
            Dictionary<string, XElement[]> elements = new Dictionary<string, XElement[]>();
            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();
            string message = "";
            string message2 = "";
            if (ID != "" && ID != string.Empty && ID != null)
            {
                for (int i = 0; i < App.LanguageList.Count; i++)
                {
                    if (InputBoxes[i].Content != string.Empty && InputBoxes[i].Content != null)
                    {
                        XDocument xDoc = null;
                        if (Common.AddElementwithDefaultKey(ResourcePath, App.LanguageList[i], ID, InputBoxes[i].Content, ref xDoc) == true)
                        {
                            message += App.LanguageList[i] + ", ";
                            xDocs.Add(App.LanguageList[i], xDoc);
                        }
                        else MessageBox.Show("같은 ID를 가진 리소스가 " + App.LanguageList[i] + ".xaml 파일에 존재합니다.");
                    }
                }
                if (message.Length > 3)
                {
                    string finalmessage = message.Substring(0, message.Length - 2) + " 파일에 저장했습니다.";
                    if (message2.Length > 3) finalmessage += "\n\n" + message2.Substring(0, message2.Length - 2) + "파일은 공백값으로 처리했습니다.";
                    Common.SaveFiles(ResourcePath, xDocs);
                    MessageBox.Show(finalmessage);
                    Messenger.Default.Send<ResetMessage>(new ResetMessage());
                }
                else
                {
                    MessageBox.Show("최소 하나의 언어에는 값을 입력해주시기 바랍니다.");
                }
            }
            else
            {
                MessageBox.Show("ID를 공백문자로 만들 수 없습니다.");
            }
        }

        private void ExecuteModifyCommand()
        {
            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();

            try
            {
                for (int i = 0; i < App.LanguageList.Count; i++)
                {
                    XDocument xDoc = null;
                    Common.ModifyElementwithIDandValue(ResourcePath, App.LanguageList[i], ID, InputBoxes[i].Content, ref xDoc);
                    xDocs.Add(App.LanguageList[i], xDoc);
                }
                Common.SaveFiles(ResourcePath, xDocs);
                MessageBox.Show("성공적으로 요소들을 수정하였습니다.");
                Messenger.Default.Send<ResetMessage>(new ResetMessage());
            }
            catch
            {
                MessageBox.Show("ID 검색 과정에서 오류가 발생했습니다. xaml 파일들을 다시 한번 점검해주십시오.");
            }
        }

        private void ExecuteDeleteCommand()
        {
            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();

            int successes = 0;
            string nowid = "";

            if (CurrentDataGridRow != null)
            {
                nowid = (string)CurrentDataGridRow["ID"];
            }
            else if (ID != null && ID != string.Empty && ID != "" && ID.Length != 0)
            {
                nowid = ID;
            }
            else
            {
                MessageBox.Show("삭제할 요소를 선택하거나 ID를 입력해주십시오.");
                return;
            }
            try
            {
                foreach (string lang in App.LanguageList)
                {
                    XDocument xDoc = null;
                    if (Common.DeleteElementbyID(ResourcePath, lang, nowid, ref xDoc) == false)
                    {
                        MessageBox.Show(string.Format("{0}.xaml 파일에서 해당 키 값을 찾지 못했습니다.", lang));
                    }
                    else
                    {
                        xDocs.Add(lang, xDoc);
                        successes++;
                    }
                }

                if (successes > 1)
                {
                    Common.SaveFiles(ResourcePath, xDocs);
                    MessageBox.Show("해당 ID(" + nowid + ") 를 성공적으로 삭제하였습니다.");
                    Messenger.Default.Send<ResetMessage>(new ResetMessage());
                }
                else
                {
                    return;
                }
            }
            catch
            {
                MessageBox.Show("ID 검색 과정에서 오류가 발생했습니다. xaml 파일들을 다시 한번 점검해주십시오.");
            }
        }

        private void ExecuteClickCommand()
        {
            ID = (string)CurrentDataGridRow["ID"];
            for (int i = 0; i < App.LanguageList.Count; i++)
            {
                try
                {
                    InputBoxes[i].Content = (string)CurrentDataGridRow[App.LanguageList[i]];
                }
                catch
                {
                    InputBoxes[i].Content = "NULL";
                }
            }
            if (IsAttrsUsing) GetAttributesFromID((string)CurrentDataGridRow["ID"]);
        }

        private void ExecuteSearchCommand()
        {
            for (int i = 0; i < App.LanguageList.Count; i++)
            {
                InputBoxes[i].Content = Common.GetValuefromID(ResourcePath, App.LanguageList[i], ID);
            }
            try
            {
                CurrentDataGridRow = MainDataTable.DefaultView.FindRows(ID).Single();
            }
            catch
            {
                CurrentDataGridRow = null;
            }
        }

        private void ExecuteAddCommandA1()
        {
            if (CurrentAttributeUtterSelected == null)
            {
                MessageBox.Show("삽입할 어트리뷰트를 선택해주세요!");
                return;
            }

            if ((from w in AttributeCollectionUpper where w.Content.Name == CurrentAttributeUtterSelected.Content.Name select w).Count() > 0)
            {
                MessageBox.Show("동일한 어트리뷰트의 중복 삽입은 불가능합니다!");
                return;
            }

            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();
            XAttribute target = CurrentAttributeUtterSelected.Content;
            try
            {
                foreach (var name in App.LanguageList)
                {
                    XDocument xDoc = null;
                    Common.AddAttributefromElementbyID(ResourcePath, name, ID, target, ref xDoc);
                    xDocs.Add(name, xDoc);
                }
                Common.SaveFiles(ResourcePath, xDocs);
                MessageBox.Show("성공적으로 요소들을 수정하였습니다.");
                Messenger.Default.Send<ResetMessage>(new ResetMessage());
                GetAttributesFromID(ID);
            }
            catch
            {
                MessageBox.Show("ID 검색 과정에서 오류가 발생했습니다. xaml 파일들을 다시 한번 점검해주십시오.");
            }
        }

        private void ExecuteDeleteCommandA1()
        {
            if (CurrentAttributeUpperSelected == null)
            {
                MessageBox.Show("삭제할 어트리뷰트를 선택해주세요!");
                return;
            }

            if (CurrentAttributeUpperSelected.Content.Name.LocalName.Contains("Key"))
            {
                MessageBox.Show("키 값의 삭제는 불가능합니다!");
                return;
            }

            Dictionary<string, XDocument> xDocs = new Dictionary<string, XDocument>();
            XAttribute target = CurrentAttributeUpperSelected.Content;
            try
            {
                foreach (var name in App.LanguageList)
                {
                    XDocument xDoc = null;
                    Common.DeleteAttributefromElementbyID(ResourcePath, name, ID, target, ref xDoc);
                    xDocs.Add(name, xDoc);
                }
                Common.SaveFiles(ResourcePath, xDocs);
                MessageBox.Show("성공적으로 요소들을 수정하였습니다.");
                Messenger.Default.Send<ResetMessage>(new ResetMessage());
                GetAttributesFromID(ID);
            }
            catch
            {
                MessageBox.Show("ID 검색 과정에서 오류가 발생했습니다. xaml 파일들을 다시 한번 점검해주십시오.");
            }
        }

        private void ExecuteOpenAutoEditCommand()
        {
            Global.CurrentSelectedPath = CurrentFolderPath;
            Global.CurrentSelectedItems = MainDataTable;
            //WorkerViewModel = null;
            AutoEditView dialog = new AutoEditView();
            //dialog.DataContext = new AutoEditViewModel();
            dialog.ShowDialog();
        }

        private void ExecuteAddFavoriteCommand()
        {
            var dialog = new AddFavoriteDialog(CurrentFolderPath);
            dialog.ShowDialog();
            bool result = dialog.result;
            if (result)
            {
                MessageBox.Show("즐겨찾기 등록을 성공했습니다.");
            }
            else
            {
                MessageBox.Show("즐겨찾기 등록을 실패했습니다.");
            }
        }

        #endregion command methods

        #region IDisposable Support

        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mainDataTable.Clear();
                    _mainDataTable.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}