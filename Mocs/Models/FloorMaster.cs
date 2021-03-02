using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    class FloorMaster
    {
        /// <summary>
        /// 名前を取得するSQL
        /// </summary>
        /// <param name="localeCode"></param>
        /// <param name="id_field_name"></param>
        /// <returns></returns>
        internal static string SelectNameSql(string localeCode, string id_field_name, string other_name = null)
        {
            if (other_name != null)
            {
                other_name = " AS " + other_name;
            }
            else
            {
                other_name = "";
            }

            return "SELECT floor_name_" + localeCode + other_name + " FROM floor_master WHERE floor_id=" + id_field_name;
        }

    }
}
