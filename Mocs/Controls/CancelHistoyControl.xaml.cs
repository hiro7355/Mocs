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
    /// CancelHistoyControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CancelHistoyControl : HistoryBaseControl
    {
        public CancelHistoyControl()
        {
            InitializeComponent();
        }
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

        }
        protected override void Update()
        {
            string sql = SqlForCancelHistory.GetListSql();

            System.Data.DataTable table = m_db.getDataTable(sql);

            cancelHistory.DataContext = table;
        }

    }
}
