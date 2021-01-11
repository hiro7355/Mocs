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

namespace Mocs.Controls
{
    /// <summary>
    /// HistoryTabControl.xaml の相互作用ロジック
    /// </summary>
    public partial class HistoryTabControl : UserControl
    {
        DBAccess m_db;
        public HistoryTabControl()
        {
            InitializeComponent();


        }

        internal void Init(DBAccess db)
        {
            m_db = db;

            orderHistory.Init(m_db);
            errorHistory.Init(m_db);
            comHistory.Init(m_db);
            cancelHistory.Init(m_db);

        }
    }
}
