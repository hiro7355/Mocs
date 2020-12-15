using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForTabletList
    {
        public static string GetListSql()
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
                "SELECT " +
                    " hospital_name_" + localeCode + " AS hospital" +
                    ",floor_name_" + localeCode + " AS floor" +
                    ", section_name_" + localeCode + " AS sect" +
                    ", tablet_name_" + localeCode + " AS tablet_name" +
                    ", tablet_ip AS tablet_ip" +
                " FROM tablet_master" +
                " LEFT JOIN section_master ON tablet_master.tablet_sect = section_master.section_id" +
                " LEFT JOIN floor_master ON section_master.section_floor_id = floor_master.floor_id" +
                " LEFT JOIN hospital_master ON floor_master.floor_hospital_id = hospital_master.hospital_id" +
                " ORDER BY hospital_master.hospital_id, floor_master.floor_id, section_master.section_id";
            return sql;

        }



    }
}
