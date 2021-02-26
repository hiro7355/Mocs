using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Converter 用
// IValueConverter、CultureInfo
using System.Windows.Data;
using System.Globalization;

using Mocs.Utils;
using Mocs.Properties;
using Mocs.Models;

namespace Mocs.Controls.Converters
{
    /// <summary>
    /// オーダーIDからカートに変換
    /// order_status_logのorder_log_idの一致する行でorder_log_datetimeが最新の行が対象
    /// 
    /// </summary>
    class OrderIdToCartConverter : BaseConverter, IValueConverter
    {
        /// <summary>
        /// オーダーIDからカートに変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int order_id;
            string result_value = "";

            if (this.GetInt(value, out order_id))
            {
                // order_status_logのorder_log_idの一致する行でorder_log_datetimeが最新の行が対象
                OrderStatusLog order_status_log = GetOrderStatusLog(order_id);

                if (order_status_log != null)
                {
                    string localeCode = CommonUtil.GetAppLocaleCode();

                    //  カート名を取得
                    string valueFieldName = "cart_name_" + localeCode;

                    result_value = GetValue<string>(CartMaster.SelectNameSql(localeCode, order_status_log.order_log_cart_id.ToString()), valueFieldName);

                }


            }

            return result_value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
