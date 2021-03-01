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
    /// カンマ区切りのpointからカンマ区切りのステーション名に変換
    /// </summary>
    class PointsToStationNamesConverter : BaseConverter, IValueConverter
    {
        /// <summary>
        /// 着ステーション
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string localeCode = CommonUtil.GetAppLocaleCode();
                value = GetValue<string>(StationMaster.SelectNamesByPointSql(localeCode, (string)value), "value");

            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
