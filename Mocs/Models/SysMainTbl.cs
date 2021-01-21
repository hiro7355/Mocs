using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Mocs.Models
{
    /// <summary>
    /// システム定義
    /// </summary>
    public class SysMainTbl : BaseModel
    {
        public Int16 sys_id;    // システムID  smallint
        public string login_name;   //   システムログイン名 varchar
        public string login_pass;   // システムログインパス  varchar
        public string system_name_en;   //  システム名称(英語)  varchar
        public string system_name_jp;   //  システム名称(日本語) varchar
        public string system_name_cn;   //  システム名称(中国語) varchar
        public string hospital_name_en;   //    病院名(英語) varchar
        public string hospital_name_jp;   //    病院名(日本語)    varchar
        public string hospital_name_cn;   //    病院名(中国語)    varchar
        public ValueTuple<IPAddress, int> cell_ip; //  Cell IPアドレス cidr
        public int mu_port1; //    MU通信ポート番号(主）	integer
        public int mu_port2; //    MU通信ポート番号(副）	integer
        public Int16 mu_mon_period; //   MU通信接続監視間隔 smallint
        public Int16 mu_kalive_time1; // MU通信Keep-Alive監視時間(正ｿｹｯﾄ）	smallint
        public Int16 mu_kalive_time2; // MU通信Keep-Alive監視時間(副ｿｹｯﾄ）	smallint
        public Int16 mu_comlog_flg; //   MU通信ログ出力有効/無効フラグ smallint
        public int tab_port; // タブレット、監視モニター通信受付ポート(Cell側) integer
        public int tab_term_port; // タブレット、監視モニター通信受付ポート(タブレット、監視モニター側)  integer
        public Int16 tab_comlog_flg; //  タブレット、監視モニター通信ログ出力有効/無効フラグ smallint
        public Int16 proc_after_order; // オーダー完了後処理設定 smallint
        public int order_inspect_resume_timeout; //    監査搬送再開待ちタイムオーバー integer
        public int order_stop_resume_timeout; // 経由地搬送再開待ちタイムオーバー    integer
        public int routelog_flg; //    ルートシナリオログ有効・無効 smallint
        public Int16 io_ctrl_type; // I/O制御方式 smallint
        public ValueTuple<IPAddress, int> io_ctrl_ip; // I/O制御通信IPアドレス cidr
        public int io_ctrl_port; // I/O制御通信ポート番号 integer
        public string mu_config_ver; // MU動作設定ファイル版数    varchar
        public string mu_config_filename; //  MU動作設定ファイル名 varchar
        public string slam_data_ver; // SLAMデータファイル版数   varchar
        public string slam_data_filename; //  SLAMデータファイル名 varchar
        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {

        this.sys_id = this.getValue<Int16>(dr, "sys_id");    // システムID  smallint
            this.login_name = this.getValue<string>(dr, "login_name");   //   システムログイン名 varchar
            this.login_pass = this.getValue<string>(dr, "login_pass");   // システムログインパス  varchar
            this.system_name_en = this.getValue<string>(dr, "system_name_en");   //  システム名称(英語)  varchar
            this.system_name_jp = this.getValue<string>(dr, "system_name_jp");   //  システム名称(日本語) varchar
            this.system_name_cn = this.getValue<string>(dr, "system_name_cn");   //  システム名称(中国語) varchar
            this.hospital_name_en = this.getValue<string>(dr, "hospital_name_en");   //    病院名(英語) varchar
            this.hospital_name_jp = this.getValue<string>(dr, "hospital_name_jp");   //    病院名(日本語)    varchar
            this.hospital_name_cn = this.getValue<string>(dr, "hospital_name_cn");   //    病院名(中国語)    varchar
            this.cell_ip = this.getValue <ValueTuple<IPAddress, int>>(dr, "cell_ip"); //  Cell IPアドレス cidr
            this.mu_port1 = this.getValue<int>(dr, "mu_port1"); //    MU通信ポート番号(主）	integer
            this.mu_port2 = this.getValue<int>(dr, "mu_port2"); //    MU通信ポート番号(副）	integer
            this.mu_mon_period = this.getValue<Int16>(dr, "mu_mon_period"); //   MU通信接続監視間隔 smallint
            this.mu_kalive_time1 = this.getValue<Int16>(dr, "mu_kalive_time1"); // MU通信Keep-Alive監視時間(正ｿｹｯﾄ）	smallint
            this.mu_kalive_time2 = this.getValue<Int16>(dr, "mu_kalive_time2"); // MU通信Keep-Alive監視時間(副ｿｹｯﾄ）	smallint
            this.mu_comlog_flg = this.getValue<Int16>(dr, "mu_comlog_flg"); //   MU通信ログ出力有効/無効フラグ smallint
            this.tab_port = this.getValue<int>(dr, "tab_port"); // タブレット、監視モニター通信受付ポート(Cell側) integer
            this.tab_term_port = this.getValue<int>(dr, "tab_term_port"); // タブレット、監視モニター通信受付ポート(タブレット、監視モニター側)  integer
            this.tab_comlog_flg = this.getValue<Int16>(dr, "tab_comlog_flg"); //  タブレット、監視モニター通信ログ出力有効/無効フラグ smallint
            this.proc_after_order = this.getValue<Int16>(dr, "proc_after_order"); // オーダー完了後処理設定 smallint
            this.order_inspect_resume_timeout = this.getValue<int>(dr, "order_inspect_resume_timeout"); //    監査搬送再開待ちタイムオーバー integer
            this.order_stop_resume_timeout = this.getValue<int>(dr, "order_stop_resume_timeout"); // 経由地搬送再開待ちタイムオーバー    integer
            this.routelog_flg = this.getValue<int>(dr, "routelog_flg"); //    ルートシナリオログ有効・無効 smallint
            this.io_ctrl_type = this.getValue<Int16>(dr, "io_ctrl_type"); // I/O制御方式 smallint
            this.io_ctrl_ip = this.getValue<ValueTuple<IPAddress, int>>(dr, "io_ctrl_ip"); // I/O制御通信IPアドレス cidr
            this.io_ctrl_port = this.getValue<int>(dr, "io_ctrl_port"); // I/O制御通信ポート番号 integer
            this.mu_config_ver = this.getValue<string>(dr, "mu_config_ver"); // MU動作設定ファイル版数    varchar
            this.mu_config_filename = this.getValue<string>(dr, "mu_config_filename"); //  MU動作設定ファイル名 varchar
            this.slam_data_ver = this.getValue<string>(dr, "slam_data_ver"); // SLAMデータファイル版数   varchar
            this.slam_data_filename = this.getValue<string>(dr, "slam_data_filename"); //  SLAMデータファイル名 varchar

        }


        /*
        public string language_type;//言語タイプ -- JN:日本語  EN:英語  CH:北京語  CK:広東語
        public string login_name;   //   システムログイン名
        public string login_pass;   //  システムログインパス

        public string system_name;  //システム名称 -- 製造元の管理名称（ログで使用）
        public string hospital_name;//病院名 -- 製造元の管理病院名（ログで使用）

        public Int16 building_su;     //  建物病棟数 smallint
        public Int16 robot_su;        //  登録搬送ロボット数 smallint
        public int   robot_port1;     //  通信ポート番号(主）	integer
        public int   robot_port2;     //  通信ポート番号(副）	integer
        public Int16 robot_monitor;   //   搬送ロボット監視間隔 smallint
        public Int16 robot_keep1;     //   搬送ロボット通信Keep-Alive監視時間(正ｿｹｯﾄ）	smallint
        public Int16 robot_keep2;     //   搬送ロボット通信Keep-Alive監視時間(副ｿｹｯﾄ）	smallint
        public Int16 cart_su;         //   登録カート数 smallint
        public Int16 comlog_flg;      //   搬送ロボット通信ログ出力    smallint
        public Int16 cycle_charge;    //    サイクル充電の有効・無効 smallint
        public Int16 order_endchg;    //   オーダ処理完了待機設定 smallint
        public Int16 cansel_are;      //    キャンセル受付フラグ smallint
        public Int16 routelog_flg;    //  ルートシナリオログ有効・無効 smallint


        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.language_type = this.getValue<string>(dr, "language_type");   //言語タイプ -- JN:日本語  EN:英語  CH:北京語  CK:広東語
            this.login_name = this.getValue<string>(dr, "login_name");         //   システムログイン名
            this.login_pass = this.getValue<string>(dr, "login_pass");         //  システムログインパス
            this.system_name = this.getValue<string>(dr, "system_name");       //システム名称 -- 製造元の管理名称（ログで使用）
            this.hospital_name = this.getValue<string>(dr, "hospital_name");   //病院名 -- 製造元の管理病院名（ログで使用）
            this.building_su = this.getValue<Int16>(dr, "building_su");        //  建物病棟数 smallint
            this.robot_su = this.getValue<Int16>(dr, "robot_su");              //  登録搬送ロボット数 smallint
            this.robot_port1 = this.getValue<int>(dr, "robot_port1");
            this.robot_port2 = this.getValue<int>(dr, "robot_port2");
            this.robot_monitor = this.getValue<Int16>(dr, "robot_monitor");    //   搬送ロボット監視間隔 smallint
            this.robot_keep1 = this.getValue<Int16>(dr, "robot_keep1");        //   搬送ロボット通信Keep-Alive監視時間(正ｿｹｯﾄ）	smallint
            this.robot_keep2 = this.getValue<Int16>(dr, "robot_keep2");        //   搬送ロボット通信Keep-Alive監視時間(副ｿｹｯﾄ）	smallint
            this.cart_su = this.getValue<Int16>(dr, "cart_su");                //   登録カート数 smallint
            this.comlog_flg = this.getValue<Int16>(dr, "comlog_flg");          //   搬送ロボット通信ログ出力    smallint
            this.cycle_charge = this.getValue<Int16>(dr, "cycle_charge");      //    サイクル充電の有効・無効 smallint
            this.order_endchg = this.getValue<Int16>(dr, "order_endchg");      //   オーダ処理完了待機設定 smallint
            this.cansel_are = this.getValue<Int16>(dr, "cansel_are");          //    キャンセル受付フラグ smallint
            this.routelog_flg = this.getValue<Int16>(dr, "routelog_flg");      //  ルートシナリオログ有効・無効 smallint

        }
        */

    }
}
