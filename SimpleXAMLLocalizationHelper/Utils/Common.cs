using Microsoft.WindowsAPICodePack.Dialogs;
using SimpleXAMLLocalizationHelper.CustomDialogs;
using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace SimpleXAMLLocalizationHelper.Utils
{
    public static class Common
    {
        public static readonly XNamespace xmn = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml");
        public static readonly XNamespace smn = XNamespace.Get("clr-namespace:System;assembly=mscorlib");
        //public static string ResourcePath = @"C:\Languages\";

        #region methods

        public static void ReplaceCarriageReturns_XDoc(ref XDocument xd)
        {
            try
            {
                if (xd.DescendantNodes() != null)
                    foreach (var v in xd.DescendantNodes())
                    {
                        XElement x = null;
                        if (v is XElement) x = (XElement)v;
                        else continue;
                        if (x.HasElements) continue;
                        x.Value = x.Value.Replace("\n", "&#xA;");
                    }
            }
            catch
            {
                throw new Exception();
            }
        }

        public static XDocument ReplaceCarriageReturns_XDoc(XDocument xd)
        {
            try
            {
                if (xd.DescendantNodes() != null)
                {
                    foreach (var v in xd.DescendantNodes())
                    {
                        XElement x = null;
                        if (v is XElement) x = (XElement)v;
                        else continue;
                        if (x.HasElements) continue;
                        x.Value = x.Value.Replace("\n", "&#xA;");
                    }
                }
                return xd;
            }
            catch
            {
                throw new Exception();
            }
        }

        public static void ReplaceCarriageReturns_String(ref string s)
        {
            s = s.Replace("&amp;#xA;", "&#xA;");
            s = s.Replace("\" xmlns", "\"\r\n\t\t\t\t\txmlns");
        }

        public static string ReplaceCarriageReturns_String(string s)
        {
            s = s.Replace("&amp;#xA;", "&#xA;");
            s = s.Replace("\" xmlns", "\"\r\n\t\t\t\t\txmlns");
            return s;
        }

        public static bool AddElementwithDefaultKey(string ResourcePath, string location, string newID, string newvalue, ref XDocument xd)
        {
            bool result = false;
            XElement addplace;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.HasElements != true
                           select w;
                try
                {
                    addplace = find.Last<XElement>();
                }
                catch
                {
                    addplace = (XElement)xd.FirstNode;
                }

                var findsame = from x in find
                               where x.Attribute(xmn + "Key").Value == newID
                               select x;

                if (findsame.Count<XElement>() > 0)
                {
                    result = false;
                }
                else
                {
                    XElement xle = new XElement(smn + "String", newvalue.Replace("\r\n", "&#xA;").Replace("&amp;#xA;", "&#xA;"));
                    xle.Add(new XAttribute(xmn + "Key", newID));
                    if (newvalue.Contains(" "))
                    {
                        xle.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
                    }
                    if (addplace == (XElement)xd.FirstNode) addplace.Add(xle);
                    else addplace.AddAfterSelf(xle);
                    result = true;
                }
            }
            return result;
        }

        public static bool AddElementwithDefaultKey(string ResourcePath, string location, string newID, string newvalue, ref XDocument xd, List<XAttribute> attributes=null)
        {
            bool result = false;
            XElement addplace;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.HasElements != true
                           select w;
                try
                {
                    addplace = find.Last<XElement>();
                }
                catch
                {
                    addplace = (XElement)xd.FirstNode;
                }

                var findsame = from x in find
                               where x.Attribute(xmn + "Key").Value == newID
                               select x;

                if (findsame.Count<XElement>() > 0)
                {
                    result = false;
                }
                else
                {
                    XElement xle = new XElement(smn + "String", newvalue.Replace("\r\n", "&#xA;").Replace("&amp;#xA;", "&#xA;"));
                    xle.Add(new XAttribute(xmn + "Key", newID));
                    if (attributes != null)
                    {
                        foreach (XAttribute attr in attributes) xle.Add(attr);
                    }
                    //xle.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
                    if (addplace == (XElement)xd.FirstNode) addplace.Add(xle);
                    else addplace.AddAfterSelf(xle);
                    result = true;
                }
            }
            return result;
        }

        public static bool ModifyElementwithIDandValue(string ResourcePath, string location, string selectedID, string newvalue, ref XDocument xd)
        {
            bool result = false;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.Attribute(xmn + "Key").Value == selectedID
                           select w;
                foreach (var x in find)
                {
                    x.Value = newvalue.Replace("\r\n", "&#xA;"); ;
                }
                result = true;
            }
            return result;
        }

        public static bool ModifyElementwithIDandValueFullPath(string location, string selectedID, string newvalue, ref XDocument xd)
        {
            bool result = false;
            using (var sr = new StreamReader(location))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.Attribute(xmn + "Key").Value == selectedID
                           select w;
                foreach (var x in find)
                {
                    x.Value = newvalue.Replace("\r\n", "&#xA;"); ;
                }
                result = true;
            }
            return result;
        }

        public static bool ModifyElementwithIDandValueFolderPath(string path, string location, string selectedID, string newvalue, ref XDocument xd)
        {
            bool result = false;
            using (var sr = new StreamReader(path + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.Attribute(xmn + "Key").Value == selectedID
                           select w;
                foreach (var x in find)
                {
                    x.Value = newvalue.Replace("\r\n", "&#xA;"); ;
                }
                result = true;
            }
            return result;
        }

        public static bool DeleteElementbyID(string ResourcePath, string location, string nowid, ref XDocument xd)
        {
            bool result = false;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                try
                {
                    var find = from w in xd.Descendants()
                               where w.Attribute(xmn + "Key") != null && w.HasElements != true && w.Attribute(xmn + "Key").Value == nowid
                               select w;
                    find.Single<XElement>().Remove();
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        public static List<XAttribute> GetElementAttributesbyID(string ResourcePath, string location, string nowid)
        {
            XDocument xd;
            List<XAttribute> result = null;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                try
                {
                    var find = from w in xd.Descendants()
                               where w.Attribute(xmn + "Key") != null && w.HasElements != true && w.Attribute(xmn + "Key").Value == nowid
                               select w;
                    result = find.Single<XElement>().Attributes().ToList();
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }

        public static bool DeleteAttributefromElementbyID(string ResourcePath, string location, string selectedID, XAttribute newAttribute, ref XDocument xd)
        {
            bool result = false;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.Attribute(xmn + "Key").Value == selectedID
                           select w;
                foreach (var x in find)
                {
                    var attr = from w in x.Attributes(newAttribute.Name) where w.Value == newAttribute.Value select w;
                    attr.Remove();
                }
                result = true;
            }
            return result;
        }

        public static bool AddAttributefromElementbyID(string ResourcePath, string location, string selectedID, XAttribute newAttribute, ref XDocument xd)
        {
            bool result = false;
            using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
            {
                xd = XDocument.Load(sr);
                var find = from w in xd.Descendants()
                           where w.Attribute(xmn + "Key") != null && w.Attribute(xmn + "Key").Value == selectedID
                           select w;
                foreach (var x in find)
                {
                    x.SetAttributeValue(newAttribute.Name, newAttribute.Value);
                }
                result = true;
            }
            return result;
        }

        public static string GetValuefromID(string ResourcePath, string location, string nowid)
        {
            XDocument xd;
            string result = "";
            try
            {
                using (var sr = new StreamReader(ResourcePath + location + ".xaml"))
                {
                    xd = XDocument.Load(sr);
                    var find = from w in xd.Descendants()
                               where w.Attribute(xmn + "Key") != null && w.HasElements != true && w.Attribute(xmn + "Key").Value == nowid
                               select w;
                    result = find.Single<XElement>().Value;
                }
            }
            catch
            {
                result = "결과를 찾지 못하였습니다.";
            }
            return result;
        }

        public static string GetFolderPath(bool isfirst = false)
        {
            string sresult;
            using (var dialog = new MyFolderSelectDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "언어 리소스 파일들이 들어있는 폴더를 선택해주세요.";
                if (isfirst) dialog.Title += "설정에 있는 파일명이 모두 필요합니다.";
                dialog.ShowDialog();
                try
                {
                    sresult = dialog.FileName;
                }
                catch
                {
                    sresult = "";
                }
            }
            sresult += "\\";
            return sresult;
        }

        public static string GetFolderPath(Window basewindow)
        {
            string sresult;
            using (var dialog = new MyFolderSelectDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "언어 리소스 파일들이 들어있는 폴더를 선택해주세요.";
                dialog.ShowDialog(basewindow);
                try
                {
                    sresult = dialog.FileName;
                }
                catch
                {
                    sresult = "";
                }
            }
            sresult += "\\";
            return sresult;
        }

        public static string GetFilePath()
        {
            string sresult;
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Title = "언어 리소스 파일을 선택해주세요.";
                dialog.Filters.Add(new CommonFileDialogFilter("언어 리소스 파일", ".xaml"));
                CommonFileDialogResult result = dialog.ShowDialog();
                try
                {
                    sresult = dialog.FileName;
                    //lastSelectedPath = sresult;
                }
                catch
                {
                    sresult = "";
                }
            }
            sresult += "\\";
            return sresult;
        }

        public static string GetFilePath(Window basewindow)
        {
            string sresult;
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Title = "언어 리소스 파일을 선택해주세요.";
                dialog.Filters.Add(new CommonFileDialogFilter("언어 리소스 파일", ".xaml"));
                CommonFileDialogResult result = dialog.ShowDialog(basewindow);
                try
                {
                    sresult = dialog.FileName;
                }
                catch
                {
                    sresult = "";
                }
            }
            sresult += "\\";
            return sresult;
        }

        private static void SaveFiles(string ResourcePath, Dictionary<string, string> strdic)
        {
            try
            {
                foreach (var x in strdic)
                {
                    try
                    {
                        using (StreamWriter wr = new StreamWriter(ResourcePath + x.Key + ".xaml"))
                        {
                            wr.Write(ReplaceCarriageReturns_String(x.Value));
                        }
                    }
                    catch
                    {
                        MessageBox.Show(x.Key + ".xaml 파일의 저장에 실패하였습니다.");
                    }
                }
            }
            catch
            {
                MessageBox.Show("저장 과정 도중 오류가 발생했습니다. 파일이 다른 프로그램에서 열려있지는 않은지 봐주십시오.");
            }
        }

        public static void SaveFiles(string ResourcePath, Dictionary<string, XDocument> xDocs)
        {
            //List<string> param = new List<string>();
            Dictionary<string, string> param = new Dictionary<string, string>();
            try
            {
                foreach (string x in App.LanguageList)
                {
                    xDocs[x] = ReplaceCarriageReturns_XDoc(xDocs[x]);
                    param.Add(x, xDocs[x].ToString());
                }
            }
            catch (Exception e)
            {
                App.LoggerEx.Warn(e.Message);
            }
            SaveFiles(ResourcePath, param);
        }

        #endregion methods
    }
}