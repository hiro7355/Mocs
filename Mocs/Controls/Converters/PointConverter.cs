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
    /// 通過ポイントコンバーター
    /// _で区切られている場合は最後の値
    /// </summary>
    class PointConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v;
            if (GetString(value,out v))
            {
                if (v.Contains("_"))
                {
                    string[] values = v.Split('_');
                    v = values[values.Length - 1];

                }

            }
            return v;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
