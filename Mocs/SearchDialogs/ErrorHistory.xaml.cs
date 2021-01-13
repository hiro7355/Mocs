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
using System.Windows.Shapes;
using Mocs.Models;

namespace Mocs.SearchDialogs
{
    /// <summary>
    /// ErrorHistory.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorHistory : BaseSearchDialog
    {
        public ErrorHistory()
        {
            InitializeComponent();

            //  初期有効無効表示
            SetStartEndEnabled(false);
            SetTypeEnabled(false);

        }

        /// <summary>
        /// キャンセル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DoCancel();
        }

        /// <summary>
        /// 決定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DoOK();
        }

        /// <summary>
        /// 全検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allButton_Click(object sender, RoutedEventArgs e)
        {
            this.DoAll();
        }

        private void checkStartEnd_Checked(object sender, RoutedEventArgs e)
        {
            SetStartEndEnabled(true);

        }

        private void checkStartEnd_Unchecked(object sender, RoutedEventArgs e)
        {
            SetStartEndEnabled(false);

        }

        private void SetStartEndEnabled(bool enabled)
        {
            SetStackEnabled(enabled, this.stackStartEnd);
        }

        private void SetTypeEnabled(bool enabled)
        {
            SetStackEnabled(enabled, this.stackType);
        }


        private void checkType_Checked(object sender, RoutedEventArgs e)
        {
            SetTypeEnabled(true);

        }

        private void checkType_Unchecked(object sender, RoutedEventArgs e)
        {
            SetTypeEnabled(false);

        }

        /// <summary>
        /// 検索条件SQLを取得
        /// </summary>
        /// <returns></returns>
        public override string GetConditionSql()
        {
            List<string> values = new List<string>();

            //  検索期間
            if (this.checkStartEnd.IsChecked == true)
            {
                if (this.dateStart.SelectedDate != null)
                {
                    string sql = SqlForErrorHistory.GetStartSql((DateTime)this.dateStart.SelectedDate);
                    values.Add(sql);
                }
                if (this.dateEnd.SelectedDate != null)
                {
                    string sql = SqlForErrorHistory.GetEndSql((DateTime)this.dateEnd.SelectedDate);
                    values.Add(sql);
                }
            }

            return string.Join(" and ", values);
        }

        /// <summary>
        /// unionするとき、どれをunionするのかの情報
        /// 0はすべてunion
        /// </summary>
        /// <returns></returns>
        public override int GetUnionType()
        {
            int unionType = 0;
            if (this.checkType.IsChecked == true)
            {
                ComboBoxItem item = (ComboBoxItem)this.comboType.SelectedItem;
                unionType = item != null ? int.Parse(item.Tag.ToString()) : 0;
            }
            return unionType;
        }

    }
}
