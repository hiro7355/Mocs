using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Mocs.Models;
using Mocs.Utils;
using Mocs.SearchDialogs;

namespace Mocs.Controls
{
    abstract public class HistoryBaseControl : UserControl, IDisposable
    {
        protected DBAccess m_db;

        protected BaseSearchDialog m_search;
        public void Dispose()
        {
            if (m_search != null)
            {
                m_search.Dispose();
            }
        }

        public void CreateSearchDialog<T>() where T : BaseSearchDialog, new()
        {
            m_search = new T();
        }


        public void Init(DBAccess db)
        {
            this.m_db = db;
            this.Update(null, 0);
        }


        /// <summary>
        /// 検索ボタンクリック時に派生クラスから呼び出される
        /// 検索ダイアログを表示して再検索
        /// </summary>
        protected void DoSearch()
        {
            m_search.ShowDialog();

            if (m_search.GetResult() != 0)
            {
                //  キャンセル以外のとき


                string conditionSql = null;
                int unionType = 0;
                if (m_search.GetResult() == 1)
                {
                    //  決定のとき条件を取得
                    conditionSql = m_search.GetConditionSql();
                    unionType = m_search.GetUnionType();
                }


                //  検索結果を表示
                this.Update(conditionSql, unionType);
            }


        }

        /// <summary>
        /// 派生クラスで実装すること
        /// init時に呼び出される
        /// </summary>
        /// <param name="conditionSql">where以下のsql文</param>
        /// <param name="unionType">unionする場合、どれをunionするか。0はすべてunion</param>
        abstract protected void Update(string conditionSql, int unionType);

    }
}
