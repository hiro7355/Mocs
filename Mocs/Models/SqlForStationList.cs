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
                    ", section_master.section_name_" + localeCode + " AS sect" +
                    ", concat_ws(' / ', CASE station_master.station_type WHEN 1 THEN '" + Properties.Resources.SLV_TYPE_1 + "' WHEN 2 THEN '" + Properties.Resources.SLV_TYPE_2 + "' WHEN 3 THEN '" + Properties.Resources.SLV_TYPE_3 + "' WHEN 4 THEN '" + Properties.Resources.SLV_TYPE_4 + "' ELSE '' END, LPAD(CAST(station_master.station_id AS TEXT), 4, '0')) AS type_id" +
                    ", '' AS absence_setting" +
                    ", CASE WHEN station_master.station_enable = 1 THEN '" + Properties.Resources.SLV_ENABLE_1 + "' ELSE '" + Properties.Resources.SLV_ENABLE_0 + "' END AS enable" +
                    ", CASE WHEN station_master.station_enable = 1 THEN '" + Properties.Resources.SLV_ENABLE_0 + "' ELSE '" + Properties.Resources.SLV_ENABLE_1 + "' END AS change_enable" +
                    ", station_master.station_enable" +
                    ", station_section_id" +
                " FROM station_master" +
                 " LEFT JOIN section_master ON station_master.station_section_id = section_master.section_id" +
                 " LEFT JOIN floor_master ON section_master.section_floor_id = floor_master.floor_id" +
                 " LEFT JOIN hospital_master ON floor_master.floor_hospital_id = hospital_master.hospital_id" +
                 " ORDER BY hospital_master.hospital_id, floor_master.floor_id, section_master.section_id, station_master.station_id";
                 
            return sql;

        }

    }
}
