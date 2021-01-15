using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForErrorHistory
    {
        public static string GetListSql(string conditionSql, int unionType)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            List<string> values = new List<string>();
            switch(unionType)
            {
                case 1: //  CELLのとき
                    values.Add(GetCellErrorSql(localeCode));
                    break;
                case 2: //  MUのとき
                    values.Add(GetMuErrorSql(localeCode));
                    break;
                default:
                    values.Add(GetCellErrorSql(localeCode));
                    values.Add(GetMuErrorSql(localeCode));
                    break;
            }
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

        private static string GetCellErrorSql(string localeCode)
        {
            string sql =
                "SELECT" +
                " to_char(cellerr_log_datetime, 'YYYY-MM-DD') AS date" +
                 ", to_char(cellerr_log_datetime, 'HH24:MI:SS') AS time" +
                 ", 'CELL' AS type" +
                 ", 'CELL001' AS name" +
                 ", '' AS req_sect" +
                 ", '' AS req_station" +
                 ", '' AS to_sect" +
                 ", '' AS to_station" +
                 ", '' AS cart_id" +
                 ", cellerr_log_code AS code" +
                 ", (SELECT cell_errinfo_msg_" + localeCode + " FROM cell_error_info_master WHERE cell_errinfo_code = cellerr_log_code) AS message" +
                 ", (SELECT cell_errinfo_msgdetail_" + localeCode + " FROM cell_error_info_master WHERE cell_errinfo_code = cellerr_log_code) AS detail" +
                " FROM cell_error_log";
            return sql;
        }

        /// <summary>
        /// 「MU状態ログ」テーブル
        ///  搬送オーダ処理中の異常のみ表示する。
　　    ///  MU有効 and muエラーレベル、muエラーコードが共に<>0の場合
        ///  で且つ、”搬送オーダID”<>0のレコードから、搬送オーダIDから
　　    /// 「搬送オーダ状態履歴テーブル」を参照し、オーダ処理ステータス=100(異常)
        ///  の該当するレコードより発部署/ST,着部署/着STを表示。（着ST空白有り）
　　    ///  ＊この着部署・着STは、１３項と同様の処理（巡回、不在等）
　　    /// ＊①、②の異常内容、詳細の表示は「CELL異常情報マスタ」、「MU異常情報
        /// マスタ」を参照し表示する。
　　    /// 尚、部署、STの登録がない場合は空白表示
        /// </summary>
        /// <returns></returns>
        private static string GetMuErrorSql(string localeCode)
        {
      //      string order_sql = "(SELECT {0} FROM order_status_log WHERE order_log_id=mu_log_order_id LIMIT 1)";
            string sql =
    "SELECT" +
 " to_char(mu_log_datetime, 'YYYY-MM-DD') AS date" +
 ", to_char(mu_log_datetime, 'HH24:MI:SS') AS time" +
 ", 'MU' AS type" +
 ", mu_name_" + localeCode + " AS name" +
    ", (" + SectionMaster.SelectNameSql(localeCode, "order_log_from_sect") + ") AS req_sect" +
    ", (" + StationMaster.SelectNameSql(localeCode, "order_log_from_pt") + ") AS req_station" +
    ", CASE WHEN order_log_round_flg = 1 OR order_log_forward_list is NULL THEN order_log_stop_to_sects ELSE order_log_forward_list END AS to_sect" +
    ", order_log_stop_to_points AS to_station" +
    ", CAST(mu_log_cart_id AS text) AS cart_id" +
 ", mu_log_errcode AS code" +
 ", (SELECT mu_errinfo_msg_" + localeCode + " FROM mu_error_info_master WHERE mu_errinfo_code = mu_log_errcode) AS message" +
 ", (SELECT mu_errinfo_msgdetail_" + localeCode + " FROM mu_error_info_master WHERE mu_errinfo_code = mu_log_errcode) AS detail" +
" FROM mu_status_log" +
" LEFT JOIN mu_master ON mu_id=mu_log_mu_id" +
" LEFT JOIN order_status_log ON order_log_id=mu_log_order_id" +
" WHERE" +
" mu_log_order_status = 1" +
 " AND mu_log_enable = 1" +
 " AND mu_log_errlevel<> 0" +
 " AND mu_log_errcode<> 0" +
 " AND mu_log_order_id<> 0"
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


    }
}
