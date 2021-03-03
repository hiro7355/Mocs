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
using System.Media;
using System.IO;

namespace Mocs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        
        DBAccess m_db;

        SoundPlayer m_wavePlayer;

        string m_lastErrorMessage;

        SurveyMonitor m_monitor = new SurveyMonitor();

        // 監視モニタ制御スレッド
        private Thread threadMonCtrlLoop;
        private int _MonCtrlPeriod = 500;
        private bool bMonCtrlContinue = true;
        private Brush m_systemStatusButtonBackground;
        private Brush m_red;
        private Brush m_orange;
        private Brush m_white;
        private Brush m_gray;
        private Brush m_green;
        private Brush m_black;
        private Brush m_yellow;
        private Brush m_light_gray;

        private static Button g_currentButtonForCell;        //  ソケット通信処理中アクションに対応するボタン

        private static MainWindow g_this;


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
            g_this = this;

            m_db = db;

            SysMainTbl sys_main_tbl = m_db.sys_main_tbl;

            m_monitor.CellIpAddress = sys_main_tbl.cell_ip.Item1.ToString();
            m_monitor.CellPortNo = (ushort)sys_main_tbl.tab_port;

            
            m_monitor.Id = 1;
            m_monitor.Name = "monitor";
            //  monitoripaddressはcellとの通信には必要のない値だが、なにか設定しておかないとエラーになってしまうので設定する。
            m_monitor.MonitorIpAddress = "127.0.0.1";
            //  CELLからのメッセージ受信用のモニターのポート番号を設定
            m_monitor.MonitorPortNo = (ushort)sys_main_tbl.tab_term_port;
            //

            //  CELLとの通信完了のコールバック設定
            Utils.CommonUtil.SetCallbackCellRequestDone(CellRequestDone);

            InitializeComponent();

            //  ボタン背景色を保存
            m_systemStatusButtonBackground = systemStatusButton.Background;
            m_red = Brushes.Red;
            m_orange = Brushes.Orange; ;
            m_white = Brushes.White;
            m_gray = Brushes.Gray;
            m_green = Brushes.Green;
            m_black = Brushes.Black;
            m_yellow = Brushes.Yellow;

            m_light_gray = Brushes.LightGray; 


            //  CELL運転などを行うスレッド開始
            this.startThread();


            //  システムボタンをいったん無効に
            InitSystemButton();

            //  最後のメッセージ情報を取得（ここではnullがかえる）
            m_lastErrorMessage = this.systemStatusControl.GetLastErrorMessage();


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
            this.floorListControl.Init(m_db, errorInfo);
            this.stationListControl.Init(m_db);
            this.historyTabControl.Init(m_db);
            this.deviceControl.Init(m_db, errorInfo);


        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            historyTabControl.Dispose();

        }


        private void ShowPannel(bool showSystemStatus, UserControl control, Button button)
        {
            //  対応するボタンを無効状態にする
            UpdateButtonStatus(button);

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

            //  システム状態ボタンの背景を更新
            UpdateSystemControlButton();

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
            this.deviceControl.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// 履歴ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(false, null, (Button)sender);

        }

        /// <summary>
        /// 異常情報ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void errorInfoButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, this.errorInfoControl, (Button)sender);

        }

        /// <summary>
        /// カート一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cartListButton_Click(object sender, RoutedEventArgs e)
        {

            ShowPannel(true, this.cartListControl, (Button)sender);
        }

        /// <summary>
        /// フロア一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void floorListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, floorListControl, (Button)sender);

        }

        /// <summary>
        /// ステーション一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stationListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, stationListControl, (Button)sender);

        }

        /// <summary>
        /// タブレット一覧クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabletListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, tabletListControl, (Button)sender);

        }

        /// <summary>
        /// 周辺機器ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deviceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, deviceControl, (Button)sender);

        }

        /// <summary>
        /// システム状態ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void systemStatusButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, muListControl, (Button)sender);

        }

        /// <summary>
        /// 左側のボタンの有効無効設定
        /// </summary>
        /// <param name="desableButton"></param>
        private void UpdateButtonStatus(Button desableButton)
        {
            //  いったんすべて有効にする
            this.systemStatusButton.IsEnabled = true;
            this.errorInfoButton.IsEnabled = true;
            this.cartListButton.IsEnabled = true;
            this.floorListButton.IsEnabled = true;
            this.stationListButton.IsEnabled = true;
            this.tabletListButton.IsEnabled = true;
            this.deviceButton.IsEnabled = true;
            this.historyButton.IsEnabled = true;

            //  指定されたボタンを無効にする
            desableButton.IsEnabled = false;


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //  システム状態を表示
            ShowPannel(true, muListControl, this.systemStatusButton);
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
            string pdfPath = Properties.Settings.Default.help;
            if (System.IO.File.Exists(pdfPath))
            {
                // pdfを開く
                System.Diagnostics.Process.Start(pdfPath);
            } else
            {
                MessageBox.Show(string.Format(Properties.Resources.ERROR_NOT_EXISTS_FILE, pdfPath));
            }


            /*
            // ダイアログのインスタンスを生成
            var dialog = new Microsoft.Win32.OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = Mocs.Properties.Resources.FILE_DIALOG_FILTER_PDF;

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // pdfを開く
                System.Diagnostics.Process.Start(dialog.FileName);
            }
            */
        }


        /// <summary>
        /// 運転ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunButtonEnabled())
            {
                //  CELLの起動リクエスト
                RequestCell(CellOperationType.eType.Start, runButton);
            }

        }

        /// <summary>
        /// 停止ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsStopButtonEnabled())
            {
                //  停止ボタンが有効のとき

                //  CELLの停止リクエスト
                RequestCell(CellOperationType.eType.Stop, stopButton);

            }

        }

        /// <summary>
        /// ブザー通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopBuzzerButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsStopButtonEnabled())
            {
                StopSound();
            }
        }

        /// <summary>
        /// 地震異常復帰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void earthquakeButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEarthquakeButtonEnabled())
            {
                //  ボタンが有効なとき復帰処理
                if (RequestCell(CellOperationType.eType.Recovery, this.earthquakeButton) )
                {
                    //  ボタンを無効にする
                    this.SetEarthquakeButtonEnabled(false);
                }

            }
        }

        /// <summary>
        /// 火災異常復帰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fireButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsFireButtonEnabled())
            {
                //  ボタンが有効なとき復帰処理
                if (RequestCell(CellOperationType.eType.Recovery, this.fireButton))
                {
                    //  ボタンを無効にする
                    this.SetFireButtonEnabled(false);
                }
            }

        }

        /// <summary>
        /// 停電異常復帰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void powerButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsPowerButtonEnabled())
            {
                //  ボタンが有効なとき復帰処理
                if (RequestCell(CellOperationType.eType.Recovery, this.powerButton))
                {
                    //  ボタンを無効にする
                    this.SetPowerButtonEnabled(false);
                }

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
            catch (Exception )
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

//                Console.WriteLine("ctrl count:{0}", simCount);
                simCount++;
                Thread.Sleep(simPeriod);
            }

        }


        /// <summary>
        /// CELLのステータス通知
        /// CELL状態をDBから読み込むのタイミングで定期的に呼び出される
        /// </summary>
        private void systemStatusControl_OnCellStatus(object sender, EventArgs e)
        {
            //  ボタンをいったん無効にする
            InitSystemButton();

            //  システムコントロールボタンの背景を設定
            UpdateSystemButton();
        }


        /// <summary>
        /// ステータスとレベルによって、
        /// システムボタンの状態を変更
        /// </summary>
        private void UpdateSystemButton()
        {
            int status = this.systemStatusControl.GetLastStatus();
            int level = this.systemStatusControl.GetLastLevel();

            if (status == 1)
            {

                if (level == 1)
                {
                    //  CELLは起動中

                    //  運転ボタンを無効にする
                    SetRunButtonEnabled(false);

                    //  停止ボタンを有効にする
                    SetStopButtonEnabled(true);



                }
                else if (level == 10)
                {
                    //  CELLは正常に停止中

                    //  運転ボタンを有効にする
                    SetRunButtonEnabled(true);
                    //  停止ボタンを無効にする
                    SetStopButtonEnabled(false);
                }

                if (level == 2 || level == 3 || level == 4)
                {
                    //  異常発生しているとき

                    //  エラー情報を取得
                    string lastMessage = this.systemStatusControl.GetLastErrorMessage();


                    if (lastMessage != null && lastMessage != m_lastErrorMessage)
                    {
                        //  前回と違うエラーのとき
                        if (Properties.Settings.Default.rmelody == "ON")
                        {
                            //  設定が有効のときは、ブザーを鳴らす
                            PlaySound(Properties.Settings.Default.rmelody_file, Mocs.Utils.CommonUtil.parseInt(Properties.Settings.Default.tmelody_time, 10));

                            //  ブザー停止ボタンを有効に
                            SetStopBuzzerButtonEnabled(true);
                        }

                    }

                    if (level == 2)
                    {
                        //  管制運転中（火災）です
                        SetFireButtonEnabled(true);
                    }
                    else if (level == 3)
                    {
                        //  管制運転中（地震）です
                        SetEarthquakeButtonEnabled(true);

                    }
                    else if (level == 4)
                    {
                        //  管制運転中（停電）です
                        SetPowerButtonEnabled(true);
                    }


                    //  エラー情報を更新
                    m_lastErrorMessage = lastMessage;

                }
                else
                {
                    m_lastErrorMessage = null;
                }



            }

            //  システム状態ボタンの背景を更新
            UpdateSystemControlButton();

        }

        /// <summary>
        /// システム状態ボタンの背景を更新
        /// </summary>
        private void UpdateSystemControlButton()
        {

            int level = this.systemStatusControl.GetLastLevel();
            //  CELL停止中かエラーが発生しているときで、システムコントロールが非表示のときはボタンを赤にする
            bool isCellStopped = this.systemStatusControl.IsCellStopped();
            Brush brush = ((isCellStopped || (level > 1)) && !systemStatusControl.IsVisible) ? m_red : m_systemStatusButtonBackground;
            systemStatusButton.Background = brush;
        }

        /// <summary>
        /// 異常情報ボタンの背景を更新
        /// </summary>
        private void UpdateErrorInfoControlButton()
        {

            int errorLevel = this.errorInfoControl.GetErrorLevel();

            Brush brush = errorLevel == 2 ? m_red : (errorLevel == 1 ? m_orange : m_systemStatusButtonBackground);
            errorInfoButton.Background = brush;
        }

        /// <summary>
        /// 　運転ボタン有効：ボタン緑色・文字白
        /// 運転ボタン無効：ボタン灰色・文字黒
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetRunButtonEnabled(bool isEnable)
        {
            if (isEnable)
            {
                //  有効のとき
                UpdateButtonColor(this.runButton, m_white, m_green);
            } 
            else
            {
                //  無効のとき
                UpdateButtonColor(this.runButton, m_black, m_gray);
            }
        }

        /// <summary>
        /// 　停止ボタン有効：ボタン黄色・文字白
        ///  停止ボタン無効：ボタン灰色・文字黒
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetStopButtonEnabled(bool isEnable)
        {
            if (isEnable)
            {
                //  有効のとき
                UpdateButtonColor(this.stopButton, m_white, m_yellow);
            }
            else
            {
                //  無効のとき
                UpdateButtonColor(this.stopButton, m_black, m_gray);
            }
        }
        private bool IsRunButtonEnabled()
        {
            return IsButtonEnabled(runButton);
        }

        private bool IsStopButtonEnabled()
        {
            return IsButtonEnabled(stopButton);
        }


        /// <summary>
        /// ブザー停止ボタン有効：ボタン赤色・文字白
        /// ブザー停止ボタン無効：ボタン灰色・文字黒＆禁止マーク
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetStopBuzzerButtonEnabled(bool isEnable)
        {
            SetImageButtonEnabled(isEnable, stopBuzzerButton, stopBuzzerText, stopBuzzerImage, stopBuzzerImage_d, m_white, m_red, m_black, m_gray);
        }

        /// <summary>
        /// ブザー停止ブタンが有効かどうか
        /// </summary>
        /// <returns></returns>
        private bool IsStopBuzzerButtonEnabled()
        {
            return IsButtonEnabled(stopBuzzerButton);
        }


        /// <summary>
        /// 地震異常復帰ボタン有効：注記マーク赤色＆ボタン灰色・文字黒
        /// 地震異常復帰ボタン無効：注記マーク黒色＆ボタン灰色・文字薄灰色
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetEarthquakeButtonEnabled(bool isEnable)
        {
            SetErrorButtonEnabled(isEnable, earthquakeButton, eathquakeText, earthquakeImage, earthquakeImage_d);
        }
        /// <summary>
        /// 地震異常ボタンが有効かどうか
        /// </summary>
        /// <returns></returns>
        private bool IsEarthquakeButtonEnabled()
        {
            return IsButtonEnabledForErrorButton(earthquakeButton);
        }

        /// <summary>
        /// 火災異常ボタンの有効無効設定
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetFireButtonEnabled(bool isEnable)
        {
            SetErrorButtonEnabled(isEnable, fireButton, fireText, fireImage, fireImage_d);
        }
        /// <summary>
        /// 火災異常ボタンが有効かどうか
        /// </summary>
        /// <returns></returns>
        private bool IsFireButtonEnabled()
        {
            return IsButtonEnabledForErrorButton(fireButton);
        }

        /// <summary>
        /// 電源異常ボタンの有効無効設定
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetPowerButtonEnabled(bool isEnable)
        {
            SetErrorButtonEnabled(isEnable, powerButton, powerText, powerImage, powerImage_d);
        }
        /// <summary>
        /// 電源異常ボタンが有効かどうか
        /// </summary>
        /// <returns></returns>
        private bool IsPowerButtonEnabled()
        {
            return IsButtonEnabledForErrorButton(powerButton);
        }



        /// <summary>
        /// 異常ボタンの有効無効設定
        /// 地震異常復帰ボタン有効：注記マーク赤色＆ボタン灰色・文字黒
        /// 地震異常復帰ボタン無効：注記マーク黒色＆ボタン灰色・文字薄灰色
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetErrorButtonEnabled(bool isEnable, Button button, Label text,  Image validImage, Image invalidImage)
        {
            SetImageButtonEnabled(isEnable, button, text, validImage, invalidImage, m_black, m_gray, m_light_gray, m_gray);
        }


        /// <summary>
        /// イメージつきボタンの色設定
        /// </summary>
        /// <param name="isEnable"></param>
        /// <param name="button"></param>
        /// <param name="text"></param>
        /// <param name="validImage"></param>
        /// <param name="invalidImage"></param>
        /// <param name="validFore"></param>
        /// <param name="validBack"></param>
        /// <param name="invalidFore"></param>
        /// <param name="invalidBack"></param>
        private void SetImageButtonEnabled(bool isEnable, Button button, Label text, Image validImage, Image invalidImage, Brush validFore, Brush validBack, Brush invalidFore, Brush invalidBack)
        {
            //  いったん、ボタンのイメージを消す
            validImage.Visibility = Visibility.Collapsed;
            invalidImage.Visibility = Visibility.Collapsed;

            Brush fore, back;

            if (isEnable)
            {
                //  有効のとき
                fore = validFore;
                back = validBack;

                validImage.Visibility = Visibility.Visible;
            }
            else
            {
                //  無効のとき
                fore = invalidFore;
                back = invalidBack;

                invalidImage.Visibility = Visibility.Visible;
            }

            if (fore != null)
            {
                text.Foreground = fore;
                button.Foreground = fore;       //  異常ボタンのときは有効、無効のチェックにつかう
            }
            if (back != null)
            {
                button.Background = back;
            }

        }



        /// <summary>
        /// ボタンカラーの設定
        /// </summary>
        /// <param name="button"></param>
        /// <param name="fore"></param>
        /// <param name="back"></param>
        private void UpdateButtonColor(Button button, Brush fore, Brush back)
        {
            if (back != null)
            {
                button.Background = back;
            }
            if (fore != null)
            {
                button.Foreground = fore;

            }
        }


        /// <summary>
        /// ボタンの有効無効を識別
        /// 背景色が灰色のとき無効と判断する
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool IsButtonEnabled(Button button)
        {
            return button.Background == m_gray ? false : true;
        }

        /// <summary>
        /// 異常ボタンの有効無効を識別
        /// 異常ボタンは背景色がつねに灰色なので、文字色で無効かどうか判断する
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool IsButtonEnabledForErrorButton(Button button)
        {
            return button.Foreground == m_light_gray ? false : true;
        }



        /// <summary>
        /// システムボタンを無効にする
        /// 起動時と定期チェック時に呼び出される
        /// </summary>
        private void InitSystemButton()
        {
            //  異常復帰ボタンを無効にする
            SetEarthquakeButtonEnabled(false);
            SetFireButtonEnabled(false);
            SetPowerButtonEnabled(false);
            SetStopBuzzerButtonEnabled(false);


            //  運転ボタンを無効にする
            SetRunButtonEnabled(false);
            //  停止ボタンを無効にする
            SetStopButtonEnabled(false);

        }


        /// <summary>
        /// ブザーを鳴らす
        /// </summary>
        /// <param name="buzzerFilePath">サウンドファイルパス</param>
        /// <param name="buzzerSec">継続秒数</param>
        private void PlaySound(string buzzerFilePath, int buzzerSec)
        {
            try
            {
                if (File.Exists(buzzerFilePath))
                {

                    if (m_wavePlayer == null)
                    {
                        m_wavePlayer = new SoundPlayer(buzzerFilePath);
                    }
                    if (m_wavePlayer != null)
                    {
                        m_wavePlayer.PlayLooping(); // ループ再生

                        //  指定された秒数の後、再生停止するためタイマーを開始
                        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, buzzerSec);
                        dispatcherTimer.Start();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            StopSound();
        }

            
        private void StopSound()
        {
            if (m_wavePlayer != null)
            {
                m_wavePlayer.Stop();

                m_wavePlayer = null;

                //  ブザー停止ボタンを無効に
                SetStopBuzzerButtonEnabled(false);

            }
        }

        private bool isSoundPlaying()
        {
            return m_wavePlayer != null ? true : false;
        }

        private void errorInfoControl_OnCellStatus(object sender, EventArgs e)
        {
            //  異常情報ボタンの背景を更新
            UpdateErrorInfoControlButton();
        }

        /*

        /// <summary>
        /// ブザー停止ボタンの有効無効設定
        /// </summary>
        /// <param name="enabled"></param>
        private void StopBuzzerButtonEnabled(bool enabled)
        {
            stopBuzzerButton.IsEnabled = enabled;
            stopBuzzerImage.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
            stopBuzzerImage_d.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void EarthquakeButtonEnabled(bool enabled)
        {
            earthquakeButton.IsEnabled = enabled;
            earthquakeImage.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
            earthquakeImage_d.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void FireButtonEnabled(bool enabled)
        {
            fireButton.IsEnabled = enabled;
            fireImage.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
            fireImage_d.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PowerButtonEnabled(bool enabled)
        {
            powerButton.IsEnabled = enabled;
            powerImage.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
            powerImage_d.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        }
        */

        /// <summary>
        /// CELLとの通信リクエスト
        /// 通信完了（応答受信またはタイムアウト）でCellRequestDoneコールバックが呼び出される
        /// </summary>
        /// <param name="opeType"></param>
        /// <param name="button"></param>
        private bool RequestCell(CellOperationType.eType opeType, Button button)
        {
            bool bRet;
            //  CELLとの通信開始
            if (true != this.m_monitor.ReqOperation(opeType))
            {
                bRet = false;
                MessageBox.Show(Properties.Resources.ERROR_IN_PROGRESS);
            }
            else
            {
                bRet = true;

                //  処理中設定
                g_currentButtonForCell = button;

                //  ボタンの背景を灰色にする
                UpdateButtonColor(button, null, m_gray);
            }
            return bRet;

        }



        /// <summary>
        /// CELLとの通信完了（応答またはタイムアウト）時によびだされるコールバック
        /// 別のスレッドから呼び出されるので注意
        /// 
        /// ボタンの文字色を黒にする
        /// </summary>
        static void CellRequestDone()
        {
            g_this.Dispatcher.Invoke((Action)(() =>
            {
                //  コントロールの操作
                
                if (g_currentButtonForCell != null)
                {
                    g_this.CellRequestDone(g_currentButtonForCell);

                    g_currentButtonForCell = null;

                }



            }));

        }

        void CellRequestDone(Button button)
        {
            //  ボタンの文字色を黒にする
            Brush brush = Brushes.Black;
            button.Foreground = brush;

            if (button == this.earthquakeButton)
            {
                this.eathquakeText.Foreground = brush;
            } 
            else if(button == this.powerButton)
            {
                this.powerText.Foreground = brush;
            }
            else if (button == this.fireButton)
            {
                this.fireText.Foreground = brush;
            }
        }



    }
}
