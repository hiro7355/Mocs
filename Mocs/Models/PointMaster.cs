using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    public class PointMaster
    {
        internal static string SelectNameSql(string localeCode, string id_field_name, string other_name)
        {
            if (other_name != null)
            {
                other_name = " AS " + other_name;
            }
            else
            {
                other_name = "";
            }

            return "SELECT point_name_" + localeCode + other_name + " FROM point_master WHERE point_id=" + id_field_name;
        }

    }
}
