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
using Mocs.CellMonTabNet;
using System.Threading;

namespace Mocs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        
        DBAccess m_db;

        SurveyMonitor m_monitor = new SurveyMonitor();

        // 監視モニタ制御スレッド
        private Thread threadMonCtrlLoop;
        private int _MonCtrlPeriod = 500;
        private bool bMonCtrlContinue = true;


        /// <summary>
        /// converterからDBへアクセスできるようにするため
        /// </summary>
        /// <returns></returns>
        public DBAccess GetDBAccess()
        {
            return m_db;
        }

        public MainWindow(DBAccess db)
        {
            m_db = db;

            SysMainTbl sys_main_tbl = m_db.sys_main_tbl;

            m_monitor.CellIpAddress = sys_main_tbl.cell_ip.Item1.ToString();
            m_monitor.CellPortNo = (ushort)sys_main_tbl.tab_port;

            m_monitor.Id = 1;
            m_monitor.Name = "monitor";
            m_monitor.MonitorIpAddress = "127.0.0.1";
            m_monitor.MonitorPortNo = (ushort)sys_main_tbl.tab_term_port;


            InitializeComponent();


            //  CELL運転などを行うスレッド開始
            this.startThread();





        }

        public void InitWithDB()
        {

            ErrorInfo errorInfo = new ErrorInfo();

            this.systemStatusControl.Init(m_db, errorInfo);
            this.muListControl.Init(m_db, errorInfo);
            this.errorInfoControl.Init(m_db, errorInfo);
            this.orderListControl.Init(m_db, errorInfo);
            this.cartListControl.Init(m_db);
            this.tabletListControl.Init(m_db);
            this.floorListControl.Init(m_db);
            this.stationListControl.Init(m_db);
            this.historyTabControl.Init(m_db);


        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            historyTabControl.Dispose();

        }


        private void ShowPannel(bool showSystemStatus, UserControl control)
        {


            if (showSystemStatus)
            {
                this.systemStatusPanel.Visibility = Visibility.Visible;
                this.historyTabControl.Visibility = Visibility.Collapsed;

                //  いったんシステム状態の隣のコントロールをすべて非表示にする
                this.hideAllNextSystemStatusControl();

                if (control != null)
                {
                    //  指定されたコントロールを表示する
                    control.Visibility = Visibility.Visible;

                }
            }
            else
            {
                this.systemStatusPanel.Visibility = Visibility.Collapsed;
                this.historyTabControl.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// システム状態の隣のコントロールをすべて非表示にする
        /// </summary>
        private void hideAllNextSystemStatusControl()
        {
            this.muListControl.Visibility = Visibility.Collapsed;
            this.errorInfoControl.Visibility = Visibility.Collapsed;
            this.cartListControl.Visibility = Visibility.Collapsed;
            this.tabletListControl.Visibility = Visibility.Collapsed;
            this.floorListControl.Visibility = Visibility.Collapsed;
            this.stationListControl.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// 履歴ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(false, null);

        }

        /// <summary>
        /// 異常情報ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void errorInfoButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, this.errorInfoControl);

        }

        /// <summary>
        /// カート一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cartListButton_Click(object sender, RoutedEventArgs e)
        {

            ShowPannel(true, this.cartListControl);
        }

        /// <summary>
        /// フロア一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void floorListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, floorListControl);

        }

        /// <summary>
        /// ステーション一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stationListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, stationListControl);

        }

        /// <summary>
        /// タブレット一覧クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabletListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, tabletListControl);

        }

        private void deviceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, muListControl);

        }

        private void systemStatusButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, muListControl);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            this.Hide();
            //ログイン画面の表示
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Owner = GetWindow(this);
            bool? result = loginWindow.ShowDialog();
            if (result == false)
            {
                this.Close();
            }
            this.Show();
            */
        }


        /// <summary>
        /// クローズメニュークリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        /// <summary>
        /// ヘルプメニュークリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new Microsoft.Win32.OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "PDFファイル (*.pdf)|*.pdf";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // pdfを開く
                System.Diagnostics.Process.Start(dialog.FileName);
            }
        }

        /// <summary>
        /// 運転ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            if (true != this.m_monitor.ReqOperation(CellOperationType.eType.Start))
            {
                MessageBox.Show(Properties.Resources.ERROR_RUN);
            }

        }

        /// <summary>
        /// 停止ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopBtton_Click(object sender, RoutedEventArgs e)
        {
            if (true != this.m_monitor.ReqOperation(CellOperationType.eType.Stop))
            {
                MessageBox.Show(Properties.Resources.ERROR_STOP);
            }

        }

        /// <summary>
        /// ブザー通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopBuzzerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 地震異常復帰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void earthquakeButton_Click(object sender, RoutedEventArgs e)
        {
            doRecover();
        }

        /// <summary>
        /// 火災異常復帰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fireButton_Click(object sender, RoutedEventArgs e)
        {
            doRecover();

        }

        /// <summary>
        /// 停電異常復帰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void powerButton_Click(object sender, RoutedEventArgs e)
        {
            doRecover();
        }

        private void doRecover()
        {
            if (true != this.m_monitor.ReqOperation(CellOperationType.eType.Recovery))
            {
                MessageBox.Show(Properties.Resources.ERROR_RECOVER);
            }

        }


        private void startThread()
        {
            try
            {
                threadMonCtrlLoop = new Thread(threadFuncMonCtrlLoop);
                threadMonCtrlLoop.IsBackground = true;
                threadMonCtrlLoop.Start(this._MonCtrlPeriod);

            }
            catch (Exception ex)
            {

            }

        }
        /// <summary>
        /// 監視モニタ制御スレッド
        /// </summary>
        /// <param name="objSimPeriod"></param>
        private void threadFuncMonCtrlLoop(object objMonCtrlPeriod)
        {
            int simCount = 0;
            int simPeriod = (int)objMonCtrlPeriod;

            while (bMonCtrlContinue)
            {
                m_monitor.execCtrlProc();

                Console.WriteLine("ctrl count:{0}", simCount);
                simCount++;
                Thread.Sleep(simPeriod);
            }

        }
    }
}
