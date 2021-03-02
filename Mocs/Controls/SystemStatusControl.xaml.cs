using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using Npgsql;

namespace Mocs.Controls
{
 
    /// <summary>
    /// SystemStatusControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemStatusControl : TimerBaseControl
    {
        private const int CELL_STOP_LEVEL = 100;        //  セル停止状態のときのレベル（内部用）


        //  運転状態LED用のメッセージ種別
        private const int INDEX_CELL = 0;
        private const int INDEX_MU = 1;
        private const int INDEX_ERROR = 2;

        //  通信状態LED用のメッセージ種別
        private const int INDEX_DB = 0;
        private const int INDEX_SOCKET_DATA = 1;
        private const int INDEX_SOCKET_CONNECT = 2;

        //  異常状態LED用のメッセージ種別
        private const int INDEX_ERROR_MU = 0;


        // イベントを定義
        public event EventHandler OnCellStatus;

        private int m_last_level;
        private int m_last_status;
        private bool m_is_cell_stopped;

        private bool m_is_socket_connected;     //  コネクション確立チェック用（成功したらもう確認しないようにするため）


        private Brush m_red;
        private Brush m_orange;


        public int GetLastLevel()
        {
            return m_last_level;
        }
        public int GetLastStatus()
        {
            return m_last_status;
        }


        /// <summary>
        /// CELLが停止状態かどうか判別
        /// </summary>
        /// <returns></returns>
        public bool IsCellStopped()
        {
            return m_is_cell_stopped;
        }
        bool m_is_socket_error;              //  ソケット通信エラーが発生したかどうか。リカバリーしたかどうかの判別に利用

        ObservableCollection<MessageInfo> m_messageList;

        Dictionary<int, string> m_mu_id_to_message_for_error; //  mu_idとmessageのディクショナリ（異常状態監視でmuごとにメッセージを表示するときに状態変化があったかどうか判断するのに利用）

        DateTime m_last_update_time;     //  前回の更新時間

        public SystemStatusControl()
        {
            InitializeComponent();


            m_red = Brushes.Red;
            m_orange = Brushes.Orange;


            m_last_level = 0;
            m_is_cell_stopped = false;
            m_is_socket_connected = false;


            // 変更通知してくれるObservableCollectionを使用すると、コレクションに要素を追加・削除すると、自動的にListBoxにも反映します
            m_messageList = new ObservableCollection<MessageInfo>();
            this.messageList.ItemsSource = m_messageList;
           

            ResetMuError();


            //  周辺機器LEDは今回のフェーズでは緑固定とする
            this.device.SetColorFromName("Green");

        }

        private void ResetMuError()
        {
            m_mu_id_to_message_for_error = new Dictionary<int, string>();

        }


        protected override  void Update()
        {

            //  LEDの初期化（いったん黒にする）
            InitLeds();

            //  cell_statusの読み込み。  エラーが発生していた場合はCommonUtil.LastDBErrorにDBエラーメッセージが設定される
            CellStatus cellStatus = CellStatus.GetFirst(m_db.Conn);


            //  通信状態監視
            UpdateNetwork();

            //  異常状態監視
            if (cellStatus != null)
            {
                UpdateError(cellStatus);

                //  運転状態監視 (他のチェックより後にくるように。ブザーの制御がうまくいくようにするため)
                UpdateCell(cellStatus);

            }


            //  親に通知
            OnCellStatus(this, EventArgs.Empty);

        }

        /// <summary>
        /// LEDの色を初期化
        /// </summary>
        private void InitLeds()
        {
            this.cell.SetColorFromName("Black");
            this.network.SetColorFromName("Black");
            this.error.SetColorFromName("Black");
        }

        /// <summary>
        /// 異常状態監視
        /// 
        /// MU の状態を LED とメッセージで表示する。MU 個別の状態(MU 状態テーブル)を監視し、 その状態の変化を表示する。
        /// 但し、システム状態の監視にて cellstat_mu_status = 2or4の場合はLEDのみ赤にする。 また、cellstat_mu_status=1 or 3 の場合、LED を緑にする。
        /// ① cell_statusテーブル参照
        　      /// ・cellstat_mu_statusの内容（仕様書記載）で赤/緑LED点灯
        /// メッセージは、DB設計書の備考欄になります。
        　　    /// 1:オフライン正常(オフラインMUあり、異常MUなし)
        　　    /// 2:オフライン異常(オフラインMUあり、異常MUあり)
        　　    /// 3:オンライン正常(オフラインMUなし、異常MUなし)
        　　    /// 4:オンライン異常(オフラインMUなし、異常MUあり)
        /// cellstat_mu_status =3の場合は処理終了。他は、②へ
        /// ②　mu_statusテーブル参照（有効なmuのみ）
        ///　　・mu有効なレコードを全て参照する。
        /// 　　メッセージの表示形態（１行）は、以下になります。
        ///　　”2020/12/11 16:40:20 mu名称：状態メッセージ”
        ///　　・mu_stat_com =0 の場合
        /// mu_stat_idから mu_masterに登録されているmu名称（言語別）を取得し、
        ///　　　メッセージ＝”mu名称：Wifi未接続です。”
        ///　　・mu_stat_errlevel,mu_stat_errcodeを取得し、異常がある場合は以下の手順で
        /// エラー内容を表示する。
        /// 本プログラムに mu_stat_errlevelとmu_stat_errcodeの組み合わせにより
        /// 表示するメッセージコードを****.confに定義してください。
        ///　　　このメツセージコードから mu_error_info_maste(DB)に定義されている
        /// 異常メッセージのみ表示してください。mu一覧のメッセージでは、エラー詳細も
        /// 表示します。
        /// 　　・異常が無い場合、mu_stat_ope_modeを取得し、オンライン自動状態では無い
        /// muの運転状態を表示します。
        ///　　　”2020/12/11 16:40:20 mu名称：運転状態メッセージ”
        　　///（運転状態メッセージは、DB設計書の備考欄の内容）
        /// </summary>
        private void UpdateError(CellStatus cellStatus)
        {
            DateTime updatedAt = cellStatus.cellstat_stat_update_datetime;
            int mu_status = cellStatus.cellstat_mu_status;

            if (mu_status == 2 || mu_status == 4)
            {
                // cellstat_mu_status = 2or4の場合はLEDのみ赤にする。
                this.UpdateLedAndMessage(updatedAt, this.error, INDEX_ERROR_MU, "Red", "Red", mu_status == 2 ? Properties.Resources.MSG_ERROR_MU_STATUS_2 : Properties.Resources.MSG_ERROR_MU_STATUS_4);
            } 
            else if (mu_status == 1 || mu_status == 3)
            {
                this.UpdateLedAndMessage(updatedAt, this.error, INDEX_ERROR_MU, "Green", "White", mu_status == 1 ? Properties.Resources.MSG_ERROR_MU_STATUS_1 : Properties.Resources.MSG_ERROR_MU_STATUS_3);

                this.ResetMuError();
            }

//            if (mu_status != 3)       <--  検査シートによると3のときもmu_statusをチェックするということなのでこのチェックはしない
            {
                //  muごとの処理
                UpdateErrorForMu();



            }

        }

        /// <summary>
        /// 異常状態チェックで
        /// MUエラーチェック
        /// </summary>
        private void UpdateErrorForMu()
        {
            string locale_code = CommonUtil.GetAppLocaleCode();

            MuStatus[] mu_statuses = BaseModel.GetRows<MuStatus>(m_db.Conn, "SELECT * FROM mu_status WHERE mu_stat_enable=1");

            foreach (MuStatus mu_status in mu_statuses)
            {
                string error_message = null;
                int mu_id = mu_status.mu_stat_id;
                if (mu_status.mu_stat_com == 0)
                {
                    //  Wifi未接続
                    error_message = Properties.Resources.MSG_ERROR_NO_WIFI;

                }
                else if(mu_status.mu_stat_com == 1 && mu_status.mu_stat_errcode != 0)
                {
                    //  異常がある場合はメッセージを取得
                    error_message = CommonUtil.MuErrorMessageFormat(m_db.Conn, locale_code, mu_id, mu_status.mu_stat_errlevel, mu_status.mu_stat_errcode);

                }


                bool do_update = false;
                bool is_success = false;
                //  メッセージに更新があるかチェック
                if (this.m_mu_id_to_message_for_error.ContainsKey(mu_id))
                {
                    if (this.m_mu_id_to_message_for_error[mu_id] != error_message)
                    {
                        do_update = true;

                        if (error_message == null)
                        {
                            //  正常に復帰したとき
                            error_message = Properties.Resources.MSG_ERROR_MU_RECOVER;
                            is_success = true;
                        }
                    }
                }
                else
                {
                    if (error_message != null)
                    {
                        //  エラーが発生している場合のみメッセージ追加
                        do_update = true;
                    }
                }

                if (do_update)
                {
                    MuMaster muMaster = BaseModel.GetFirst<MuMaster>(m_db.Conn, MuMaster.GetSql(locale_code, mu_id));

                    if (muMaster != null)
                    {
                        DateTime updatedAt = MuStatus.GetDateTime(m_db.Conn, mu_status.mu_stat_id, mu_status.mu_stat_errcode);

                        //  メッセージを追加
                        this.addMessage(updatedAt, is_success ? "White" : "Red", error_message, muMaster.mu_name);

                        
                        //  次回更新チェック用にメッセージを保存
                        this.m_mu_id_to_message_for_error[mu_id] = is_success ? null : error_message;
                    }

                }

            }

        }


        /// <summary>
        /// 通信状態監視
        /// </summary>
        private void UpdateNetwork()
        {
            //  エラー情報の通信情報をリセット
            m_errorInfo.ResetCommuInfo();

            int errorTypeDB = 1;
            int errorTypeSocket = 2;



            DateTime updatedAt = DateTime.Now;
            string errorMessage = null;
            //  DBアクセス
            int lastError = CommonUtil.GetLastDBError();
            if (lastError != 0)
            {
                errorMessage = Properties.Resources.DB_ACCESS_ERROR + " " + CommonUtil.DBErrorCodeFormat(lastError);
                //  エラー内容に変化があるときのみメッセージを表示。LEDは常に表示
                this.UpdateLedAndMessage(updatedAt, this.network, INDEX_DB, "Red", "Red", errorMessage);

                m_errorInfo.UpdateCommuError(updatedAt, errorMessage, errorTypeDB);


            }
            else
            {
                //  変化があるとき（初期状態とエラーから復帰したとき）のみメッセージを表示。LEDは常に表示
                this.UpdateLedAndMessage(updatedAt, this.network , INDEX_DB, "Green", "White", Properties.Resources.DB_ACCESS_OK);

            }


            /*

            //  Socket通信コネクション接続
            if (!m_is_socket_connected)
            {
                SysMainTbl sys_main_tbl = m_db.sys_main_tbl;
                string host = sys_main_tbl.cell_ip.Item1.ToString();
                int port = sys_main_tbl.tab_port;
                Socket s = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
                //            Console.WriteLine("BeginConnect" + " ThreadID:" + Thread.CurrentThread.ManagedThreadId);
                s.BeginConnect(host, port,
                    new AsyncCallback(ConnectCallback), s);

            }
            */


            //  SOCKET通信データ通信
            lastError = CommonUtil.GetLastSocketError();
            if (lastError != 0)
            {
                errorMessage = String.Format(Properties.Resources.CELL_SOCKET_ERROR, lastError);
                this.UpdateLedAndMessage(updatedAt, this.network, INDEX_SOCKET_DATA, "Red", "Red", errorMessage);
                m_is_socket_error = true;

                m_errorInfo.UpdateCommuError(updatedAt, errorMessage, errorTypeSocket);


            }
            else
            {
                if (m_is_socket_error)
                {
                    //  エラーから復帰したとき

                    this.UpdateLedAndMessage(updatedAt, this.network, INDEX_SOCKET_DATA, "Green", "White", Properties.Resources.CELL_SOCKET_OK);
                    m_is_socket_error = false;


                }
            }



        }

        /*
        public void ConnectCallback(IAsyncResult ar)
        {
//            Console.WriteLine("ConnectCallback" + " ThreadID:" + Thread.CurrentThread.ManagedThreadId);
            try
            {
                bool isConnected = false;
                // Retrieve the socket from the state object.
                Socket s = (Socket)ar.AsyncState;

                if (s.Connected)
                {
                    //  接続成功
                    // Complete the connection.
                    s.EndConnect(ar);

                    s.Close();

                    isConnected = true;

                }

                m_is_socket_connected = isConnected;

                this.Dispatcher.Invoke((Action)(() =>
                {
                    //  コントロールの操作
                    UpdateNetworkSocketConnection(isConnected);
                }));


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        private void UpdateNetworkSocketConnection(bool isConnected)
        {
            if (isConnected)
            {
                this.UpdateLedAndMessage(this.network, INDEX_SOCKET_CONNECT, "Green", "White", Properties.Resources.CELL_SOCKET_CONNECTION_SUCCESS,Properties.Resources.COMMU);
            } 
            else
            {
                this.UpdateLedAndMessage(this.network, INDEX_SOCKET_CONNECT, "Red", "Red", Properties.Resources.CELL_SOCKET_CONNECTION_ERROR, Properties.Resources.COMMU);
            }
        }

        */



        /// <summary>
        /// 運転状態監視
        /// </summary>
        private CellStatus UpdateCell(CellStatus cellStatus)
        {


            m_errorInfo.ResetCellInfo();

            DateTime updatedAt = cellStatus.cellstat_stat_update_datetime;
            int status = cellStatus.cellstat_status;
            int level = cellStatus.cellstat_level;
            m_last_level = level;
            m_last_status = status;

            DateTime update_time = cellStatus.cellstat_stat_update_datetime;
            int mu_status = cellStatus.cellstat_mu_status;

            string type = "CELL";


            //  mu状態のメッセージ表示
            if (mu_status == 0)
            {
                //  MUの状態を確認しています。
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_MU,  null, "White", Properties.Resources.MSG_CELL_CHECK_MU);
            }
            else if (mu_status == 1)
            {
                //  オフラインMUがあります。
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_MU, null, "White", Properties.Resources.MSG_CELL_MU_OFF);
            }
            else if (mu_status == 2)
            {
                //  オフラインMUと異常のMUがあります。
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_MU, "Red", "Red", Properties.Resources.MSG_CELL_MU_OFF_ERROR);
            }
            else if (mu_status == 3)
            {
                //  MU全てオンライン正常です
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_MU, null, "White", Properties.Resources.MSG_CELL_MU_OK);
            }
            else if (mu_status == 4)
            {
                //  MUで異常が発生しています。
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_MU, "Red", "Red", Properties.Resources.MSG_CELL_MU_ERROR);
            }



            if (status == 0 || status == 10)
            {
                //  statusが0か10のときはCELLは停止している意味
                m_is_cell_stopped = true;

                //  CELLが起動していません
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_CELL, "White", "White", Properties.Resources.MSG_CELL_NOT_RUNNING, type);

                //  異常情報用にCELLエラー情報を設定
                m_errorInfo.UpdateCellError(update_time, Properties.Resources.MSG_CELL_NOT_RUNNING, Properties.Resources.START_CELL, m_orange);


            }
            else if (status == 9)
            {
                //  システム停止（継続不可）しました。
                this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_CELL, "Red", "Red", Properties.Resources.MSG_CELL_SYSTEM_STOPPED, type);

                //  異常情報用にCELLエラー情報を設定
                m_errorInfo.UpdateCellError(update_time, Properties.Resources.MSG_CELL_SYSTEM_STOPPED, Properties.Resources.START_CELL, m_red);

            }
            else if (status == 1)
            {
                //  更新時間によるCELLの停止チェック（このチェックは他のエラーチェックより先に行うこと）
                if (m_last_update_time == update_time)
                {
                    //  CELLの動作が停止しています
                    m_is_cell_stopped = true;

                    this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_CELL, "Red", "Red", Properties.Resources.MSG_CELL_STOPPED, type);

                    //  異常情報用にCELLエラー情報を設定
                    m_errorInfo.UpdateCellError(update_time, Properties.Resources.MSG_CELL_STOPPED, Properties.Resources.START_CELL, m_red);

                }
                else
                {
                    m_is_cell_stopped = false;
                }



                if (level == 1)
                {
                    if (m_last_update_time != update_time)
                    {
                        //  時間に変化がある　－＞　通常運転中です

                        this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_CELL, "Green", "White", Properties.Resources.MSG_CELL_RUNNING, type);

                    }
                }
                else if (level == 2)
                {
                    //  管制運転中（火災）です
                    this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_ERROR, "Red", "Red", Properties.Resources.MSG_CELL_FIRE, type);

                    //  異常情報用にCELLエラー情報を設定
                    m_errorInfo.UpdateCellError(update_time, Properties.Resources.MSG_CELL_FIRE, "", m_red);

                }
                else if (level == 3)
                {
                    //  管制運転中（地震）です
                    this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_ERROR, "Red", "Red", Properties.Resources.MSG_CELL_EARTHQUAKE, type);

                    //  異常情報用にCELLエラー情報を設定
                    m_errorInfo.UpdateCellError(update_time, Properties.Resources.MSG_CELL_EARTHQUAKE, "", m_red);

                }
                else if (level == 4)
                {
                    //  管制運転中（停電）です
                    this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_ERROR, "Red", "Red", Properties.Resources.MSG_CELL_POWER_OUTAGE, type);

                    //  異常情報用にCELLエラー情報を設定
                    m_errorInfo.UpdateCellError(update_time, Properties.Resources.MSG_CELL_POWER_OUTAGE, "", m_red);
                }
                else if (level == 10)
                {
                    if (m_last_update_time != update_time)
                    {
                        //  CELLが正常に停止しています
                        this.UpdateLedAndMessage(updatedAt, this.cell, INDEX_CELL, "Yellow", "White", Properties.Resources.MSG_CELL_SYSTEM_STOPPED_SUCCESSFULLY, type);
                    }
                }

            }


            //  次回確認ように更新時間を設定
            m_last_update_time = update_time;

            return cellStatus;

        }

        /// <summary>
        /// LEDとメッセージを更新
        /// </summary>
        /// <param name="led">LEDコントローラー</param>
        /// <param name="ledColorName">LED色</param>
        /// <param name="bgColorName">文字背景色</param>
        /// <param name="message">メッセージ</param>
        private void UpdateLedAndMessage(DateTime dt, LedControl led, int messageIndex, String ledColorName, String bgColorName, String message, String type = null)
        {

            if (message != null && led.GetLastMessage(messageIndex) != message)
            {
                addMessage(dt, bgColorName, message, type);

                led.SetLastMessage(messageIndex, message);
            }
            if (ledColorName != null)
            {
                led.SetColorFromName(ledColorName);
            }
        }


        private void addMessage(DateTime dt, String bgColorName, String message, String type)
        {
            if (bgColorName == "White")
            {
                bgColorName = "Black";
            }
             
//            m_messageList.Insert(0, new MessageInfo(ColorUtil.brushFromColorName(bgColorName), CommonUtil.MessageFormat(message, type)));
            m_messageList.Insert(0, new MessageInfo(dt, ColorUtil.brushFromColorName(bgColorName), message, type));
        }


        /// <summary>
        /// 最後のメッセージを取得
        /// </summary>
        /// <returns></returns>
        public MessageInfo GetLastMessageInfo()
        {
            MessageInfo info = null;
            if (m_messageList.Count > 0)
            {
                info = m_messageList[0];
            }
            return info;
        }
    }

}
