using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleXAMLLocalizationHelper.Model
{
    public class ReplaceItem : ObservableObject, IDisposable
    {
        public List<DataItem> original;
        public DataTable originalTable;
        public List<DataItem> replace;
        public DataTable replaceTable;
        public bool isfoldermode;
        public string langmode;
        public string orgpath;
        public string path;

        public ReplaceItem()
        {
            original = new List<DataItem>();
            originalTable = new DataTable();
            originalTable.Columns.Add("ID");
            originalTable.PrimaryKey = new DataColumn[] { originalTable.Columns["ID"] };
            replace = new List<DataItem>();
            replaceTable = new DataTable();
            replaceTable.Columns.Add("ID");
            replaceTable.PrimaryKey = new DataColumn[] { replaceTable.Columns["ID"] };
            isfoldermode = false;
        }

        public override string ToString()
        {
            if (originalTable.Rows.Count > 0 && replaceTable.Rows.Count > 0)
            {
                string result = "";

                if (isfoldermode)
                {
                    for (int i = 0; i < originalTable.Rows.Count; i++)
                    {
                        var curorgitem = originalTable.Rows[i];
                        var currepitem = replaceTable.Rows[i];
                        result += string.Format("[{0}](", curorgitem[0]);
                        for(int j = 1; j < originalTable.Columns.Count; j++)
                        {
                            result += curorgitem[j];
                            if (j < originalTable.Columns.Count-1) result += " , ";
                        }
                        result += string.Format(") to [{0}](", currepitem[0]);
                        for (int j = 1; j < replaceTable.Columns.Count; j++)
                        {
                            result += currepitem[j];
                            if (j < replaceTable.Columns.Count-1) result += " , ";
                        }
                        result += ")\n\n";
                    }
                }
                else
                {
                    for (int i = 0; i < originalTable.Rows.Count; i++)
                    {
                        var curorgitem = originalTable.Rows[i];
                        var currepitem = replaceTable.Rows[i];
                        string orgrep = (string)curorgitem[langmode];
                        string reprep = (string)currepitem["NewItem"];

                        if (orgrep.Length > 0 && reprep.Length > 0)
                        {
                            result += string.Format("In [{0}](", curorgitem[0]);
                            for(int j = 1; j < originalTable.Columns.Count; j++)
                            {
                                result += curorgitem[j];
                                if (j < originalTable.Columns.Count-1) result += " , ";
                            }
                            result += string.Format("), Change {0} to {1}\n\n", orgrep, reprep);
                        }
                    }
                }
                if (result.Length > 0) return result;
                else return "수정 사항이 없습니다.";
            }
            else if (original.Count>0 && replace.Count > 0)
            {
                string result="";

                if (isfoldermode)
                {
                    for (int i = 0; i < original.Count; i++)
                    {
                        var curorgitem = original[i];
                        var currepitem = replace[i];
                        result += string.Format("{0}({1}, {2}, {3}, {4}) to {5}({6}, {7}, {8}, {9}))\n",curorgitem.ID, curorgitem.Kor, curorgitem.Eng, curorgitem.Jpn, curorgitem.Chns, currepitem.ID, currepitem.Kor, currepitem.Eng, currepitem.Jpn, currepitem.Chns);
                    }
                }
                else
                {
                    for (int i = 0; i < original.Count; i++)
                    {
                        var curorgitem = original[i];
                        var currepitem = replace[i];
                        string orgrep="";
                        string reprep = currepitem.Kor;
                        switch (langmode)
                        {
                            case "Korean":
                                orgrep = curorgitem.Kor;
                                break;
                            case "English":
                                orgrep = curorgitem.Eng;
                                break;
                            case "Japanese":
                                orgrep = curorgitem.Jpn;
                                break;
                            case "Chinese":
                                orgrep = curorgitem.Chns;
                                break;
                        }

                        if (orgrep.Length > 0 && reprep.Length > 0)
                        {
                            result += string.Format("in {0}({1}, {2}, {3}, {4}), {5} to {6}\n", curorgitem.ID, curorgitem.Kor, curorgitem.Eng, curorgitem.Jpn, curorgitem.Chns, orgrep, reprep);
                        }
                    }

                }
                if (result.Length > 0) return result;
                else return "수정 사항이 없습니다.";
            }
            else
            {
                return "수정 사항이 없습니다.";
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    originalTable.Clear();
                    originalTable.Dispose();
                    replaceTable.Clear();
                    replaceTable.Dispose();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~ReplaceItem() {
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
    }
}
