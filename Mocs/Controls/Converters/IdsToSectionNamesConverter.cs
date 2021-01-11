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
    /// カンマ区切りのidからカンマ区切りの部署名に変換
    /// </summary>
    class IdsToSectionNamesConverter : BaseConverter, IValueConverter
    {
        /// <summary>
        /// 着部署
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string v = (string)value;
            if (v.Contains(":"))
            {
                string valueFieldName = "section_name_" + localeCode;
                //  不在転送リストから部署名を取得
                string[] values = v.Split(':');
                string name1 = GetValue<string>(SectionMaster.SelectNameSql(localeCode, values[0]), valueFieldName);
                string name2 = GetValue<string>(SectionMaster.SelectNameSql(localeCode, values[1]), valueFieldName);
                value = name1;
                if (name2 != null)
                {
                    value += "(" + name2 + ")";
                }

            }
            else
            {
                //  立ち寄り部署一覧から部署名をカンマ区切りで取得
                value = GetValue<string>(SectionMaster.SelectNamesSql(localeCode, v), "value");
            }


            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
