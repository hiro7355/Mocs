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

namespace Mocs.Controls
{
    /// <summary>
    /// OrderListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class OrderListControl : TimerBaseControl
    {
        private List<int> m_excludeOrderIds;
        public OrderListControl()
        {
            InitializeComponent();

            m_excludeOrderIds = new List<int>();

        }


        protected override void Update()
        {
            // 　定期更新はしない
            StopTimer();

            ComboBoxItem item = (ComboBoxItem)this.selectBox.SelectedItem;
            string orderStatus = item != null ? item.Tag.ToString() : "all";

            string sql = SqlForOrderList.GetListSql(orderStatus, m_excludeOrderIds.ToArray());

            System.Data.DataTable table = m_db.getDataTable(sql);

            orderList.DataContext = table;
        }

        private void selectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.m_db != null)
            {
                this.Update();
            }

        }

        /// <summary>
        /// 削除ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            System.Data.DataTable table = (System.Data.DataTable)this.orderList.DataContext;

            var ids = new List<int>();


            foreach (System.Data.DataRow row in table.Rows)
            {
                bool isChecked = (bool)row["is_checked"];
                if (isChecked)
                {
                    int order_id = (int)row["order_id"];

                    ids.Add(order_id); // 末尾に追加
                }

            }

            if (ids.Count() > 0)
            {
                //  削除確認ダイアログ
                if (MessageBox.Show(Properties.Resources.MSG_DELETE_ORDER, Properties.Resources.CONFIRM, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //  削除対象のorder_idを保存
                    m_excludeOrderIds.AddRange(ids);

                    //  表示更新
                    this.Update();

                }
            }



        }
    }
}
