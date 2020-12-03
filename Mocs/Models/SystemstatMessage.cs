using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// sysmes_levelとsystem__codeをキー、SystemstatMessageを値とするディクショナリ
    /// GetRowで行を取得できる
    /// </summary>
    public class SystemstatMessageDictionary : Dictionary<int, Dictionary<int, SystemstatMessage>>
    {
        public SystemstatMessageDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            SystemstatMessage[] rows = BaseModel.GetRows<SystemstatMessage>(conn, "select * from systemstat_message");

            foreach (SystemstatMessage row in rows)
            {
                int level = row.sysmes_level;

                Dictionary<int, SystemstatMessage> dict;
                if (this.ContainsKey(level))
                {
                    dict = this[level];

                }
                else
                {
                    dict = new Dictionary<int, SystemstatMessage>();
                    this.Add(level, dict);
                }
                dict.Add(row.sysmes__code, row);
            }
        }
        /// <summary>
        /// levelとcodeを指定して行を取得
        /// </summary>
        /// <param name="level"></param>
        /// <param name="code"></param>
        /// <returns>levelとcodeに一致するRobotMessageTbl</returns>
        public SystemstatMessage GetRow(int level, int code)
        {
            SystemstatMessage row = null;
            if (this.ContainsKey(level))
            {
                Dictionary<int, SystemstatMessage> dict = this[level];

                if (dict.ContainsKey(code))
                {
                    row = dict[code];
                }

            }
            return row;
        }

    }


    public class SystemstatMessage : BaseModel
    {
        public int sysmes_index;            //  インデックス  serial
        public int sysmes_level;            //    状態レベル integer
        public int sysmes__code;            // 状態コード   integer
        public int sysmesr_disptype;        //    状態表示文字機能 integer
        public string sysmes_message;       // 状態表示文字  varchar
        public long sysmes_color;           //    状態表示文字色 bigint
        public long sysmes_backcolor;       // 状態表示背景色 bigint
        public int sysmes_size;             // 状態表示文字サイズ integer
        public string sysmes_font;          // 状態表示文字フォント  varchar
        public int sysmes_recov_disptype;   //   原因・対処文字機能 integer
        public string sysmes_recov_message; // 原因、対処メッセージ varchar
        public long sysmes_recov_color;     // 原因、対処表示文字色 bigint
        public long sysmes_recov_backcolor; // 原因、対処表示背景色 bigint
        public int sysmes_recov_size;       // 原因、対処表示文字サイズ integer
        public string sysmes_recov_font;    // 原因、対処表示文字フォント varchar

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.sysmes_index = this.getValue<int>(dr, "sysmes_index");
            this.sysmes_level = this.getValue<int>(dr, "sysmes_level");
            this.sysmes__code = this.getValue<int>(dr, "sysmes__code");
            this.sysmesr_disptype = this.getValue<int>(dr, "sysmesr_disptype");
            this.sysmes_message = this.getValue<string>(dr, "sysmes_message");
            this.sysmes_color = this.getValue<long>(dr, "sysmes_color");
            this.sysmes_backcolor = this.getValue<long>(dr, "sysmes_backcolor");
            this.sysmes_size = this.getValue<int>(dr, "sysmes_size");
            this.sysmes_font = this.getValue<string>(dr, "sysmes_font");
            this.sysmes_recov_disptype = this.getValue<int>(dr, "sysmes_recov_disptype");
            this.sysmes_recov_message = this.getValue<string>(dr, "sysmes_recov_message");
            this.sysmes_recov_color = this.getValue<long>(dr, "sysmes_recov_color");
            this.sysmes_recov_backcolor = this.getValue<long>(dr, "sysmes_recov_backcolor");
            this.sysmes_recov_size = this.getValue<int>(dr, "sysmes_recov_size");
            this.sysmes_recov_font = this.getValue<string>(dr, "sysmes_recov_font");
        }
    }
}
