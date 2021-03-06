﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForOrderHistory
    {
        public static string GetListSql(string conditionSql)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
"SELECT" +
    " to_char(order_result_reserve_datetime, '" + Properties.Resources.FORMAT_DATE + "') as reserve_date" +
    ",to_char(order_result_reserve_datetime, 'HH24:MI:SS') as reserve_time" +
    ",to_char(order_result_start_datetime, '" + Properties.Resources.FORMAT_DATE + "') as start_date" +
    ", to_char(order_result_start_datetime, 'HH24:MI:SS') as start_time" +
    ", to_char(order_result_datetime, '" + Properties.Resources.FORMAT_DATE + "') as result_date" +
    ", to_char(order_result_datetime, 'HH24:MI:SS') as result_time" +
    ", (" + SectionMaster.SelectNameSql(localeCode, "order_result_from_sect") + ") AS req_sect" +
    ", (" + StationMaster.SelectNameByPointSql(localeCode, "order_result_from_pt") + ") AS req_station" +
    ", CASE WHEN order_result_round_flg = 1 OR order_result_forward_list is NULL THEN order_result_stop_to_sects ELSE order_result_forward_list END AS to_sect" +
    ", order_result_stop_to_points AS to_station" +
    ", (" + CartMaster.SelectNameSql(localeCode, "order_result_cart_id") + ") AS cart_id" +
    ", (" + MuMaster.SelectNameSql(localeCode, "order_result_mu_id") + ") AS mu_id" +
//    ", order_result_cart_id AS cart_id" +
//    ", order_result_mu_id AS mu_id" +
    ", CASE order_result_status WHEN 0 THEN '" + Mocs.Properties.Resources.ORDER_RESULT_STATUS_0 + "' WHEN 1 THEN '" + Mocs.Properties.Resources.ORDER_RESULT_STATUS_1 + "' WHEN 10 THEN '" + Mocs.Properties.Resources.ORDER_RESULT_STATUS_10 + "' WHEN 20  THEN '" + Mocs.Properties.Resources.ORDER_RESULT_STATUS_20 + "' WHEN 100 THEN '" + Mocs.Properties.Resources.ORDER_RESULT_STATUS_100 + "' ELSE CAST(order_result_status AS text) END AS status" +
" FROM order_result_log";

            if (conditionSql == null || conditionSql.Length == 0)
            {
                conditionSql = DBAccess.GetTodayConditionSql("order_result_datetime");
            }
            sql += " WHERE " + conditionSql;
            sql += " ORDER BY order_result_datetime DESC, order_result_start_datetime DESC, order_result_reserve_datetime DESC";

            return sql;

        }

        /// <summary>
        /// 指定日以降の条件
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        internal static string GetStartSql(DateTime selectedDate)
        {
            return "order_result_datetime >= " + DateTimeUtil.FormatDBDate(selectedDate); 
        }

        /// <summary>
        /// 指定日以前の条件
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <returns></returns>
        internal static string GetEndSql(DateTime selectedDate)
        {
            // 指定日の翌日より前
            return "order_result_datetime < " + DateTimeUtil.FormatNextDBDate(selectedDate);
        }

        /// <summary>
        /// 発部署の一致する行
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        internal static string GetReqSectSql(int selectedValue)
        {
            return "order_result_from_sect = " + selectedValue;
        }

        /// <summary>
        /// カートIDの一致する行
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        internal static string GetCartSql(int selectedValue)
        {
            return "order_result_cart_id = " + selectedValue;
        }

        /// <summary>
        /// MU idの一致する行
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        internal static string GetMuSql(int selectedValue)
        {
            return "order_result_mu_id = " + selectedValue;
        }

        /// <summary>
        /// 着部署の一致する行
        /// 
        /// order_result_stop_to_sectsにカンマ区切りで着部署が設定されているものとする
        /// または、order_result_forward_listに":"で区切った左側に着部署が設定されているものとする
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        internal static string GetToSectSql(int selectedValue)
        {
            string sql1 = "order_result_index IN (SELECT id FROM (SELECT order_result_index AS id, CAST(unnest(regexp_split_to_array(order_result_stop_to_sects, ',')) AS int) AS data FROM order_result_log) as c WHERE c.data = " + selectedValue + ")";
            string sql2 = "order_result_forward_list LIKE '" + selectedValue + ":%'";

            return "((" + sql1 + ") OR (" + sql2 + "))";
        }
    }
}
