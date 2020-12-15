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
    /// StationListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class StationListControl : MasterBaseControl
    {
        public StationListControl()
        {
            InitializeComponent();
        }

        protected override void Update()
        {
            string sql = SqlForStationList.GetListSql();

            System.Data.DataTable table = m_db.getDataTable(sql);

            stationList.DataContext = table;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Data.DataRowView rowView = (((Button)sender).Tag as System.Data.DataRowView);
            System.Data.DataRow row = rowView.Row;

            Int16 station_enable = (Int16)row["station_enable"];
            station_enable ^= 1;        //  値反転

            Int32 station_section_id = (Int32)row["station_section_id"];

            if (station_section_id != 0)
            {
                string sect = (string)row["sect"];  //  部署名
                string change_enable = (string)row["change_enable"];

                string msg = string.Format(Properties.Resources.CONFIRM_STATION_CHANGE_ENABLE, sect, Properties.Resources.STATION, change_enable);
                MessageBoxResult res = MessageBox.Show(msg, Properties.Resources.CONFIRM, MessageBoxButton.OKCancel,
                   MessageBoxImage.Question, MessageBoxResult.Cancel);
                switch (res)
                {
                    case MessageBoxResult.OK:
                        // OKの処理

                        m_db.execute("UPDATE station_master SET station_enable=" + station_enable + " WHERE station_section_id=" + station_section_id);

                        this.Update();

                        break;
                    case MessageBoxResult.Cancel:
                        // Cancelの処理
                        break;
                }

            }


        }
    }
}
