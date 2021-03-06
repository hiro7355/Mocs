﻿using System;
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
    /// OrderHistoryControl.xaml の相互作用ロジック
    /// </summary>
    public partial class OrderHistoryControl : HistoryBaseControl
    {

        public OrderHistoryControl()
        {
            InitializeComponent();

            //  検索ダイアログを生成
            this.CreateSearchDialog<OrderHistory>();
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


        protected override void Update(string conditionSql, int unionType, string comboValue)
        {
            string sql = SqlForOrderHistory.GetListSql(conditionSql);

            System.Data.DataTable table = m_db.getDataTable(sql);

            orderHistory.DataContext = table;
        }

    }
}
