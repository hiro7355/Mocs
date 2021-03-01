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
        public static string GetListSql(string orderStatus, int[] excludeOrderIds)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();


            string sql =
            "SELECT" +
                " order_id, false AS is_checked" +
                ",(CASE order_status WHEN 0 THEN '" + Properties.Resources.RESERVE + "' WHEN 1 THEN '" + Properties.Resources.IN_ORDER + "' WHEN 10 THEN '" + Properties.Resources.DONE + "' WHEN 20 THEN '" + Properties.Resources.STOP_CANCEL + "' WHEN 100 THEN '" + Properties.Resources.ERROR + "' END) AS order_status" +
                 ", to_char(order_reserve_datetime, '" + Properties.Resources.FORMAT_DATE + "') as reserve_date" +
                 ",to_char(order_reserve_datetime, 'HH24:MI:SS') as reserve_time" +
                 ",order_cart_id" +
                  ", CASE WHEN order_round_flg <> 1 THEN '" + Properties.Resources.PATROL + "' ELSE '" + Properties.Resources.NORMAL + "' END AS order_round_flg" +     //  搬送種別
                 ", CASE WHEN order_req_sect = 0 THEN '' ELSE section_name_" + localeCode + " END AS req_sect" +        //　発部署
                  ", (SELECT station_name_" + localeCode + " FROM station_master WHERE order_reserve.order_from_pt = station_master.station_point_id1 LIMIT 1) AS req_station" +      //  発ステーション

                 //  従って、着部署の表示は以下になります。

                 //  ・order_round_flg <> 1(巡回搬送無し）、order_forward_list = NULLの場合は

                 //  通常搬送として、order_stop_to_sects、order_stop_to_pointsを表示する。

                 // ・order_round_flg = 1(巡回搬送有）、order_stop_to_sect、order_stop_to_pointsを表示する。

                 ", CASE WHEN  order_round_flg <> 1 AND order_forward_list IS NOT NULL THEN order_forward_list ELSE order_stop_to_sects END AS to_sect" +
                 ", order_stop_to_points AS to_station" + //  着ステーション
                 //                 ", CASE WHEN  order_forward_list IS NOT NULL THEN" +
                 //                       " CASE WHEN order_round_flg <> 1 THEN order_stop_to_sects ELSE cast(order_next_stop_sect as character varying) END" +
                 //                 " END AS to_sect" +             //  着部署
                 //",CASE WHEN order_round_flg <> 1 THEN order_stop_to_points ELSE cast(order_next_stop_pt as character varying) END AS to_station" +     //  着ステーション
                 //                 ", (SELECT order_log_datetime FROM order_status_log WHERE order_id=order_reserve.order_id AND order_log_index=(SELECT MIN(order_log_index) FROM order_status_log WHERE order_id=order_reserve.order_id)) AS start_datetime" + //  開始日付 
            ",order_start_datetime  AS start_datetime" + //  開始日付 
                 ",mu_name_" + localeCode + " AS mu_name" +           //  MU名称
//                ",CASE WHEN mu_stat_com = 0 THEN 1000 ELSE  CASE mu_stat_ope_mode WHEN 1 THEN 1001 WHEN 3 THEN 1003 WHEN 2 THEN (CASE WHEN mu_stat_ope_mode = 2 THEN mu_stat_muorder_status ELSE 0 END) ELSE 0 END END AS muorder_status" + //  MU状態
                ",CASE WHEN mu_stat_com = 0 THEN 1000 ELSE  CASE mu_stat_ope_mode WHEN 1 THEN 1001 WHEN 3 THEN 1003 WHEN 2 THEN mu_stat_muorder_status ELSE 0 END END AS muorder_status" + //  MU状態
                ", (SELECT max(concat_ws('_', mu_log_datetime, mu_log_hospital_id, mu_log_floor_id, mu_log_point_last)) FROM mu_status_log WHERE mu_log_mu_id=order_mu_id) AS datetime_hospital_floor_point" + //  通過時刻、現在フロア、通過ポイント　用の情報
            " FROM order_reserve" +
            " LEFT OUTER JOIN section_master ON order_reserve.order_req_sect = section_master.section_id" +
            //            " LEFT OUTER JOIN station_master ON order_reserve.order_from_pt = station_master.station_point_id1" +
            " LEFT OUTER JOIN cart_master ON order_reserve.order_cart_id = cart_master.cart_id" +
            " LEFT OUTER JOIN mu_master ON order_reserve.order_mu_id = mu_master.mu_id" +
            " LEFT OUTER JOIN mu_status ON order_reserve.order_mu_id = mu_status.mu_stat_id";
            //            " WHERE order_mu_id IS NOT NULL";
            string condition = "";
            if (orderStatus != "all")
            {
                condition += "order_status=" + orderStatus;
            }
            if (excludeOrderIds.Length > 0)
            {
                if (condition != "")
                {
                    condition += " AND ";
                }
                condition += "order_id not in (" + string.Join(", ", excludeOrderIds) + ")";
            }

            if (condition != "")
            {
                sql += " WHERE " + condition;
            }

            sql += " ORDER BY order_reserve.order_id DESC";



            return sql;

        }

    }
}
