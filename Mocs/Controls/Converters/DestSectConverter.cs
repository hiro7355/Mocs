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
    /// カート一覧の行先部署
    /// 行先部署欄の表示は、行先ID(cart_dest_id)より搬送行先マスタ（dest_master）の該当するdest_idの搬送行先立寄り部署、　　搬送行先巡回フラグの状態により、部署を表示する。（巡回の場合は複数部署＋”（巡回）”の表示）
    /// </summary>
    class DestSectConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int dest_id = (int)value;
            value = "";

            DestMaster dest = BaseModel.GetFirst<DestMaster>(GetDBAccess().Conn, "SELECT * FROM dest_master WHERE dest_id=" + dest_id);

            if (dest != null)
            {

                string localeCode = CommonUtil.GetAppLocaleCode();

                // 搬送行先立寄り部署から部署名をカンマ区切りで取得
                value = GetValue<string>(SectionMaster.SelectNamesSql(localeCode, dest.dest_stop_sects), "value");

                if (dest.dest_round_flg != 0)
                {
                    value = value + "(" + Properties.Resources.PATROL + ")";

                }
            }

            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
