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
using System.Windows.Threading;
using Mocs.Models;
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// SystemStatusControl.xaml の相互作用ロジック
    /// 
    /// Initを呼び出すことでUpdateが定期的に呼び出される
    /// </summary>
    abstract public class MasterBaseControl : UserControl
    {
        protected DBAccess m_db;


        public void Init(DBAccess db)
        {
            this.m_db = db;

            this.Update();
        }

        /// <summary>
        /// 派生クラスで実装すること
        /// いかいだけ呼び出される
        /// </summary>
        abstract protected void Update();
    }

}
