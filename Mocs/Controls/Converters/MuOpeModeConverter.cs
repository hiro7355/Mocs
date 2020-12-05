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
    /// mu一覧の「運転モード」列表示用
    /// </summary>
    class MuOpeModeConverter : IValueConverter
    {
        /// <summary>
        /// mu_stat_ope_modeの値を表示内容に変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CommonUtil.GetValueFromCSV(Properties.Resources.MLD_OPE_MODE, (short)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
