﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using Mocs.Models;
using Mocs.Utils;

namespace Mocs.Controls.Converters
{
    class BaseConverter
    {
        protected DBAccess GetDBAccess()
        {
            return DBAccess.GetDBAccess();
        }


        protected T GetValue<T>(string sql, string valueFieldName)
        {
            DBAccess db = GetDBAccess();

            return BaseModel.GetFirstValue<T>(db.Conn, sql, valueFieldName);
            
        }


        /// <summary>
        /// 搬送オーダー状態履歴の行を取得
        /// order_status_logのorder_log_idの一致する行でorder_log_datetimeが最新の行が対象
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        protected OrderStatusLog GetOrderStatusLog(int order_id)
        {
            
            OrderStatusLog order_status_log = BaseModel.GetFirst<OrderStatusLog>(GetDBAccess().Conn, "SELECT * FROM order_status_log WHERE order_log_id=" + order_id + " ORDER BY order_log_datetime DESC");

            return order_status_log;
        }


        /// <summary>
        /// 部署ID一覧から部署名一覧を取得
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected string GetSectionNames(string ids)
        {
            return SectionMaster.GetSectionNames(GetDBAccess().Conn, ids);

        }

        protected bool GetInt(object value, out int int_value)
        {
            bool bRet = false;
            int_value = 0;

            if (value is Int32 || value is Int16)
            {
                int_value = (int)value;
                bRet = true;
            }
            else if (value is string)
            {

                if (Int32.TryParse((string)value, out int_value))
                {
                    bRet = true;

                }
            }
            return bRet;

        }

        protected bool GetString(object value, out string string_value)
        {
            bool bRet;
            if (value is string)
            {
                string_value = (string)value;

                bRet = true;
            } 
            else
            {
                string_value = "";
                bRet = false;
            }

            return bRet;


        }

    }
}
