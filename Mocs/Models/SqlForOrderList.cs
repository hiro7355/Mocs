using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForOrderList
    {
        public static string GetListSql(string orderStatus)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            
            string sql =
            "SELECT" +
                " (CASE order_status WHEN 0 THEN '" + Properties.Resources.RESERVE + "' WHEN 1 THEN '" + Properties.Resources.IN_ORDER + "' WHEN 10 THEN '" + Properties.Resources.DONE + "' WHEN 20 THEN '" + Properties.Resources.STOP_CANCEL + "' WHEN 100 THEN '" + Properties.Resources.ERROR + "' END) AS order_status" +
                 ", to_char(order_reserve_datetime, '" + Properties.Resources.FORMAT_DATE + "') as reserve_date" +
                 ",to_char(order_reserve_datetime, 'HH24:MI:SS') as reserve_time" +
                 ",order_cart_id" +
                  ", order_round_flg" +
                 ", CASE WHEN order_req_sect = 0 THEN '' ELSE section_name_" + localeCode + " END AS req_sect" +        //　発部署
                  ", station_name_en AS req_station" +      //  発ステーション
                 ", CASE WHEN order_forward_list IS NOT NULL THEN order_forward_list ELSE" +
                       " CASE WHEN order_round_flg <> 1 THEN order_stop_to_sects ELSE cast(order_next_stop_sect as character varying) END" +
                 " END AS to_sect" +             //  着部署
                 ",CASE WHEN order_round_flg <> 1 THEN order_stop_to_points ELSE cast(order_next_stop_pt as character varying) END AS to_station" +     //  着ステーション
                 ", (SELECT order_log_datetime FROM order_status_log WHERE order_id=order_reserve.order_id AND order_log_index=(SELECT MIN(order_log_index) FROM order_status_log WHERE order_id=order_reserve.order_id)) AS start_datetime" + //  開始日付 
                 ",cart_name_" + localeCode + " AS mu_name" +           //  MU名称
                ",CASE WHEN mu_stat_com = 0 THEN 1000 ELSE  CASE mu_stat_ope_mode WHEN 1 THEN 1001 WHEN 3 THEN 1003 WHEN 2 THEN (CASE WHEN mu_stat_ope_mode = 2 THEN mu_stat_muorder_status ELSE 0 END) ELSE 0 END END AS muorder_status" + //  MU状態
                ", (SELECT max(concat_ws('_', mu_log_datetime, mu_log_hospital_id, mu_log_floor_id, mu_log_point_last)) FROM mu_status_log WHERE mu_log_cart_id=order_cart_id) AS datetime_hospital_floor_point" + //  通過時刻、現在フロア、通過ポイント　用の情報
            " FROM order_reserve" +
            " LEFT OUTER JOIN section_master ON order_reserve.order_req_sect = section_master.section_id" +
            " LEFT OUTER JOIN station_master ON order_reserve.order_from_pt = station_master.station_point_id1" +
            " LEFT OUTER JOIN cart_master ON order_reserve.order_cart_id = cart_master.cart_id" +
            " LEFT OUTER JOIN mu_status ON order_reserve.order_cart_id = mu_status.mu_stat_cart_id" +
            " WHERE order_mu_id IS NOT NULL";
            if (orderStatus != "all")
            {
                sql += " AND order_status=" + orderStatus;
            }


            return sql;

        }

    }
}
