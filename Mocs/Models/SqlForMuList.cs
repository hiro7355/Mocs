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
                ",mu_stat_ope_mode AS ope_mode" +
                ",hospital_name_" + localeCode + " || ',' || floor_name_" + localeCode + " AS floor" +
                ",point_name" +
                ",point_x" +
                ",point_y" +
                ",mu_stat_battery_cap || '%' AS charge" +
                ",CASE WHEN mu_stat_ope_mode = 2 THEN mu_stat_muorder_status ELSE 0 END AS transport_status" +
            " FROM mu_status" +
            " JOIN mu_master ON mu_status.mu_stat_id = mu_master.mu_id" +
            " LEFT OUTER JOIN hospital_master ON mu_status.mu_stat_hospital_id = hospital_master.hospital_id" +
            " LEFT OUTER JOIN floor_master ON mu_status.mu_stat_floor_id = floor_master.floor_id" +
            " LEFT OUTER JOIN point_master ON mu_status.mu_stat_point_last = point_master.point_id" +
            " WHERE mu_stat_enable=1";

            return sql;

        }


        
    }
}
