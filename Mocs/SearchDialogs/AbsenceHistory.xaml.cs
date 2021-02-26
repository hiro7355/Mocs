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
using Mocs.Utils;
using Mocs.Models;

namespace Mocs.SearchDialogs
{
    /// <summary>
    /// AbsenceHistory.xaml の相互作用ロジック
    /// </summary>
    public partial class AbsenceHistory : BaseSearchDialog
    {

        public AbsenceHistory()
        {
            InitializeComponent();
            string localeCode = CommonUtil.GetAppLocaleCode();

            //  コンボボックスの初期化
            InitCombo(SectionMaster.SelectIdAndNameSql(localeCode), this.comboReqSect);

            //  検索条件を初期化
            InitCondition();


        }

        protected override void InitCondition()
        {
            //  日付は初期状態でチェックON
            this.checkStartEnd.IsChecked = true;
            //  初期有効無効表示
            SetStartEndEnabled(true);
            SetReqSectEnabled(false);

            //  検索範囲日付を初期化
            this.dateStart.SelectedDate = DateTime.Today;
            this.dateEnd.SelectedDate = DateTime.Today;

            this.checkReqSect.IsChecked = false;
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
                    string sql = SqlForAbsenceHistory.GetStartSql((DateTime)this.dateStart.SelectedDate);
                    values.Add(sql);
                }
                if (this.dateEnd.SelectedDate != null)
                {
                    string sql = SqlForAbsenceHistory.GetEndSql((DateTime)this.dateEnd.SelectedDate);
                    values.Add(sql);
                }
            }

            //  発部署
            if (this.checkReqSect.IsChecked == true)
            {
                string sql = SqlForAbsenceHistory.GetSectSql((int)this.comboReqSect.SelectedValue);
                values.Add(sql);
            }


            return string.Join(" and ", values);
        }


        private void SetStartEndEnabled(bool enabled)
        {
            SetStackEnabled(enabled, this.stackStartEnd);
        }
        private void SetReqSectEnabled(bool enabled)
        {
            SetStackEnabled(enabled, this.stackReqSect);
        }
        private void checkStartEnd_Checked(object sender, RoutedEventArgs e)
        {
            SetStartEndEnabled(true);

        }

        private void checkStartEnd_Unchecked(object sender, RoutedEventArgs e)
        {
            SetStartEndEnabled(false);

        }

        private void checkReqSect_Checked(object sender, RoutedEventArgs e)
        {
            SetReqSectEnabled(true);

        }

        private void checkReqSect_Unchecked(object sender, RoutedEventArgs e)
        {
            SetReqSectEnabled(false);

        }

    }
}
