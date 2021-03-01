using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// IValueConverter、CultureInfo
using System.Windows.Data;
using System.Globalization;

using Mocs.Utils;
using Mocs.Properties;
using Mocs.Models;


namespace Mocs.Controls.Converters
{
    /// <summary>
    /// 日時に変換
    /// _ で区切られているときは一番左
    /// </summary>
    class DatetimeConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string datetime_value;

            string v;
            if (GetString(value, out v))
            {
                if (v.Contains("_"))
                {
                    string[] values = v.Split('_');
                    datetime_value = values[0];

                }
                else
                {
                    datetime_value = v;
                }
                v = DateTimeUtil.FormatDateTimeStringFromString(datetime_value);

            }
            return v;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
