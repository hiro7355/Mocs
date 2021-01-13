using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    class TabletMaster : BaseModel
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
            return "SELECT tablet_id AS id, tablet_name_" + localeCode + " AS name FROM tablet_master";
        }

    }
}
