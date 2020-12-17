using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    public class MuErrorInfoMaster : BaseModel
    {
        public int mu_errinfo_code;             //  MU異常コード
        public string mu_errinfo_msg;           // MU異常メッセージ
        public string mu_errinfo_msgdetail;     //  MU異常メッセージ詳細

        public int mu_id;   // MU識別子
        public string mu_name;  //  mu_name_en MU名称(英語) mu_name_jp MU名称(日本語) mu_name_cn MU名称(中国語)

        public static string GetSql(string locale_coe, int mu_errinfo_code = 0)
        {
            string sql = "SELECT mu_errinfo_code, mu_errinfo_msg_" + locale_coe + " AS mu_errinfo_msg , mu_errinfo_msgdetail_" + locale_coe + " AS mu_errinfo_msgdetail FROM mu_error_info_master";
            if (mu_errinfo_code != 0)
            {
                sql += " WHERE mu_errinfo_code=" + mu_errinfo_code;
            }
            return sql;
        }


        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.mu_errinfo_code = this.getValue<int>(dr, "mu_errinfo_code");
            this.mu_errinfo_msg = this.getValue<string>(dr, "mu_errinfo_msg");
            this.mu_errinfo_msgdetail = this.getValue<string>(dr, "mu_errinfo_msgdetail");
        }

    }
}
