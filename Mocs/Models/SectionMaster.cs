using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Models
{
    class SectionMaster
    {
        /// <summary>
        /// 名前を取得するSQL
        /// </summary>
        /// <param name="localeCode"></param>
        /// <param name="section_id_field_name"></param>
        /// <returns></returns>
        internal static string SelectNameSql(string localeCode, string section_id_field_name)
        {
            return "SELECT section_name_" + localeCode + " FROM section_master WHERE section_id=" + section_id_field_name;
        }

        /// <summary>
        /// コンボボックス用にidと名前を取得するSQL
        /// </summary>
        /// <param name="localeCode"></param>
        /// <returns></returns>
        internal static string SelectIdAndNameSql(string localeCode)
        {
            return "SELECT section_id AS id, section_name_" + localeCode + " AS name FROM section_master ORDER BY section_id";
        }


        /// <summary>
        /// 複数行の名前をカンマ区切りで取得するSQL。取得する行のidは配列で指定
        /// </summary>
        /// <param name="localeCode"></param>
        /// <param name="ids">カンマ区切りのid一覧</param>
        /// <returns></returns>
        internal static string SelectNamesSql(string localeCode, string ids)
        {
            return BaseModel.GetNamesSql("section_name_" + localeCode, "section_master", "section_id", ids);
        }
    }
}
