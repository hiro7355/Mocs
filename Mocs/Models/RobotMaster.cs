using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Net;
using NpgsqlTypes;

namespace Mocs.Models
{
    /// <summary>
    /// point_idをキー、PointMasterを値とするディクショナリ
    /// </summary>
    public class RobotMasterDictionary : Dictionary<int, RobotMaster>
    {
        public RobotMasterDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            RobotMaster[] rows = BaseModel.GetRows<RobotMaster>(conn, "select * from robot_master where robot_active=1");

            foreach (RobotMaster row in rows)
            {
                this.Add(row.robot_id, row);
            }
        }
    }


    /// <summary>
    /// 搬送ロボットマスタ
    /// </summary>
    public class RobotMaster : BaseModel
    {
        public int robot_id;                        //  搬送ロボット識別子   integer
        public string robot_name;                   //   搬送ロボット名称 varchar
        public Int16 robot_active;                  //  搬送ロボット有効/無効 smallint
        public ValueTuple<IPAddress,int> robot_ipqaddr1;  //  搬送ロボットIPｱﾄﾞﾚｽ1  cidr
        public ValueTuple<IPAddress, int>robot_ipqaddr2;  //  搬送ロボットIPｱﾄﾞﾚｽ2 cidr
        public string robot_data1;                  //  搬送ロボット定義データ1 varchar
        public string robot_data2;                  //  搬送ロボット定義データ2    varchar
        public string robot_data3;                  //  搬送ロボット定義データ3 varchar
        public string robot_data4;                  //  搬送ロボット定義データ4    varchar
        public string robot_data5;                  //  搬送ロボット定義データ5 varchar
        public string robot_data6;                  //  搬送ロボット定義データ6    varchar
        public string robot_data7;                  //  搬送ロボット定義データ7 varchar
        public string robot_data8;                  //  搬送ロボット定義データ8    varchar
        public string robot_data9;                  //  搬送ロボット定義データ9 varchar
        public string robot_data10;                 //  搬送ロボット定義データ10   varchar


        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.robot_id = this.getValue<int>(dr, "robot_id");
            this.robot_name = this.getValue<string>(dr, "robot_name");
            this.robot_active = this.getValue<Int16>(dr, "robot_active");
            this.robot_ipqaddr1 = this.getValue<ValueTuple<IPAddress, int>>(dr, "robot_ipqaddr1");
            this.robot_ipqaddr2 = this.getValue<ValueTuple<IPAddress, int>>(dr, "robot_ipqaddr2");

            this.robot_data1 = this.getValue<string>(dr, "robot_data1");
            this.robot_data2 = this.getValue<string>(dr, "robot_data2");
            this.robot_data3 = this.getValue<string>(dr, "robot_data3");
            this.robot_data4 = this.getValue<string>(dr, "robot_data4");
            this.robot_data5 = this.getValue<string>(dr, "robot_data5");
            this.robot_data6 = this.getValue<string>(dr, "robot_data6");
            this.robot_data7 = this.getValue<string>(dr, "robot_data7");
            this.robot_data8 = this.getValue<string>(dr, "robot_data8");
            this.robot_data9 = this.getValue<string>(dr, "robot_data9");
            this.robot_data10 = this.getValue<string>(dr, "robot_data10");
        }

        /// <summary>
        /// ディアクティブに設定
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="robot_id"></param>
        public static void setDeactive(NpgsqlConnection conn, int robot_id)
        {
            string sql = @"UPDATE robot_master SET robot_active=0 WHERE robot_id=" + robot_id;
            update(conn, sql);
        }

        /// <summary>
        /// すべての行を取得
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static RobotMaster[] getAll(NpgsqlConnection conn)
        {
            return BaseModel.GetRows<RobotMaster>(conn, "select * from robot_master order by robot_id");
        }


        /// <summary>
        /// コンボボックスの表示用
        /// ロボット名を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.robot_name;
        }

    }
}
