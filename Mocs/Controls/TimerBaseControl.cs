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
        protected ErrorInfo m_errorInfo;

        private DispatcherTimer m_timer;

        private bool m_isFirstUpdate;
        public void Init(DBAccess db, ErrorInfo errorInfo)
        {
            this.m_db = db;
            this.m_errorInfo = errorInfo;

            m_isFirstUpdate = true;
            //  初回は１秒後に実行
            SetupTimer(1);
        }

        private void SetupTimer(int sec)
        {

            m_timer = new DispatcherTimer();
            m_timer.Interval = new TimeSpan(0, 0, sec);

            m_timer.Tick += new EventHandler(TimerFunc);
            m_timer.Start();

        }

        public void StopTimer()
        {
            if (m_timer != null)
            {
                m_timer.Stop();
                m_timer = null;
            }
        }

        /// <summary>
        /// Cell監視処理
        /// Timerにより一定間隔で呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerFunc(object sender, EventArgs e)
        {
            if (m_isFirstUpdate)
            {
                //  初回実行のとき


                m_isFirstUpdate = false;

                //  タイマーを再設定
                StopTimer();


                //  ２回目からは設定ファイルにしたがって一定間隔で処理する
                SetupTimer(Int32.Parse(Properties.Settings.Default.Cell_interval));

            }

            Update();
        }

        /// <summary>
        /// 派生クラスで実装すること
        /// 一定間隔で呼び出される
        /// </summary>
        abstract protected void Update();
    }

}
