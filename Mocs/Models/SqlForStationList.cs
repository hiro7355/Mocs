using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForStationList
    {
        public static string GetListSql()
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
                "SELECT" +
                    " hospital_master.hospital_name_" + localeCode + " AS hospital" +
                    ", floor_master.floor_name_" + localeCode + " AS floor" +
                    ", sm.section_name_" + localeCode + " AS sect" +
                    ", concat_ws(' / ', CASE station_master.station_type WHEN 1 THEN '" + Properties.Resources.SLV_TYPE_1 + "' WHEN 2 THEN '" + Properties.Resources.SLV_TYPE_2 + "' WHEN 3 THEN '" + Properties.Resources.SLV_TYPE_3 + "' WHEN 4 THEN '" + Properties.Resources.SLV_TYPE_4 + "' ELSE '' END, LPAD(CAST(station_master.station_id AS TEXT), 4, '0')) AS type_id" +
                    //  「不在設定/転送部署」欄の表示は、受信属性のステーションにのみ　”不在/転送先部署名”を表示する。（不在フラグ＝1で表示。）
                    ", CASE WHEN (sm.section_absence_flg=1 AND station_master.station_type = 2) THEN concat_ws(' ', '" + Properties.Resources.ABSENCE  +  "', '/', (SELECT section_name_" + localeCode+ " FROM section_master WHERE section_id=sm.section_forward_sect)) ELSE '' END AS  absence_setting" + 
                    ", '' AS absence_setting" +
                    ", CASE WHEN station_master.station_enable = 1 THEN '" + Properties.Resources.SLV_ENABLE_1 + "' ELSE '" + Properties.Resources.SLV_ENABLE_0 + "' END AS enable" +
                    ", CASE WHEN station_master.station_enable = 1 THEN '" + Properties.Resources.SLV_ENABLE_0 + "' ELSE '" + Properties.Resources.SLV_ENABLE_1 + "' END AS change_enable" +
                    ", station_master.station_enable" +
                    ", station_section_id" +
                " FROM station_master" +
                 " LEFT JOIN section_master sm ON station_master.station_section_id = sm.section_id" +
                 " LEFT JOIN floor_master ON sm.section_floor_id = floor_master.floor_id" +
                 " LEFT JOIN hospital_master ON floor_master.floor_hospital_id = hospital_master.hospital_id" +
                 " ORDER BY hospital_master.hospital_id, floor_master.floor_floors_number, sm.section_id, station_master.station_type, station_master.station_id";


            return sql;

        }

    }
}
