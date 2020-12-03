using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    /// <summary>
    /// システム定義
    /// </summary>
    public class SysMainTbl : BaseModel
    {
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

    }
}
