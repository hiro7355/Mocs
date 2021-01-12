using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mocs.SearchDialogs
{
    abstract public class BaseSearchDialog : Window, IDisposable
    {
        private int m_result;       //  0:キャンセル、1:決定、2:全て
        protected DBAccess m_db;


        public BaseSearchDialog()
        {
            m_db = DBAccess.GetDBAccess();
        }

        internal int GetResult()
        {
            return m_result;
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        protected void DoCancel()
        {
            m_result = 0;
            this.DialogResult = false;
        }

        /// <summary>
        /// 決定
        /// </summary>
        protected void DoOK()
        {
            m_result = 1;
            this.DialogResult = true;
        }

        /// <summary>
        /// 全検索
        /// </summary>
        protected void DoAll()
        {
            m_result = 2;
            this.DialogResult = true;
        }

        /// <summary>
        /// this.DialogResultに値が設定されるかthis.Close()で呼び出される
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            if (IsVisible)
            {
                // 表示している場合は、閉じる処理をキャンセルして非表示にする
                e.Cancel = true;

            } 
            this.Hide();
        }

        public void Dispose()
        {
            //  OnClosingが呼び出されるが、IsVisibleはfalseなのでキャンセルにはならない
            this.Close();
        }

        abstract public string GetConditionSql();

        protected DBAccess GetDBAccess()
        {
            return DBAccess.GetDBAccess();
        }

        /// <summary>
        /// コンボボックスに初期データ設定
        /// 
        /// </summary>
        /// <param name="sql">idとnameをセレクトするsql文</param>
        /// <param name="combo">コンボボックス</param>
        protected void InitCombo(string sql, ComboBox combo)
        {
            if (combo.DataContext == null)
            {
                System.Data.DataTable table = m_db.getDataTable(sql);
                combo.DataContext = table;
            }
        }

        /// <summary>
        /// コントロールの有効無効切り替え
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="controls"></param>
        protected void SetControlsEnabled(bool enabled, Control[] controls)
        {
            foreach (Control control in controls)
            {
                control.IsEnabled = enabled;
            }
        }


        /// <summary>
        /// スタックパネルの有効無効切り替え
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="stack"></param>
        protected void SetStackEnabled(bool enabled, StackPanel stack)
        {
            stack.IsEnabled = enabled;
        }


    }
}
