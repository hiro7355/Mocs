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
    abstract public  class TimerBaseControl : UserControl
    {
        protected DBAccess m_db;

        private DispatcherTimer m_timer;


        public void Init(DBAccess db)
        {
            this.m_db = db;
            SetupTimer();
        }

        private void SetupTimer()
        {

            m_timer = new DispatcherTimer();
            m_timer.Interval = new TimeSpan(0, 0, Int32.Parse(Properties.Settings.Default.Cell_Monitoring_Interval));

            m_timer.Tick += new EventHandler(TimerFunc);
            m_timer.Start();

            TimerFunc(null, null);
        }

        /// <summary>
        /// Cell監視処理
        /// Timerにより一定間隔で呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerFunc(object sender, EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// 派生クラスで実装すること
        /// 一定間隔で呼び出される
        /// </summary>
        abstract protected void Update();
    }

}
