using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    class SectionMaster
    {
        public static string SelectNameSql(string localeCode, string section_id_field_name)
        {
            return "SELECT section_name_" + localeCode + " FROM section_master WHERE section_id=" + section_id_field_name;
        }
    }
}
