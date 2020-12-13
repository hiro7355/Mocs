using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;

namespace Mocs.Models
{
    class SqlForCartList
    {
        public static string GetListSql(string cart_enable)
        {
            string localeCode = CommonUtil.GetAppLocaleCode();

            string sql =
            "SELECT " +
                "cart_id AS id" +
                ",cart_name_" + localeCode + " AS name" +
                ",CASE cart_use WHEN 0 THEN '" + Properties.Resources.CLV_USE_0 + "'" +
                               "WHEN 1 THEN '" + Properties.Resources.CLV_USE_1 + "'" +
                               "WHEN 2 THEN '" + Properties.Resources.CLV_USE_2 + "'" +
                               "ELSE '' " +
                            "END As use" +
                ",CASE WHEN cart_enable = 1 THEN '" + Properties.Resources.CLV_ENABLE_1 + "' ELSE '" + Properties.Resources.CLV_ENABLE_0 + "' END AS enable" +
                ",(" + SectionMaster.SelectNameSql(localeCode, "cart_master.cart_section_id") + ") AS belong_sect" +    // 所属部署名
                ",CASE WHEN cart_sect_restrict_flg = 0 THEN '" + Properties.Resources.CLV_RESTRICT_0 + "' ELSE '" + Properties.Resources.CLV_RESTRICT_NOT_0 + "' END AS belong_restrict" +  //  所属部署制限
                ",(" + SectionMaster.SelectNameSql(localeCode, "cart_master.cart_dest_id") + ") AS dest_sect" +    // 行先部署
                ",CASE WHEN cart_dest_restrict_flg = 0 THEN '" + Properties.Resources.CLV_RESTRICT_0 + "' ELSE '" + Properties.Resources.CLV_RESTRICT_NOT_0 + "' END AS dest_restrict" +  //  行先部署制限
                ",CASE cart_func WHEN 1 THEN '" + Properties.Resources.CLV_FUNC_1 + "'" +
                               "WHEN 2 THEN '" + Properties.Resources.CLV_FUNC_2 + "'" +
                               "ELSE '' " +
                            "END As func" +
                ",'' AS key" +
            " FROM cart_master";

            if (cart_enable != "all")
            {
                sql += " WHERE cart_enable=" + cart_enable;
            }
            sql += " ORDER BY cart_id ASC";
            return sql;

        }



    }
}
