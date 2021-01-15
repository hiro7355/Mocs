using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForComHistory
    {
        public static string GetListSql(string conditionSql, int unionType)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            List<string> values = new List<string>();
            values.Add(GetTabletComSql(localeCode));
            values.Add(GetMonitorComSql(localeCode));
            values.Add(GetMuComSql(localeCode));
            string unionSql = string.Join(" UNION ", values);

            string sql = "SELECT * FROM (" +
                unionSql +
                ") AS TMP ";

            if (conditionSql == null || conditionSql.Length == 0)
            {
                conditionSql = "date = " + DateTimeUtil.FormatDBDate(DateTime.Now);
            }
            sql += " WHERE " + conditionSql;
            sql += " ORDER BY date, time";

            return sql;

        }

        internal static string SelectNameSql(string localeCode)
        {
            List<string> values = new List<string>();
            values.Add(MuMaster.SelectIdAndNameSql(localeCode));        //  MU名一覧
            values.Add(TabletMaster.SelectIdAndNameSql(localeCode));        //  タブレット名一覧
            values.Add(MonitorMaster.SelectIdAndNameSql(localeCode));        //  監視モニタ名一覧
            string unionSql = string.Join(" UNION ", values);

            return unionSql;
        }

        private static string GetMuComSql(string localeCode)
        {
            string sql =
    "SELECT" +
 " to_char(mu_com_datetime, 'YYYY-MM-DD') AS date" +
 ", to_char(mu_com_datetime, 'HH24:MI:SS') AS time" +
 ", 'MU' AS type" +
 ", mu_name_" + localeCode + " AS name" +
 ", mu_com_ipaddr AS ip" +
 ", CAST(mu_com_port AS text) AS port" +
    ", CASE mu_com_type WHEN 1 THEN '" + Properties.Resources.COM_TYPE1 + "' WHEN 2 THEN '" + Properties.Resources.COM_TYPE2 + "' WHEN 3 THEN '" + Properties.Resources.COM_TYPE3 + "' ELSE '' END AS com" +
    ", CASE mu_com_dir WHEN 1 THEN '" + Properties.Resources.SEND + "' WHEN 2 THEN '" + Properties.Resources.RECEIVE + "' ELSE '' END AS send_receive" +
    ", mu_com_dir AS dir" +         //  送受信検索用
    ", substring(mu_com_data from 1 for 40) AS message" +
   ", mu_com_data AS detail" +
" FROM mu_com_log" +
" LEFT JOIN mu_master ON mu_id=mu_com_id" 
 ;
            return sql;

        }

        /// <summary>
        /// 指定日以降の条件
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        internal static string GetStartSql(DateTime selectedDate)
        {
            return "date >= " + DateTimeUtil.FormatDBDate(selectedDate);
        }

        /// <summary>
        /// 指定日以前の条件
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        internal static string GetEndSql(DateTime selectedDate)
        {
            // 指定日の翌日より前
            return "date < " + DateTimeUtil.FormatNextDBDate(selectedDate);
        }


        private static string GetTabletComSql(string localeCode)
        {
            string sql =
    "SELECT" +
 " to_char(tabmon_com_datetime, 'YYYY-MM-DD') AS date" +
 ", to_char(tabmon_com_datetime, 'HH24:MI:SS') AS time" +
 ", '" + Properties.Resources.TABLET+ "' AS type" +
 ", tablet_name_" + localeCode + " AS name" +
 ", host(tablet_ip) AS ip" +
 ", '' AS port" +
    ", CASE tablet_type WHEN 0 THEN '" + Properties.Resources.TABLET_TYPE0 + "' WHEN 1 THEN '" + Properties.Resources.TABLET_TYPE1 + "' ELSE '' END AS com" +
    ", CASE tabmon_com_dir WHEN 1 THEN '" + Properties.Resources.SEND + "' WHEN 2 THEN '" + Properties.Resources.RECEIVE + "' ELSE '' END AS send_receive" +
    ", tabmon_com_dir AS dir" +         //  送受信検索用
    ", substring(tabmon_com_data from 1 for 40) AS message" +
   ", tabmon_com_data AS detail" +
" FROM tabmon_com_log" +
" LEFT JOIN tablet_master ON tablet_id=tabmon_com_id" +
" WHERE" +
" tabmon_com_device_type = 1"
 ;
            return sql;

        }

        private static string GetMonitorComSql(string localeCode)
        {
            string sql =
    "SELECT" +
 " to_char(tabmon_com_datetime, 'YYYY-MM-DD') AS date" +
 ", to_char(tabmon_com_datetime, 'HH24:MI:SS') AS time" +
 ", '" + Properties.Resources.MONITOR + "' AS type" +
 ", mon_name_" + localeCode + " AS name" +
 ", host(mon_ip) AS ip" +
 ", '' AS port" +
    ", '' AS com" +
    ", CASE tabmon_com_dir WHEN 1 THEN '" + Properties.Resources.SEND + "' WHEN 2 THEN '" + Properties.Resources.RECEIVE + "' ELSE '' END AS send_receive" +
    ", tabmon_com_dir AS dir" +         //  送受信検索用
    ", substring(tabmon_com_data from 1 for 40) AS message" +
   ", tabmon_com_data AS detail" +
" FROM tabmon_com_log" +
" LEFT JOIN monitor_master ON mon_id=tabmon_com_id" +
" WHERE" +
" tabmon_com_device_type = 2"
 ;
            return sql;

        }

        internal static string GetDirSql(int dir)
        {
            return "dir = " + dir;
        }

        internal static string GetNameSql(string name)
        {
            return "name = '" + name + "'";
        }

    }


}
