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
    /// CartListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CartListControl : MasterBaseControl
    {
        public CartListControl()
        {
            InitializeComponent();
        }

        protected override void Update()
        {
            ComboBoxItem item = (ComboBoxItem)this.selectBox.SelectedItem;
            string cart_enable = item != null ? item.Tag.ToString() : "all";
            string sql = SqlForCartList.GetListSql(cart_enable);

            System.Data.DataTable table = m_db.getDataTable(sql);

            cartList.DataContext = table;
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
