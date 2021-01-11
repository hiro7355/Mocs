using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForCancelHistory
    {
        public static string GetListSql()
        {
            
            string start_datetime_sql = "(SELECT order_cancel_log_datetime FROM order_cancel_log WHERE order_cancel_log_status=1 AND order_cancel_log_id=log.order_cancel_log_id LIMIT 1)";
            string result_datetime_sql = "(SELECT order_cancel_log_datetime FROM order_cancel_log WHERE (order_cancel_log_status=10 OR order_cancel_log_status=100) AND order_cancel_log_id=log.order_cancel_log_id LIMIT 1)";
            string status_sql = "(SELECT MAX(order_cancel_log_status) FROM order_cancel_log WHERE order_cancel_log_id=log.order_cancel_log_id)";

            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
"SELECT" +
    " to_char(order_cancel_log_reserve_datetime, 'YYYY-MM-DD') as reserve_date" +
    ",to_char(order_cancel_log_reserve_datetime, 'HH24:MI:SS') as reserve_time" +
    ",to_char(" + start_datetime_sql + ", 'YYYY-MM-DD') as start_date" +
    ", to_char(" + start_datetime_sql + ", 'HH24:MI:SS') as start_time" +
    ", to_char(" + result_datetime_sql + ", 'YYYY-MM-DD') as result_date" +
    ", to_char(" + result_datetime_sql + ", 'HH24:MI:SS') as result_time" +
    ", (" + SectionMaster.SelectNameSql(localeCode, "order_log_from_sect") + ") AS req_sect" +
    ", (" + StationMaster.SelectNameSql(localeCode, "order_log_from_pt") + ") AS req_station" +
    ", CASE WHEN order_log_round_flg = 1 OR order_log_forward_list is NULL THEN order_log_stop_to_sects ELSE order_log_forward_list END AS to_sect" +
    ", order_log_stop_to_points AS to_station" +
    ", order_log_cart_id AS cart_id" +
    ", order_log_mu_id AS mu_id" +
    ", CASE " + status_sql  + " WHEN 0 THEN '" + Properties.Resources.ORDER_CANCEL_STATUS0 + "' WHEN 1 THEN '" + Properties.Resources.ORDER_CANCEL_STATUS1 + "' WHEN 10 THEN '" + Properties.Resources.ORDER_CANCEL_STATUS10 + "' WHEN 100 THEN '" + Properties.Resources.ORDER_CANCEL_STATUS100 + "' ELSE '' END AS status" +
" FROM order_cancel_log AS log" +
" LEFT JOIN order_status_log ON order_log_id=order_cancel_log_order_id" +
" WHERE order_cancel_log_status=0"

;

            return sql;

        }

    }
}
