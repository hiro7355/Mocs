using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    public class SqlForMuList
    {

        public static string GetListSql()
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
            "SELECT" +
                " mu_name_" + localeCode + " AS name" +
                ",concat_ws(',', mu_stat_com, mu_stat_ope_mode) AS ope_mode" +
                ", CASE WHEN (mu_stat_com = 0 OR mu_stat_ope_mode = 0) THEN '' ELSE hospital_name_" + localeCode + " || ',' || floor_name_" + localeCode + " END AS floor" +
                ", CASE WHEN (mu_stat_com = 0 OR mu_stat_ope_mode = 0) THEN '' ELSE point_name END AS point_name" +
                ", CASE WHEN (mu_stat_com = 0 OR mu_stat_ope_mode = 0) THEN '' ELSE CAST(mu_stat_pos_x AS text) END AS point_x" +
                ", CASE WHEN (mu_stat_com = 0 OR mu_stat_ope_mode = 0) THEN '' ELSE  CAST(mu_stat_pos_y AS text) END AS point_y" +
                ",  CASE WHEN (mu_stat_com = 0 OR mu_stat_ope_mode = 0) THEN '' ELSE  mu_stat_battery_cap || '%' END AS charge" +
                ",CASE WHEN (mu_stat_com = 1 AND mu_stat_ope_mode = 2) THEN mu_stat_muorder_status ELSE 0 END AS muorder_status" +
            " FROM mu_status" +
            " JOIN mu_master ON mu_status.mu_stat_id = mu_master.mu_id" +
            " LEFT OUTER JOIN hospital_master ON mu_status.mu_stat_hospital_id = hospital_master.hospital_id" +
            " LEFT OUTER JOIN floor_master ON mu_status.mu_stat_floor_id = floor_master.floor_id" +
            " LEFT OUTER JOIN point_master ON mu_status.mu_stat_point_last = point_master.point_id" +
            " WHERE mu_stat_enable=1" +
            " ORDER BY mu_stat_id";

            return sql;

        }


        
    }
}
