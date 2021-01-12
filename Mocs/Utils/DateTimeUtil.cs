using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Properties;
namespace Mocs.Utils
{
    class DateTimeUtil
    {
        public static string CurrentDateTimeString()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString(Resources.FORMAT_DATETIME);
        }

        /// <summary>
        /// DB 参照用の日付文字列に変換
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string FormatDBDate(DateTime dt)
        {
            return "'" + dt.ToString("yyyy-MM-dd") + "'";
        }

        /// <summary>
        /// 翌日のDB日付文字列に変換
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string FormatNextDBDate(DateTime dt)
        {
            return FormatDBDate(dt.AddDays(1));
        }

    }
}
