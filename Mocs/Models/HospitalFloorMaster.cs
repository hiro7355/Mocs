using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// hospital_fl_idをキー、HospitalMasterを値とするディクショナリ
    /// </summary>
    public class HospitalFloorMasterDictionary : Dictionary<int, HospitalFloorMaster>
    {
        public HospitalFloorMasterDictionary(NpgsqlConnection conn, string sql = "select * from hospital_floor_master")
        {
            this.Load(conn, sql);
        }

        public void Load(NpgsqlConnection conn, string sql)
        {
            HospitalFloorMaster[] rows = BaseModel.GetRows<HospitalFloorMaster>(conn, sql);

            foreach (HospitalFloorMaster row in rows)
            {
                this.Add(row.hospital_fl_id, row);
            }
        }
    }


    /// <summary>
    /// 病棟フロア定義
    /// </summary>
    public class HospitalFloorMaster : BaseModel
    {
        public Int16 hospital_fl_id;        //  病棟フロアID smallint
        public string hospital_fl_name;     //  病棟フロア名称 varchar
        public Int16 hospital_fl_master_id; //  病棟ID    smallint
        public int hospital_fl_point;       //  病棟フロアポイント数 integer
        public Int16 hospital_fl_adoor;     //  病棟フロアオートドア数 smallint
        public Int16 hospital_fl_adoorcnt;  //  病棟フロアオートドア制御方式 smallint
        public int hospital_fl_map_id;      //  病棟フロアマップファイルID  integer

        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.hospital_fl_id = this.getValue<Int16>(dr, "hospital_fl_id");
            this.hospital_fl_name = this.getValue<string>(dr, "hospital_fl_name");
            this.hospital_fl_master_id = this.getValue<Int16>(dr, "hospital_fl_master_id");
            this.hospital_fl_point = this.getValue<int>(dr, "hospital_fl_point");
            this.hospital_fl_adoor = this.getValue<Int16>(dr, "hospital_fl_adoor");
            this.hospital_fl_adoorcnt = this.getValue<Int16>(dr, "hospital_fl_adoorcnt");
            this.hospital_fl_map_id = this.getValue<int>(dr, "hospital_fl_map_id");
        }
    }
}
