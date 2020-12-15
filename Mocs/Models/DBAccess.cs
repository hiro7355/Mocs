using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Mocs.Models;
using Mocs.Utils;
using System.Data;

namespace Mocs
{

    public class DBAccess
    {
    	private NpgsqlConnection conn = new NpgsqlConnection();

        private CiLog cErrlog = new CiLog();    // ログ出力クラス

        //  システム定義
        private SysMainTbl Sys_main_tbl;


        //  メニュー
        private MenuScreenDictionary Menu_screen_dictionary;


        //  機能サブメニュー
        private SubmenuScreen[] Function_submenu_screens;

        //  検索サブメニュー
        private SubmenuScreen[] Search_submenu_screens;

        //  履歴サブメニュー
        private SubmenuScreen[] History_submenu_screens;

        //  管理サブメニュー
        private SubmenuScreen[] Manage_submenu_screens;


        //  病棟定義
        private HospitalMasterDictionary Hospital_master_dictionary;

        //  病棟フロア定義
        private HospitalFloorMasterDictionary Hospital_floor_master_dictionary;

        //  搬送ロボットマスタ
        private RobotMasterDictionary Robot_master_dictionary;
        //  有効なロボットのID一覧
        private int[] Robot_ids;

        //  カートマスタ
        private CartMaster[] Cart_masters;

        //  システム画面定義
        private SystemScreen System_screen;

        //  終了確認ダイアログメッセージ
        private FuncMessageDictionary Close_func_message_dictionary;

        //  起動画面メッセージ
        private FuncMessageDictionary Start_func_message_dictionary;

        //  ロボット状態画面メッセージ
        private FuncMessageDictionary Robot_func_message_dictionary;

        //  停止画面メッセージ
        private FuncMessageDictionary Stop_func_message_dictionary;

        //  通信履歴画面メッセージ
        private FuncMessageDictionary Communication_func_message_dictionary;


        //  搬送ロボット状態メッセージマスタ
        private RobotMessageTblDictionary Robot_message_tbl_dictionary;

        //  搬送状態表示メッセージ
        private MovingMessageDictionary Moving_message_dictionary;

        //  ステーションマスタ
        private StationMasterDictionary Station_master_dictionary;

        //  ポイントマスタ
        private PointMasterDictionary Point_master_dictionary;

        //  オーダー処理（オーダー予約）
        private OrderReserveDictionary Order_reserve_dictionary;

        //  システム状態表示メッセージマスタ
        private SystemstatMessageDictionary Systemstat_message_dictionary;

        //  メイン画面ロボット状態表示メッセージ
        private MainRobotmessageDictionary Main_robotmessage_dictionary;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectString"></param>
        public DBAccess(string connectString)
        {
            conn.ConnectionString = connectString;
        }



        /// <summary>
        /// 最初のDB読み込み
        /// </summary>
        public int InitialRead()
        {
            int ret = 0;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                // エラーログ出力
                cErrlog.WriteLog(ex.Message.ToString(), ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                return 1;
            }
            //システム定義読み込み
            this.Sys_main_tbl = BaseModel.GetFirst<SysMainTbl>(conn, "select * from sys_main_tbl");

            //  メニュー読み込み
            //            this.Menu_screen_dictionary = new MenuScreenDictionary(conn);

            /*
            //  機能サブメニュー読み込み
            this.Function_submenu_screens = BaseModel.GetRows<SubmenuScreen>(conn, getSubmenuRowsSql(Const.Menu.FUNCTION));

            //  検索サブメニュー読み込み
            this.Search_submenu_screens = BaseModel.GetRows<SubmenuScreen>(conn, getSubmenuRowsSql(Const.Menu.SEARCH));

            //  履歴サブメニュー読み込み
            this.History_submenu_screens = BaseModel.GetRows<SubmenuScreen>(conn, getSubmenuRowsSql(Const.Menu.HISTORY));

            //  管理サブメニュー読み込み
            this.Manage_submenu_screens = BaseModel.GetRows<SubmenuScreen>(conn, getSubmenuRowsSql(Const.Menu.MANAGE));

            //  病棟定義読み込み
            this.Hospital_master_dictionary = new HospitalMasterDictionary(conn);

            //  病棟フロア定義読み込み
            this.Hospital_floor_master_dictionary = new HospitalFloorMasterDictionary(conn);

            //  搬送ロボットマスタ読み込み (activeなロボットのみ取得)
            this.Robot_master_dictionary = new RobotMasterDictionary(conn);

            //  ロボットID配列を設定
            this.Robot_ids = new int[this.Robot_master_dictionary.Keys.Count];
            this.Robot_master_dictionary.Keys.CopyTo(this.Robot_ids, 0);

            */

            //  カートマスタ読み込み
            this.Cart_masters = BaseModel.GetRows<CartMaster>(conn, "select * from cart_master");


            /*
            //  システム画面定義読み込み
            this.System_screen = BaseModel.GetFirst<SystemScreen>(conn, "select * from system_screen");

            //  終了確認ダイアログメッセージ読み込み (sys_menu_screen_id = 0, submenu_type = 1)
            this.Close_func_message_dictionary = new FuncMessageDictionary(conn, 0, 1);

            //  起動画面メッセージ読み込み
            this.Start_func_message_dictionary = new FuncMessageDictionary(conn, Const.Menu.FUNCTION, Const.FunctionMenu.START);

            //  ロボット状態画面メッセージ読み込み
            this.Robot_func_message_dictionary = new FuncMessageDictionary(conn, Const.Menu.FUNCTION, Const.FunctionMenu.STATUS);

            //  停止画面メッセージ読み込み
            this.Stop_func_message_dictionary = new FuncMessageDictionary(conn, Const.Menu.FUNCTION, Const.FunctionMenu.STOP);

            //  通信履歴画面メッセージ読み込み
            this.Communication_func_message_dictionary = new FuncMessageDictionary(conn, Const.Menu.HISTORY, 5);


            //  搬送ロボット状態メッセージマスタ読み込み
            this.Robot_message_tbl_dictionary = new RobotMessageTblDictionary(conn);

            //  搬送状態表示メッセージ読み込み(level 0 のメッセージのみ読み込む)
            this.Moving_message_dictionary = new MovingMessageDictionary(conn, Const.MovingMessageLevel.DEFAULT);

            */


            //  ステーションマスタ読み込み
//            this.Station_master_dictionary = new StationMasterDictionary(conn);

            //  ポイントマスタ読み込み
//            this.Point_master_dictionary = new PointMasterDictionary(conn);

            //  オーダー処理（オーダー予約）読み込み
//            this.Order_reserve_dictionary = new OrderReserveDictionary(conn);

            //  システム状態表示メッセージマスタ読み込み 
//            this.Systemstat_message_dictionary = new SystemstatMessageDictionary(conn);

            //  メイン画面ロボット状態表示メッセージ読み込み (level 0 のメッセージのみ読み込む)
//            this.Main_robotmessage_dictionary = new MainRobotmessageDictionary(conn, Const.MainRobotmessageLebel.DEFAULT);

            return ret;
        }

        /// <summary>
        /// サブメニューを取得するSQL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string getSubmenuRowsSql(int id)
        {
            return "select * from submenu_screen where menu_screen_tbl_id=" + id + " order by submenu_number";
        }


        public DataTable getDataTable(string sql)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, this.conn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            return ds.Tables[0];
        }

        public  void execute(string sql)
        {
            NpgsqlConnection conn = this.conn;

            using (var transaction = conn.BeginTransaction())
            {
                var command = new NpgsqlCommand(sql, conn);

                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (NpgsqlException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public NpgsqlConnection Conn { get { return conn; } }

        public SysMainTbl sys_main_tbl { get { return Sys_main_tbl; } }

        public SystemScreen system_screen { get { return System_screen; } }

        public MenuScreenDictionary menu_screen_dictionary { get { return Menu_screen_dictionary; } }

        //  機能サブメニュー
        public SubmenuScreen[] function_submenu_screens { get { return Function_submenu_screens; } }

        //  検索サブメニュー
        public SubmenuScreen[] search_submenu_screens { get { return Search_submenu_screens; } }

        //  履歴サブメニュー
        public SubmenuScreen[] history_submenu_screens { get { return History_submenu_screens; } }

        //  管理サブメニュー
        public SubmenuScreen[] manage_submenu_screens { get { return Manage_submenu_screens; } }


        //  有効なロボットマスター
        public RobotMasterDictionary robot_master_dictionary { get { return Robot_master_dictionary; } }


        //  終了確認ダイアログメッセージ
        public FuncMessageDictionary close_func_message_dictionary { get { return Close_func_message_dictionary; } }

        //  起動画面メッセージ
        public FuncMessageDictionary start_func_message_dictionary { get { return Start_func_message_dictionary; } }

        //  ロボット状態画面メッセージ
        public FuncMessageDictionary robot_func_message_dictionary { get { return Robot_func_message_dictionary; } }

        //  停止画面メッセージ
        public FuncMessageDictionary stop_func_message_dictionary { get { return Stop_func_message_dictionary; } }

        //  通信履歴画面メッセージ
        public FuncMessageDictionary communication_func_message_dictionary { get { return Communication_func_message_dictionary; } }


        //  病棟定義
        public HospitalMasterDictionary hospital_master_dictionary { get { return Hospital_master_dictionary; } }

        //  病棟フロア定義
        public HospitalFloorMasterDictionary hospital_floor_master_dictionary { get { return Hospital_floor_master_dictionary; } }

        //  搬送ロボット状態メッセージマスタ
        public RobotMessageTblDictionary robot_message_tbl_dictionary { get { return Robot_message_tbl_dictionary; } }

        //  搬送状態表示メッセージ
        public MovingMessageDictionary moving_message_dictionary { get { return Moving_message_dictionary; } }

        //  ステーションマスタ
        public StationMasterDictionary station_master_dictionary { get { return Station_master_dictionary; } }

        //  ポイントマスタ
        public PointMasterDictionary point_master_dictionary { get { return Point_master_dictionary; } }

        //  オーダー処理（オーダー予約）
        public OrderReserveDictionary order_reserve_dictionary { get { return Order_reserve_dictionary; } }

        //  システム状態表示メッセージマスタ
        public SystemstatMessageDictionary systemstat_message_dictionary { get { return Systemstat_message_dictionary; } }

        //  メイン画面ロボット状態表示メッセージ
        public MainRobotmessageDictionary main_robotmessage_dictionary { get { return Main_robotmessage_dictionary; } }

        //  有効なロボットのID一覧
        public int[] robot_ids { get { return Robot_ids; } }

    }
}
