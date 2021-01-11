using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForOrderHistory
    {
        public static string GetListSql()
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
"SELECT" +
    " to_char(order_result_reserve_datetime, 'YYYY-MM-DD') as reserve_date" +
    ",to_char(order_result_reserve_datetime, 'HH24:MI:SS') as reserve_time" +
    ",to_char(order_result_start_datetime, 'YYYY-MM-DD') as start_date" +
    ", to_char(order_result_start_datetime, 'HH24:MI:SS') as start_time" +
    ", to_char(order_result_datetime, 'YYYY-MM-DD') as result_date" +
    ", to_char(order_result_datetime, 'HH24:MI:SS') as result_time" +
    ", (" + SectionMaster.SelectNameSql(localeCode, "order_result_from_sect") + ") AS req_sect" +
    ", (" + StationMaster.SelectNameSql(localeCode, "order_result_from_pt") + ") AS req_station" +
    ", CASE WHEN order_result_round_flg = 1 OR order_result_forward_list is NULL THEN order_result_stop_to_sects ELSE order_result_forward_list END AS to_sect" +
    ", order_result_stop_to_points AS to_station" +
    //    ", (" + CartMaster.SelectNameSql(localeCode, "order_result_cart_id") + ") AS cart_name" +
    //    ", (" + MuMaster.SelectNameSql(localeCode, "order_result_mu_id") + ") AS mu_name" +
    ", order_result_cart_id AS cart_id" +
    ", order_result_mu_id AS mu_id" +
    ", CASE WHEN order_result_status = 0 THEN '" + Mocs.Properties.Resources.ORDER_RESULT_STATUS_0 + "' ELSE CAST(order_result_status AS text) END AS status" +
" FROM order_result_log";

            return sql;

        }


    }
}
