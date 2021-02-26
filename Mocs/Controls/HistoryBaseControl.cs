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

        private string m_comboValue;


        protected string m_condition_of_combo;  //  コンボで設定される条件
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
            this.Update(null, 0, m_comboValue);
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
                this.Update(conditionSql, unionType, m_comboValue);
            }


        }

        /// <summary>
        /// コンボボックスの変更により再検索するとき
        /// 検索ダイアログは表示しないが、検索ダイアログから検索条件を取得する
        /// </summary>
        protected void DoSearchForCombo(string comboValue)
        {
            m_comboValue = comboValue;
            if (m_search != null)
            {

                string conditionSql = m_search.GetConditionSql();
                int unionType = m_search.GetUnionType();


                //  検索結果を表示
                this.Update(conditionSql, unionType, comboValue);

            }

        }

        /// <summary>
        /// 派生クラスで実装すること
        /// init時に呼び出される
        /// </summary>
        /// <param name="conditionSql">where以下のsql文</param>
        /// <param name="unionType">unionする場合、どれをunionするか。0はすべてunion</param>
        abstract protected void Update(string conditionSql, int unionType, string comboValue);

    }
}
