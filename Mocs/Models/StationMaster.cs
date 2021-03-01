using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Net;

namespace Mocs.Models
{
    /// <summary>
    /// station_idをキー、StationMasterを値とするディクショナリ
    /// </summary>
    public class StationMasterDictionary : Dictionary<int, StationMaster>
    {
        public StationMasterDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            StationMaster[] rows = BaseModel.GetRows<StationMaster>(conn, "select * from station_master");

            foreach (StationMaster row in rows)
            {
                this.Add(row.station_id, row);
            }
        }
    }

    public class StationMaster : BaseModel
    {
        public int station_id;  // ステーションID    integer
        public string station_name;     //    ステーション名称 varchar


        /// <summary>
        /// IDから名前を取得するSQL文を取得
        /// </summary>
        /// <param name="localeCode"></param>
        /// <param name="id_field_name"></param>
        /// <returns></returns>
        internal static string SelectNameSql(string localeCode, string id_field_name)
        {
            return "SELECT station_name_" + localeCode + " FROM station_master WHERE station_id=" + id_field_name;
        }
        internal static string SelectNameByPointSql(string localeCode, string id_field_name)
        {
            return "SELECT station_name_" + localeCode + " FROM station_master WHERE station_point_id1=" + id_field_name;
        }
        /// <summary>
        /// 複数行の名前をカンマ区切りで取得するSQL。取得する行のidは配列で指定
        /// </summary>
        /// <param name="localeCode"></param>
        /// <param name="ids">カンマ区切りのid一覧</param>
        /// <returns></returns>
        internal static string SelectNamesSql(string localeCode, string ids)
        {
            return BaseModel.GetNamesSql("station_name_" + localeCode, "station_master", "station_id", ids);
        }

        internal static string SelectNamesByPointSql(string localeCode, string ids)
        {
            return BaseModel.GetNamesSql("station_name_" + localeCode, "station_master", "station_point_id1", ids);
        }

        public string station_kind1;    //  ステーション種別1   varchar
        public string station_kind2;    //   ステーション種別2 varchar
        public int point_master_id1;    //	"ポイント識別コード（正方向、何もしない）"	integer
        public int point_master_id2;    //	"ポイントID（逆方向、何もしない）"	integer
        public int point_master_id3;    //	"ポイントID（正方向、荷積み）"	integer
        public int point_master_id4;    //	"ポイントID（逆方向、荷積み）"	integer
        public int point_master_id5;    //	"ポイントID（正方向、荷降ろし）"	integer
        public int point_master_id6;    //	"ポイントID（逆方向、荷降ろし）"	integer
        public int point_master_id7;    // "ポイントID（正方向、停止）"	integer
        public int point_master_id8;    //	"ポイントID（逆方向、停止）"	integer
        public int point_master_id9;    //	"ポイントID（正方向、予備）"	integer
        public int point_master_id10;   //	"ポイントID（逆方向、予備）"	integer
        public Int16 hospital_master_id;  // 所属病棟ID  smallint
        public Int16 hospital_fl_master_id; // 所属病棟階 smallint
        public ValueTuple<IPAddress, int> station_ipaddr;  // ステーションIPｱﾄﾞﾚｽ      cidr
        public int station_port1;       //   通信ポート番号(正）	integer
        public int station_port2;       //   通信ポート番号(副）	integer

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.station_id = this.getValue<int>(dr, "station_id");
            this.station_name = this.getValue<string>(dr, "station_name");
            this.station_kind1 = this.getValue<string>(dr, "station_kind1");
            this.station_kind2 = this.getValue<string>(dr, "station_kind2");
            this.point_master_id1 = this.getValue<int>(dr, "point_master_id1");
            this.point_master_id2 = this.getValue<int>(dr, "point_master_id2");
            this.point_master_id3 = this.getValue<int>(dr, "point_master_id3");
            this.point_master_id4 = this.getValue<int>(dr, "point_master_id4");
            this.point_master_id5 = this.getValue<int>(dr, "point_master_id5");
            this.point_master_id6 = this.getValue<int>(dr, "point_master_id6");
            this.point_master_id7 = this.getValue<int>(dr, "point_master_id7");
            this.point_master_id8 = this.getValue<int>(dr, "point_master_id8");
            this.point_master_id9 = this.getValue<int>(dr, "point_master_id9");
            this.point_master_id10 = this.getValue<int>(dr, "point_master_id10");
            this.hospital_master_id = this.getValue<Int16>(dr, "hospital_master_id");
            this.hospital_fl_master_id = this.getValue<Int16>(dr, "hospital_fl_master_id");
            this.station_ipaddr = this.getValue<ValueTuple<IPAddress, int>>(dr, "station_ipaddr");
            this.station_port1 = this.getValue<int>(dr, "station_port1");
            this.station_port2 = this.getValue<int>(dr, "station_port2");
        }
    }
}
