using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    class MonitorMaster : BaseModel
    {
        public override void LoadProp(NpgsqlDataReader dr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// コンボボックス用にidと名前を取得するSQL
        /// </summary>
        /// <param name="localeCode"></param>
        /// <returns></returns>
        internal static string SelectIdAndNameSql(string localeCode)
        {
            return "SELECT mon_id AS id, mon_name_" + localeCode + " AS name FROM monitor_master";
        }

    }
}
