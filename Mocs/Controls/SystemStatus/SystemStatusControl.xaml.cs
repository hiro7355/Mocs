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

namespace Mocs.Controls.SystemStatus
{
    /// <summary>
    /// SystemStatusControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemStatusControl : UserControl
    {
        DBAccess m_db;

        DispatcherTimer m_timer;

        DateTime m_last_update_time;     //  前回の更新時間
        
        public SystemStatusControl()
        {
            InitializeComponent();
        }


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
            //  運転状態監視
            UpdateCell();
        }

        /// <summary>
        /// 運転状態監視
        /// </summary>
        private void UpdateCell()
        {
            //  cell_statusの読み込み
            CellStatus cellStatus = CellStatus.GetFirst(m_db.Conn);

            int status = cellStatus.cellstat_status;
            int level = cellStatus.cellstat_level;
            DateTime update_time = cellStatus.cellstat_stat_update_datetime;
            int mu_status = cellStatus.cellstat_mu_status;

            if (status == 0 || status == 10)
            {
                //  CELLが起動していません
                this.UpdateLedAndMessage(this.cell, "White", "White", Properties.Resources.MSG_CELL_NOT_RUNNING);
            } 
            else if (status == 1)
            {
                if (level == 1)
                {
                    if (m_last_update_time != update_time)
                    {
                        //  通常運転中です
                        this.UpdateLedAndMessage(this.cell, "Green", "White", Properties.Resources.MSG_CELL_RUNNING);
                    }
                    else
                    {
                        //  CELLの動作が停止しています
                        this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_STOPPED);
                    }

                } 
                else if (level == 2)
                {
                    //  管制運転中（火災）です
                    this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_FIRE);

                }
                else if (level == 3)
                {
                    //  管制運転中（地震）です
                    this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_EARTHQUAKE);
                }
                else if (level == 4)
                {
                    //  管制運転中（火災）です
                    this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_FIRE);
                }
                else if (level == 9)
                {
                    //  システム停止（継続不可）しました。
                    this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_SYSTEM_STOPPED);
                } 
            }
            else
            {
                if (mu_status == 0)
                {
                    //  MUの状態を確認しています。
                    this.UpdateLedAndMessage(this.cell, "White", "White", Properties.Resources.MSG_CELL_CHECK_MU);
                }
                else if (mu_status == 1)
                {
                    //  オフラインMUがあります。
                    this.UpdateLedAndMessage(this.cell, null, "White", Properties.Resources.MSG_CELL_MU_OFF);
                }
                else if (mu_status == 2)
                {
                    //  オフラインMUと異常のMUがあります。
                    this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_MU_OFF_ERROR);
                }
                else if (mu_status == 3)
                {
                    //  MU全てオンライン正常です
                    this.UpdateLedAndMessage(this.cell, null, "White", Properties.Resources.MSG_CELL_MU_OK);
                }
                else if (mu_status == 4)
                {
                    //  MUで異常が発生しています。
                    this.UpdateLedAndMessage(this.cell, "Red", "Red", Properties.Resources.MSG_CELL_MU_ERROR);
                }
            }

            //  次回確認ように更新時間を設定
            m_last_update_time = update_time;



        }

        /// <summary>
        /// LEDとメッセージを更新
        /// </summary>
        /// <param name="led">LEDコントローラー</param>
        /// <param name="ledColorName">LED色</param>
        /// <param name="bgColorName">文字背景色</param>
        /// <param name="message">メッセージ</param>
        private void UpdateLedAndMessage(LedControl led, String ledColorName, String bgColorName, String message)
        {
            if (ledColorName != null)
            {
                led.SetColorFromName(ledColorName);
            }

            this.message.Background = ColorUtil.brushFromColorName(bgColorName);
            if (this.message.Text != "" )
            {
                this.message.Text += "\n";
            }
            this.message.Text += message;
        }
    }
}
