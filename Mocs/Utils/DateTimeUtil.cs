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
            return FormatDateTimeString(dt);
        }

        public static string FormatDateTimeString(DateTime dt)
        {
            return FormatString(dt, Resources.FORMAT_DATETIME);
        }
        public static string FormatDateString(DateTime dt)
        {
            return FormatString(dt, Resources.FORMAT_DATE);
        }
        public static string FormatTimeString(DateTime dt)
        {
            return FormatString(dt, Resources.FORMAT_TIME);
        }

        private static string FormatString(DateTime dt, string format)
        {
            return dt.ToString(format);
        }

        public static string FormatDateTimeStringFromString(string datetime)
        {
            return FormatStringFromString(datetime, Resources.FORMAT_DATETIME);
        }
        public static string FormatDateStringFromString(string datetime)
        {
            return FormatStringFromString(datetime, Resources.FORMAT_DATE);
        }
        public static string FormatTimeStringFromString(string datetime)
        {
            return FormatStringFromString(datetime, Resources.FORMAT_TIME);
        }

        private static string FormatStringFromString(string datetime, string format)
        {

            DateTime dt;
            if (DateTime.TryParse(datetime, out dt))
            {
                datetime = FormatString(dt, format);

            }
            return datetime;


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
