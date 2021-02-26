using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// 搬送行先マスタ
    /// </summary>
    public class DestMaster : BaseModel
    {



        public int dest_id; // 搬送行先ID
        public string dest_name_en; //  搬送行先名称(英語)
        public string dest_name_jp;        //  搬送行先名称(日本語)
        public string dest_name_cn;      //   搬送行先名称(中国語)
        public string dest_inspect_sects;   // 搬送行先監査部署
        public string dest_stop_sects;      //  搬送行先立寄り部署
        public Int16 dest_round_flg;          //   搬送行先巡回フラグ


        public override void LoadProp(NpgsqlDataReader dr)
        {

            this.dest_id = this.getValue<int>(dr, "dest_id");
            this.dest_name_en = this.getValue<string>(dr, "dest_name_en");
            this.dest_name_jp = this.getValue<string>(dr, "dest_name_jp");
            this.dest_name_cn = this.getValue<string>(dr, "dest_name_cn");
            this.dest_inspect_sects = this.getValue<string>(dr, "dest_inspect_sects");
            this.dest_stop_sects = this.getValue<string>(dr, "dest_stop_sects");
            this.dest_round_flg = this.getValue<Int16>(dr, "dest_round_flg");
        }

    }
}
