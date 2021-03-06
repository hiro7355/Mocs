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
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// ErrorHistoryControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorHistoryControl : HistoryBaseControl
    {
        public ErrorHistoryControl()
        {
            InitializeComponent();

            //  検索ダイアログを生成
            this.CreateSearchDialog<ErrorHistory>();

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
            string cellName = CommonUtil.GetCellName(m_db.Conn);

            string sql = SqlForErrorHistory.GetListSql(conditionSql, unionType, cellName);

            System.Data.DataTable table = m_db.getDataTable(sql);

            errorHistory.DataContext = table;
        }

        /// <summary>
        /// 詳細ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Detail_Click(object sender, RoutedEventArgs e)
        {

            string message = (string)((Button)sender).Tag;
            MessageBox.Show(message, Properties.Resources.DETAIL);
            
        }
    }
}
