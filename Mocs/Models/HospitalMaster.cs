using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{

    /// <summary>
    /// hospital_idをキー、HospitalMasterを値とするディクショナリ
    /// </summary>
    public class HospitalMasterDictionary : Dictionary<int, HospitalMaster>
    {
        public HospitalMasterDictionary(NpgsqlConnection conn, string sql = "select * from hospital_master")
        {
            this.Load(conn, sql);
        }

        public void Load(NpgsqlConnection conn, string sql)
        {
            HospitalMaster[] rows = BaseModel.GetRows<HospitalMaster>(conn, sql);

            foreach (HospitalMaster row in rows)
            {
                this.Add(row.hospital_id, row);
            }
        }
    }


    /// <summary>
    /// 病棟定義
    /// </summary>
    public class HospitalMaster : BaseModel
    {

        public Int16 hospital_id;   //  病棟ID    smallint
        public string hospital_name;    //  病棟名称（南館、新館等）	varchar
        public Int16 hospital_ugfloor;  //  病棟地下フロア数（地下階) Max5 smallint
        public Int16 hospital_upfloor;  //  病棟地上フロア数    smallint
        public Int16 hospital_ele_su;   //  病棟搬送用エレベータ数 smallint

        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.hospital_id = this.getValue<Int16>(dr, "hospital_id");
            this.hospital_name = this.getValue<string>(dr, "hospital_name");
            this.hospital_ugfloor = this.getValue<Int16>(dr, "hospital_ugfloor");
            this.hospital_upfloor = this.getValue<Int16>(dr, "hospital_upfloor");
            this.hospital_ele_su = this.getValue<Int16>(dr, "hospital_ele_su");
        }
    }
}
