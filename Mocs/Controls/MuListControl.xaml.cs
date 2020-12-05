using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Mocs.Models;
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// MuListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class MuListControl : TimerBaseControl
    {


        public MuListControl()
        {
            InitializeComponent();
        }

        protected override void Update()
        {
            string sql = SqlForMuList.GetListSql();

            System.Data.DataTable table = m_db.getDataTable(sql);

            muList.DataContext = table;
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;

            DataRowView rView = row.Item as DataRowView;

            if (rView != null)
            {
                Brush newBrush = null;

                Object[] values = rView.Row.ItemArray;

                int ope_mode = Int16.Parse(values[1].ToString());
                int muorder_status = Int16.Parse(values[7].ToString());

                if (ope_mode == 0 || ope_mode == 1)
                {
                    newBrush = Brushes.White;
                }
                else if (ope_mode == 2)
                {
                    if (muorder_status == 0 || muorder_status == 10 || muorder_status == 11)
                    {
                        newBrush = Brushes.White;
                    }
                    else if (muorder_status == 1)
                    {
                        newBrush = Brushes.Gray;
                    }
                    else if (muorder_status == 13 || muorder_status == 20 || muorder_status == 21)
                    {
                        newBrush = Brushes.Orange;
                    }
                    else if (muorder_status == 11 || muorder_status == 30)
                    {
                        newBrush = Brushes.Red;
                    }
                }
                else if(ope_mode == 4)
                {
                    newBrush = Brushes.Yellow;

                }

                if (newBrush == null)
                {
                    //  上記にあてはまらないときは白にする
                    newBrush = Brushes.White;
                }

                row.Background = newBrush;

            }
        }
    }
}
