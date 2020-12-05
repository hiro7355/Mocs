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

namespace Mocs.Controls.Converters
{
    /// <summary>
    /// mu一覧の「搬送状況モード」列表示用
    /// </summary>
    class MuOrderStatusConverter : IValueConverter
    {
        /// <summary>
        /// mu_stat_ope_muorder_statusの値を表示内容に変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int muorder_status = (int)value;

            switch (muorder_status)
            {
                case 0:
                    value = "";
                    break;
                case 1:
                    value = Properties.Resources.MLD_MUORDER_STATUS_1;
                    break;

                case 10:
                    value = Properties.Resources.MLD_MUORDER_STATUS_10;
                    break;
                case 11:
                    value = Properties.Resources.MLD_MUORDER_STATUS_11;
                    break;
                case 12:
                    value = Properties.Resources.MLD_MUORDER_STATUS_12;
                    break;
                case 13:
                    value = Properties.Resources.MLD_MUORDER_STATUS_13;
                    break;
                case 20:
                    value = Properties.Resources.MLD_MUORDER_STATUS_20;
                    break;
                case 21:
                    value = Properties.Resources.MLD_MUORDER_STATUS_21;
                    break;
                case 30:
                    value = Properties.Resources.MLD_MUORDER_STATUS_30;
                    break;

                default:
                    value = "";
                    break;
            }

            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
