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
    /// TabletListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TabletListControl : MasterBaseControl
    {
        public TabletListControl()
        {
            InitializeComponent();
        }
        protected override void Update()
        {
            string sql = SqlForTabletList.GetListSql();

            System.Data.DataTable table = m_db.getDataTable(sql);

            tabletList.DataContext = table;
        }
    }


}
