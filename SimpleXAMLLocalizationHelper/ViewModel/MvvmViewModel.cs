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
using static SimpleXAMLLocalizationHelper.Functions.Utils;

namespace SimpleXAMLLocalizationHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MvvmViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MvvmViewModel class.
        /// </summary>

        #region properties and variables

        private string _Kor;

        public string Kor
        {
            get
            {
                return _Kor;
            }
            set
            {
                Set(nameof(Kor), ref _Kor, value);
            }
        }

        private string _Eng;

        public string Eng
        {
            get
            {
                return _Eng;
            }
            set
            {
                Set(nameof(Eng), ref _Eng, value);
            }
        }

        private string _Jpn;

        public string Jpn
        {
            get
            {
                return _Jpn;
            }
            set
            {
                Set(nameof(Jpn), ref _Jpn, value);
            }
        }

        private string _Chns;

        public string Chns
        {
            get
            {
                return _Chns;
            }
            set
            {
                Set(nameof(Chns), ref _Chns, value);
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

        private string _selectedID;

        public string SelectedID
        {
            get
            {
                return _selectedID;
            }
            set
            {
                Set(nameof(SelectedID), ref _selectedID, value);
            }
        }

        private DataItem _nowindex;

        public DataItem nowindex
        {
            get
            {
                return _nowindex;
            }
            set
            {
                Set(nameof(nowindex), ref _nowindex, value);
            }
        }

        private ObservableCollection<DataItem> _Items;

        public ObservableCollection<DataItem> Items

        {
            get
            {
                return _Items;
            }

            set
            {
                Set(nameof(Items), ref _Items, value);
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ClickCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion properties and variables

        #region constructor

        public MvvmViewModel()
        {
            Items = new ObservableCollection<DataItem>();
            AddCommand = new RelayCommand(() => ExecuteAddCommand());
            ModifyCommand = new RelayCommand(() => ExecuteModifyCommand());
            SearchCommand = new RelayCommand(() => ExecuteSearchCommand());
            ClickCommand = new RelayCommand(() => ExecuteClickCommand());
            DeleteCommand = new RelayCommand(() => ExecuteDeleteCommand());
            Initialize();
        }

        private void Initialize()
        {
            XDocument xDocKR;
            XDocument xDocEN;
            XDocument xDocJP;
            XDocument xDocCH_S;
            XElement[] xarraysKR;
            XElement[] xarraysEN;
            XElement[] xarraysJP;
            XElement[] xarraysCH_S;
            ArrayList nameKR = new ArrayList();
            ArrayList nameEN = new ArrayList();
            ArrayList nameJP = new ArrayList();
            ArrayList nameCH_S = new ArrayList();

            Items.Clear();

            try
            {
                using (var sr = new StreamReader(ResourcePath + Properties.Settings.Default.KorFileName + ".xaml"))
                {
                    xDocKR = XDocument.Load(sr);
                    var findKR = from w in xDocKR.Descendants()
                                 where w.Attribute(xmn + "Key") != null && w.HasElements != true
                                 select w;
                    xarraysKR = findKR.ToArray<XElement>();
                }

                using (var sr = new StreamReader(ResourcePath + Properties.Settings.Default.EngFileName + ".xaml"))
                {
                    xDocEN = XDocument.Load(sr);
                    var findEN = from w in xDocEN.Descendants()
                                 where w.Attribute(xmn + "Key") != null && w.HasElements != true
                                 select w;
                    xarraysEN = findEN.ToArray<XElement>();
                }

                using (var sr = new StreamReader(ResourcePath + Properties.Settings.Default.JpnFileName + ".xaml"))
                {
                    xDocJP = XDocument.Load(sr);
                    var findJP = from w in xDocJP.Descendants()
                                 where w.Attribute(xmn + "Key") != null && w.HasElements != true
                                 select w;

                    xarraysJP = findJP.ToArray<XElement>();
                }

                using (var sr = new StreamReader(ResourcePath + Properties.Settings.Default.ChnsFileName + ".xaml"))
                {
                    xDocCH_S = XDocument.Load(sr);
                    var findCH_S = from w in xDocCH_S.Descendants()
                                   where w.Attribute(xmn + "Key") != null && w.HasElements != true
                                   select w;
                    xarraysCH_S = findCH_S.ToArray<XElement>();
                }
            }
            catch
            {
                MessageBox.Show("파일을 올바른 위치에 넣어주세요!\n 폴더는 C:\\Languages 여야 하며, 폴더 안에 들어가는 파일 명은: \nKorean.xaml\nEnglish.xaml\nJapanese.xaml\nChinese.xaml\n이어야 합니다. 또한 각 요소는 모두 어트리뷰트 \"x:Key\"를 고유한 값으로 지니고 있어야 합니다.\n 네 파일 모두 반드시 필요하오니, 4가지 언어가 모두 필요없다 해도 이름이 같은 더미파일이라도 넣어주시기 바랍니다.");
                Application.Current.Shutdown();
                return;
            }

            foreach (var x in xarraysKR)
            {
                nameKR.Add(x.Attribute(xmn + "Key").Value);
            }
            foreach (var x in xarraysEN)
            {
                nameEN.Add(x.Attribute(xmn + "Key").Value);
            }
            foreach (var x in xarraysJP)
            {
                nameJP.Add(x.Attribute(xmn + "Key").Value);
            }
            foreach (var x in xarraysCH_S)
            {
                nameCH_S.Add(x.Attribute(xmn + "Key").Value);
            }

            for (int i = 0; i < (new int[] { xarraysEN.Length, xarraysJP.Length, xarraysKR.Length, xarraysCH_S.Length }.Max()); i++)
            {
                ArrayList sarray = new ArrayList();

                string justadded = "";

                foreach (var x in Items)
                {
                    sarray.Add(x.ID);
                }

                if (xarraysKR.Length > i)
                {
                    if (sarray.Contains(xarraysKR[i].Attribute(xmn + "Key").Value) == false)
                    {
                        DataItem dt = new DataItem();
                        dt.ID = xarraysKR[i].Attribute(xmn + "Key").Value;
                        dt.Kor = xarraysKR[i].Value;

                        Addifnotcontains(nameEN, xarraysEN, ref dt, 1);
                        Addifnotcontains(nameJP, xarraysJP, ref dt, 2);
                        Addifnotcontains(nameCH_S, xarraysCH_S, ref dt, 3);
                        Items.Add(dt);
                        justadded = dt.ID;
                    }
                }

                if (xarraysEN.Length > i)
                {
                    if (sarray.Contains(xarraysEN[i].Attribute(xmn + "Key").Value) == false && xarraysEN[i].Attribute(xmn + "Key").Value != justadded)
                    {
                        DataItem dt = new DataItem();
                        dt.ID = xarraysEN[i].Attribute(xmn + "Key").Value;
                        dt.Eng = xarraysEN[i].Value;
                        Addifnotcontains(nameKR, xarraysKR, ref dt, 0);
                        Addifnotcontains(nameJP, xarraysJP, ref dt, 2);
                        Addifnotcontains(nameCH_S, xarraysCH_S, ref dt, 3);
                        Items.Add(dt);
                        justadded = dt.ID;
                    }
                }

                if (xarraysJP.Length > i)
                {
                    if (sarray.Contains(xarraysJP[i].Attribute(xmn + "Key").Value) == false && xarraysJP[i].Attribute(xmn + "Key").Value != justadded)
                    {
                        DataItem dt = new DataItem();
                        dt.ID = xarraysJP[i].Attribute(xmn + "Key").Value;
                        dt.Jpn = xarraysJP[i].Value;
                        Addifnotcontains(nameKR, xarraysKR, ref dt, 0);
                        Addifnotcontains(nameEN, xarraysEN, ref dt, 1);
                        Addifnotcontains(nameCH_S, xarraysCH_S, ref dt, 3);
                        Items.Add(dt);
                        justadded = dt.ID;
                    }
                }

                if (xarraysCH_S.Length > i)
                {
                    if (sarray.Contains(xarraysCH_S[i].Attribute(xmn + "Key").Value) == false && xarraysCH_S[i].Attribute(xmn + "Key").Value != justadded)
                    {
                        DataItem dt = new DataItem();
                        dt.ID = xarraysCH_S[i].Attribute(xmn + "Key").Value;
                        dt.Chns = xarraysCH_S[i].Value;
                        Addifnotcontains(nameKR, xarraysKR, ref dt, 0);
                        Addifnotcontains(nameEN, xarraysEN, ref dt, 1);
                        Addifnotcontains(nameJP, xarraysJP, ref dt, 2);
                        Items.Add(dt);
                    }
                }
            }
            foreach (var item in Items)
            {
                if (item.Kor == null) item.Kor = "null";
                if (item.Eng == null) item.Eng = "null";
                if (item.Jpn == null) item.Jpn = "null";
                if (item.Chns == null) item.Chns = "null";
            }
        }

        #endregion constructor

        #region methods
        private static void SaveFiles(string kr, string en, string jp, string chns)
        {
            try
            {
                try
                {
                    using (StreamWriter wr = new StreamWriter(ResourcePath + Properties.Settings.Default.KorFileName + ".xaml"))
                    {
                        ReplaceCarriageReturns_String(ref kr);
                        wr.Write(kr);
                    }
                }
                catch
                {
                    MessageBox.Show("한글 파일의 저장에 실패하였습니다.");
                }
                try
                {
                    using (StreamWriter wr = new StreamWriter(ResourcePath + Properties.Settings.Default.EngFileName + ".xaml"))
                    {
                        ReplaceCarriageReturns_String(ref en);
                        wr.Write(en);
                    }
                }
                catch
                {
                    MessageBox.Show("영문 파일의 저장에 실패하였습니다.");
                }
                try
                {
                    using (StreamWriter wr = new StreamWriter(ResourcePath + Properties.Settings.Default.JpnFileName + ".xaml"))
                    {
                        ReplaceCarriageReturns_String(ref jp);
                        wr.Write(jp);
                    }
                }
                catch
                {
                    MessageBox.Show("일문 파일의 저장에 실패하였습니다.");
                }
                try
                {
                    using (StreamWriter wr = new StreamWriter(ResourcePath + Properties.Settings.Default.ChnsFileName + ".xaml"))
                    {
                        ReplaceCarriageReturns_String(ref chns);
                        wr.Write(chns);
                    }
                }
                catch
                {
                    MessageBox.Show("중문 파일의 저장에 실패하였습니다.");
                }
                MessageBox.Show("성공적으로 저장했습니다.");
            }
            catch
            {
                MessageBox.Show("저장 과정 도중 오류가 발생했습니다. 파일이 다른 프로그램에서 열려있지는 않은지 봐주십시오.");
            }
        }

        public static void SaveFiles(XDocument kr, XDocument en, XDocument jp, XDocument ch)
        {
            try
            {
                ReplaceCarriageReturns_XDoc(ref kr);
                ReplaceCarriageReturns_XDoc(ref en);
                ReplaceCarriageReturns_XDoc(ref jp);
                ReplaceCarriageReturns_XDoc(ref ch);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            SaveFiles(kr.ToString(), en.ToString(), jp.ToString(), ch.ToString());
        }
        #endregion

        #region command methods

        private void ExecuteAddCommand()
        {
            XDocument xDocKR = null;
            XDocument xDocEN = null;
            XDocument xDocJP = null;
            XDocument xDocCH_S = null;
            string message = "";
            if (ID != "" && ID != string.Empty && ID != null)
            {
                if (Kor != string.Empty)
                {
                    if (AddElementwithDefaultKey(Properties.Settings.Default.KorFileName, ID, Kor, ref xDocKR) == true) message += "한글, ";
                    else MessageBox.Show("같은 ID를 가진 리소스가 한글 파일에 존재합니다.");
                }

                if (Eng != string.Empty)
                {
                    if (AddElementwithDefaultKey(Properties.Settings.Default.EngFileName, ID, Eng, ref xDocEN) == true) message += "영문, ";
                    else MessageBox.Show("같은 ID를 가진 리소스가 영문 파일에 존재합니다.");
                }

                if (Jpn != string.Empty)
                {
                    if (AddElementwithDefaultKey(Properties.Settings.Default.JpnFileName, ID, Jpn, ref xDocJP) == true) message += "일문, ";
                    else MessageBox.Show("같은 ID를 가진 리소스가 일문 파일에 존재합니다.");
                }

                if (Chns != string.Empty)
                {
                    if (AddElementwithDefaultKey(Properties.Settings.Default.ChnsFileName, ID, Chns, ref xDocCH_S) == true) message += "중문, ";
                    else MessageBox.Show("같은 ID를 가진 리소스가 중문 파일에 존재합니다.");
                }

                if (message.Length > 3)
                {
                    SaveFiles(xDocKR, xDocEN, xDocJP, xDocCH_S);
                    MessageBox.Show(message.Substring(0, message.Length - 2) + " 파일에 저장했습니다.");
                    Initialize();
                }
            }
            else
            {
                MessageBox.Show("ID를 공백문자로 만들 수 없습니다.");
            }
        }

        private void ExecuteModifyCommand()
        {
            XDocument xDocKR = null;
            XDocument xDocEN = null;
            XDocument xDocJP = null;
            XDocument xDocCH_S = null;

            try
            {
                ModifyElementwithIDandValue(Properties.Settings.Default.KorFileName, ID, Kor, ref xDocKR);
                ModifyElementwithIDandValue(Properties.Settings.Default.EngFileName, ID, Eng, ref xDocEN);
                ModifyElementwithIDandValue(Properties.Settings.Default.JpnFileName, ID, Jpn, ref xDocJP);
                ModifyElementwithIDandValue(Properties.Settings.Default.ChnsFileName, ID, Chns, ref xDocCH_S);
                SaveFiles(xDocKR, xDocEN, xDocJP, xDocCH_S);
                Initialize();
            }
            catch
            {
                MessageBox.Show("ID 검색 과정에서 오류가 발생했습니다. xaml 파일들을 다시 한번 점검해주십시오.");
            }
        }

        private void ExecuteDeleteCommand()
        {
            XDocument xDocKR = null;
            XDocument xDocEN = null;
            XDocument xDocJP = null;
            XDocument xDocCH_S = null;
            string nowid = nowindex.ID;

            try
            {
                if (DeleteElementbyID(Properties.Settings.Default.KorFileName, nowid, ref xDocKR) == false) MessageBox.Show("해당 ID는 한글 파일에는 없습니다.");
                if (DeleteElementbyID(Properties.Settings.Default.EngFileName, nowid, ref xDocEN) == false) MessageBox.Show("해당 ID는 영문 파일에는 없습니다.");
                if (DeleteElementbyID(Properties.Settings.Default.JpnFileName, nowid, ref xDocJP) == false) MessageBox.Show("해당 ID는 일문 파일에는 없습니다.");
                if (DeleteElementbyID(Properties.Settings.Default.ChnsFileName, nowid, ref xDocCH_S) == false) MessageBox.Show("해당 ID는 중문 파일에는 없습니다.");
                SaveFiles(xDocKR, xDocEN, xDocJP, xDocCH_S);
                Initialize();
            }
            catch
            {
                MessageBox.Show("ID 검색 과정에서 오류가 발생했습니다. xaml 파일들을 다시 한번 점검해주십시오.");
            }
        }

        private void ExecuteClickCommand()
        {
            ID = nowindex.ID;
            Kor = nowindex.Kor;
            Eng = nowindex.Eng;
            Jpn = nowindex.Jpn;
            Chns = nowindex.Chns;
        }

        private void ExecuteSearchCommand()
        {
            Kor = GetValuefromID(Properties.Settings.Default.KorFileName, ID);
            Eng = GetValuefromID(Properties.Settings.Default.EngFileName, ID);
            Jpn = GetValuefromID(Properties.Settings.Default.JpnFileName, ID);
            Chns = GetValuefromID(Properties.Settings.Default.ChnsFileName, ID);
        }

        #endregion command methods
    }
}