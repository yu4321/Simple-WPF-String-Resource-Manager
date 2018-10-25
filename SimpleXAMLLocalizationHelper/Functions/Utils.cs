using SimpleXAMLLocalizationHelper.Model;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SimpleXAMLLocalizationHelper.Functions
{
    public static class Utils
    {
        public static readonly XNamespace xmn = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml");
        public static readonly XNamespace smn = XNamespace.Get("clr-namespace:System;assembly=mscorlib");
        public static readonly string ResourcePath = @"C:\Languages\";

        #region methods

        public static void Addifnotcontains(ArrayList name, XElement[] xarray, ref DataItem dt, int langcase)
        {
            if (name.Contains(dt.ID))
            {
                string dtid = dt.ID;
                var query = from w in xarray where w.Attribute(xmn + "Key").Value == dtid select w;
                try
                {
                    if (langcase == 0) dt.Kor = query.Single<XElement>().Value;
                    if (langcase == 1) dt.Eng = query.Single<XElement>().Value;
                    if (langcase == 2) dt.Jpn = query.Single<XElement>().Value;
                    if (langcase == 3) dt.Chns = query.Single<XElement>().Value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

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

        public static void ReplaceCarriageReturns_String(ref string s)
        {
            s = s.Replace("&amp;#xA;", "&#xA;");
            s = s.Replace("\" xmlns", "\"\r\n\t\t\t\t\txmlns");
        }

        public static bool AddElementwithDefaultKey(string location, string newID, string newvalue, ref XDocument xd)
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
                    xle.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
                    if (addplace == (XElement)xd.FirstNode) addplace.Add(xle);
                    else addplace.AddAfterSelf(xle);
                    result = true;
                }
            }
            return result;
        }

        public static bool ModifyElementwithIDandValue(string location, string selectedID, string newvalue, ref XDocument xd)
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

        public static bool DeleteElementbyID(string location, string nowid, ref XDocument xd)
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

        public static string GetValuefromID(string location, string nowid)
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

        #endregion methods
    }
}