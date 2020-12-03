using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// point_idをキー、PointMasterを値とするディクショナリ
    /// </summary>
    public class PointMasterDictionary : Dictionary<int, PointMaster>
    {
        public PointMasterDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            PointMaster[] rows = BaseModel.GetRows<PointMaster>(conn, "select * from point_master");

            foreach (PointMaster row in rows)
            {
                this.Add(row.point_id, row);
            }
        }
    }


    public class PointMaster : BaseModel
    {
        public int point_id;                // ポイントID  integer
        public string point_name;           //  ポイント名称 varchar
        public string point_cont1;          // ポイント区分1 varchar
        public Int16 point_cont2;           // ポイント区分2 smallint
        public Int16 point_hospital_id;     // 所属病棟ID  smallint
        public Int16 point_hospital_fl_id;  //  所属病棟階 smallint
        public long point_x;                //  ポイントＸ座標 bigint
        public long point_y;                //  ポイントY座標 bigint
        public long point_z;                //  ポイントZ座標 bigint
        public Int16 point_cost;            //   ポイントコスト smallint
        public string point_blocking_id;    //  ポイントブロッキング識別コード varchar
        public string point_connect_id;     //     接続セグメントリスト varchar
        public string point_dwait;          //  デットロック発生時の退避ポイントリスト varchar

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.point_id = this.getValue<int>(dr, "point_id");
            this.point_name = this.getValue<string>(dr, "point_name");
            this.point_cont1 = this.getValue<string>(dr, "point_cont1");
            this.point_cont2 = this.getValue<Int16>(dr, "point_cont2");
            this.point_hospital_id = this.getValue<Int16>(dr, "point_hospital_id");
            this.point_hospital_fl_id = this.getValue<Int16>(dr, "point_hospital_fl_id");
            this.point_x = this.getValue<long>(dr, "point_x");
            this.point_y = this.getValue<long>(dr, "point_y");
            this.point_z = this.getValue<long>(dr, "point_z");
            this.point_cost = this.getValue<Int16>(dr, "point_cost");
            this.point_blocking_id = this.getValue<string>(dr, "point_blocking_id");
            this.point_connect_id = this.getValue<string>(dr, "point_connect_id");
            this.point_dwait = this.getValue<string>(dr, "point_dwait");

        }
    }
}
