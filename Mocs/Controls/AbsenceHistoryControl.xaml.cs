using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mocs.Models;
using Mocs.SearchDialogs;

namespace Mocs.Controls
{
    /// <summary>
    /// AbsenceHistoryControl.xaml の相互作用ロジック
    /// </summary>
    public partial class AbsenceHistoryControl : HistoryBaseControl
    {
        public AbsenceHistoryControl()
        {
            InitializeComponent();
            //  検索ダイアログを生成
            this.CreateSearchDialog<AbsenceHistory>();
        }
        /// <summary>
        /// 検索ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            this.DoSearch();
        }

        /// <summary>
        /// 一覧更新
        /// </summary>
        /// <param name="conditionSql"></param>
        /// <param name="unionType"></param>
        protected override void Update(string conditionSql, int unionType, string comboValue)
        {
            string sql = SqlForAbsenceHistory.GetListSql(conditionSql, comboValue);

            System.Data.DataTable table = m_db.getDataTable(sql);

            absenceHistory.DataContext = table;
        }

        /// <summary>
        /// コンボの選択変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.selectBox.SelectedItem;
            string value = item != null ? item.Tag.ToString() : "all";

            //  Updateが呼び出される
            this.DoSearchForCombo(value);

        }
    }
}
