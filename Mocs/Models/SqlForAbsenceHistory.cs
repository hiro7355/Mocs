using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;


namespace Mocs.Models
{
 
    class SqlForAbsenceHistory
    {
        public static string GetListSql(string conditionSql, string comboValue)
        {


            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
            "SELECT" +
                " to_char(sectlog_datetime, 'yyyy-MM-dd') as date" +
                ",to_char(sectlog_datetime, 'HH24:MI:SS') as time" +
                 ", (" + SectionMaster.SelectNameSql(localeCode, "sectlog_sect_id") + ") AS sect" +     //  部署名
                 ", CASE WHEN sectlog_absence_flg=1 THEN '" + Properties.Resources.ALV_ABSENCE_1 + "' ELSE '" + Properties.Resources.ALV_ABSENCE_0 + "' END AS absence" +
                 ", (" + SectionMaster.SelectNameSql(localeCode, "sectlog_forward_sect") + ") AS forward_sect" +     //  転送先部署名
             " FROM sect_status_log";

            string comboCondition = null;

            if (comboValue == "1")
            {
                comboCondition = "sectlog_absence_flg=1";
            } 
            else if (comboValue == "0")
            {
                comboCondition = "sectlog_absence_flg=0";

            }
            if (conditionSql == null || conditionSql.Length == 0)
            {
                conditionSql = DBAccess.GetTodayConditionSql("sectlog_datetime");
            }
            sql += " WHERE " + conditionSql ;

            if (comboCondition != null)
            {
                sql += " AND " + comboCondition;

            }
            sql += " ORDER BY sectlog_datetime DESC";

            return sql;

        }
        /// <summary>
        /// 指定日以降の条件
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        internal static string GetStartSql(DateTime selectedDate)
        {
            return "sectlog_datetime >= " + DateTimeUtil.FormatDBDate(selectedDate);
        }

        /// <summary>
        /// 指定日以前の条件
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        internal static string GetEndSql(DateTime selectedDate)
        {
            // 指定日の翌日より前
            return "sectlog_datetime < " + DateTimeUtil.FormatNextDBDate(selectedDate);
        }

        /// <summary>
        /// 部署の一致する行
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        internal static string GetSectSql(int selectedValue)
        {
            return "sectlog_sect_id=" + selectedValue;
        }


    }
}
