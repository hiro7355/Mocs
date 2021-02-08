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
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// FloorListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FloorListControl : MasterBaseControl
    {
        public FloorListControl()
        {
            InitializeComponent();


        }

        private void UpdateCombo()
        {
            if (selectBox.DataContext == null)
            {
                string localeCode = CommonUtil.GetAppLocaleCode();

                string sql = "SELECT hospital_id, hospital_name_" + localeCode + " AS hospital_name FROM hospital_master";
                System.Data.DataTable table = m_db.getDataTable(sql);
                System.Data.DataRow row_for_all = table.NewRow();

                row_for_all["hospital_id"] = "0";
                row_for_all["hospital_name"] = Properties.Resources.COMBO_ALL;
                table.Rows.InsertAt(row_for_all, 0);
                selectBox.DataContext = table;

            }

        }
        protected override void Update()
        {
            this.UpdateCombo();


            string hospital_id = this.selectBox.SelectedValue != null ? this.selectBox.SelectedValue.ToString() : "0";
            string sql = SqlForFloorList.GetListSql(hospital_id);

            System.Data.DataTable table = m_db.getDataTable(sql);

            floorList.DataContext = table;
        }

        private void selectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.m_db != null)
            {
                this.Update();
            }
        }

    }
}
