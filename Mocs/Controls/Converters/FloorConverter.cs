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
    /// フロアコンバーター
    /// _　で区切られたhospital_idとfloor_idから病棟名, フロア名の形式に変換
    /// _が2個以上あるときは、２番目がhospital_id、３番目がfloor_id
    /// </summary>
    class FloorConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hospital_id;
            string floor_id;
            string v = (string)value;
            if (v.Contains("_"))
            {

                string[] values = v.Split('_');
                if (values.Length > 1)
                {
                    hospital_id = values[1];
                    floor_id = values[2];
                } 
                else
                {
                    hospital_id = values[0];
                    floor_id = values[1];

                }

                string localeCode = CommonUtil.GetAppLocaleCode();

                string valueFieldName = "hospital_name_" + localeCode;
                string name1 = GetValue<string>(HospitalMaster.SelectNameSql(localeCode, hospital_id), valueFieldName);
                valueFieldName = "floor_name_" + localeCode;
                string name2 = GetValue<string>(FloorMaster.SelectNameSql(localeCode, floor_id), valueFieldName);
                v = name1 + "," + name2;

            }
            else
            {

                v = "";
            }
            return v;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
