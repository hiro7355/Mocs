using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForFloorList
    {
        public static string GetListSql(string hospital_id)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
                "SELECT" +
                     " hospital_name_jp AS hospital" +
                     ",floor_name_jp AS floor" +
                     ",(SELECT ARRAY_TO_STRING(ARRAY_AGG(mu_master.mu_name_jp), ',')" +
                         " FROM mu_status" +
                         " LEFT JOIN mu_master ON mu_status.mu_stat_id = mu_master.mu_id" +
                          " WHERE mu_stat_order_status = 1 AND mu_stat_floor_id = floor_master.floor_id) AS mu_order" +
                    ",(SELECT ARRAY_TO_STRING(ARRAY_AGG(mu_master.mu_name_jp), ',')" +
                         " FROM mu_status" +
                         " LEFT JOIN mu_master ON mu_status.mu_stat_id = mu_master.mu_id" +
                          " WHERE mu_stat_order_status = 0 AND mu_stat_floor_id = floor_master.floor_id) AS mu_error" +
                " FROM hospital_master" +
                " LEFT JOIN floor_master ON hospital_master.hospital_id = floor_master.floor_hospital_id";
            if (hospital_id != "0")
            {
                sql += " WHERE hospital_master.hospital_id = " + hospital_id;
            }
            sql += " ORDER BY hospital_master.hospital_id ASC";
            return sql;

        }
    }
}
