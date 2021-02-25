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

            int stat_com;
            int ope_mode;
            CommonUtil.GetValue1_2((string)value, out stat_com, out ope_mode);


            if (stat_com == 0)
            {
                return Properties.Resources.MLD_MUORDER_STATUS_NOCON;
            }
            else if(stat_com == 1)
            {
                return CommonUtil.GetValueFromCSV(Properties.Resources.MLD_OPE_MODE, ope_mode);

            } else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
