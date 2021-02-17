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
using Mocs.Utils;
using Mocs.Models;

namespace Mocs.SearchDialogs
{
    /// <summary>
    /// ComHistory.xaml の相互作用ロジック
    /// </summary>
    public partial class ComHistory : BaseSearchDialog
    {
        public ComHistory()
        {
            InitializeComponent();
            string localeCode = CommonUtil.GetAppLocaleCode();

            //  コンボボックスの初期化
            InitCombo(SqlForComHistory.SelectNameSql(localeCode), this.comboEquip);

            //  初期有効無効表示
            SetStartEndEnabled(false);
            SetEquipEnabled(false);

            //  検索範囲日付を初期化
            this.dateStart.SelectedDate = DateTime.Today;
            this.dateEnd.SelectedDate = DateTime.Today;

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
                    string sql = SqlForComHistory.GetStartSql((DateTime)this.dateStart.SelectedDate);
                    values.Add(sql);
                }
                if (this.dateEnd.SelectedDate != null)
                {
                    string sql = SqlForComHistory.GetEndSql((DateTime)this.dateEnd.SelectedDate);
                    values.Add(sql);
                }
            }

            //  機器名称
            if (this.checkEquip.IsChecked == true)
            {
                string sql = SqlForComHistory.GetNameSql((string)this.comboEquip.SelectedValue);
                values.Add(sql);
            }

            //  送受信
            if (this.radioSend.IsChecked == true)
            {
                string sql = SqlForComHistory.GetDirSql(1);
                values.Add(sql);

            }
            if (this.radioReceive.IsChecked == true)
            {
                string sql = SqlForComHistory.GetDirSql(2);
                values.Add(sql);

            }


            return string.Join(" and ", values);
        }


        private void SetStartEndEnabled(bool enabled)
        {
            SetStackEnabled(enabled, this.stackStartEnd);
        }

        private void SetEquipEnabled(bool enabled)
        {
            SetStackEnabled(enabled, this.stackEquip);
        }

        private void checkStartEnd_Checked(object sender, RoutedEventArgs e)
        {
            SetStartEndEnabled(true);

        }

        private void checkStartEnd_Unchecked(object sender, RoutedEventArgs e)
        {
            SetStartEndEnabled(false);

        }

        private void checkEquip_Checked(object sender, RoutedEventArgs e)
        {
            SetEquipEnabled(true);
        }

        private void checkEquip_Unchecked(object sender, RoutedEventArgs e)
        {
            SetEquipEnabled(false);

        }
    }
}
