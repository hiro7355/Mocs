using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    public class MuMaster : BaseModel
    {
        /// <summary>
        /// IDから名前を取得するSQL文を取得
        /// </summary>
        /// <param name="localeCode"></param>
        /// <param name="id_field_name"></param>
        /// <returns></returns>
        internal static string SelectNameSql(string localeCode, string id_field_name)
        {
            return "SELECT mu_name_" + localeCode + " FROM mu_master WHERE mu_id=" + id_field_name;
        }

        public int mu_id;   // MU識別子
        public string mu_name;  //  mu_name_en MU名称(英語) mu_name_jp MU名称(日本語) mu_name_cn MU名称(中国語)

        public static string GetSql(string locale_coe, int mu_id = 0)
        {
             string sql = "SELECT mu_id, mu_name_" + locale_coe + " AS mu_name FROM mu_master";
            if (mu_id != 0)
            {
                sql += " WHERE mu_id=" + mu_id;
            }
            return sql;
        }


        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.mu_id = this.getValue<int>(dr, "mu_id");
            this.mu_name = this.getValue<string>(dr, "mu_name");
        }

    }
}
