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
    class MuOpeModeConverter : IValueConverter
    {
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
