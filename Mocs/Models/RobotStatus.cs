using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    public class RobotStatus : BaseModel
    {
        public int robotst_master_id;       //  搬送ロボット識別子   integer
        public string robotst_name;         //    搬送ロボット名称 varchar
        public int robotst_status_level;    // 状態レベル   integer
        public int robotst_status_code;     //  状態コード integer
        public int robotst_cart_code;       //  カート載荷状態&識別コード integer
        public int robotst_order_code;      // オーダコード  varchar
        public int robotst_kind_code;       //   オーダ搬送区分 integer
        public int robotst_order_status;    //  オーダ処理ステータス  integer
        public Int16 robotst_hospital_id;   // ロボット位置病棟ID smallint
        public Int16 robotst_hospital_fr_id;//  ロボット位置病棟階   smallint
        public long robotst_pos_x;          //   ロボット位置X座標 bigint
        public long robotst_pos_y;          //  ロボット位置Y座標   bigint
        public long robotst_pos_z;          //   ロボット位置Z座標 bigint
        public long robotst_pos_t;          //  ロボット位置T座標   bigint
        public int robotst_point_now;       //   ロボット最新ポイント識別子 integer
        public int robotst_point_bef;       // ロボット前ポイント識別子    integer
        public int robotst_topoint;         //  同一階移動Toポイント識別子 integer
        public int robotst_toendpoint;      //  最終移動Toポイント識別子   integer

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.robotst_master_id = this.getValue<int>(dr, "robotst_master_id");
            this.robotst_name = this.getValue<string>(dr, "robotst_name");
            this.robotst_status_level = this.getValue<int>(dr, "robotst_status_level");
            this.robotst_status_code = this.getValue<int>(dr, "robotst_status_code");
            this.robotst_cart_code = this.getValue<int>(dr, "robotst_cart_code");
            this.robotst_order_code = this.getValue<int>(dr, "robotst_order_code");
            this.robotst_kind_code = this.getValue<int>(dr, "robotst_kind_code");
            this.robotst_order_status = this.getValue<int>(dr, "robotst_order_status");
            this.robotst_hospital_id = this.getValue<Int16>(dr, "robotst_hospital_id");
            this.robotst_hospital_fr_id = this.getValue<Int16>(dr, "robotst_hospital_fr_id");
            this.robotst_pos_x = this.getValue<long>(dr, "robotst_pos_x");
            this.robotst_pos_y = this.getValue<long>(dr, "robotst_pos_y");
            this.robotst_pos_z = this.getValue<long>(dr, "robotst_pos_z");
            this.robotst_pos_t = this.getValue<long>(dr, "robotst_pos_t");
            this.robotst_point_now = this.getValue<int>(dr, "robotst_point_now");
            this.robotst_point_bef = this.getValue<int>(dr, "robotst_point_bef");
            this.robotst_topoint = this.getValue<int>(dr, "robotst_topoint");
            this.robotst_toendpoint = this.getValue<int>(dr, "robotst_toendpoint");
        }

        /// <summary>
        /// ロボットidを指定してステータスを取得
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static RobotStatus GetFirst(NpgsqlConnection conn, int id)
        {
            return GetFirst<RobotStatus>(conn, "select * from robot_status where robotst_master_id=" + id);
        }

        /// <summary>
        /// レベルと有効なロボットIDを指定してステータス一覧を取得
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="level"></param>
        /// <param name="activeIds"></param>
        /// <returns></returns>
        public static RobotStatus[] GetRowsByLevel(NpgsqlConnection conn, int level, int code, int[] activeIds)
        {
            string ids = String.Join(",", activeIds);
            return GetRows<RobotStatus>(conn, "select * from robot_status where robotst_master_id in (" + ids + ") and robotst_status_level=" + level + " and robotst_status_code=" + code);
        }


    }
}
