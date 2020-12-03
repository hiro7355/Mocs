using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{

    /// <summary>
    /// robotm_levelとrobotm__codeをキー、RobotMessageTblを値とするディクショナリ
    /// GetRowで行を取得できる
    /// </summary>
    public class RobotMessageTblDictionary : Dictionary<int, Dictionary<int, RobotMessageTbl>>
    {
        public RobotMessageTblDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            RobotMessageTbl[] rows = BaseModel.GetRows<RobotMessageTbl>(conn, "select * from robot_message_tbl");

            foreach (RobotMessageTbl row in rows)
            {
                int level = row.robotm_level;

                Dictionary<int, RobotMessageTbl> dict;
                if (this.ContainsKey(level))
                {
                    dict = this[level];

                } else
                {
                    dict = new Dictionary<int, RobotMessageTbl>();
                    this.Add(level, dict);
                }
                dict.Add(row.robotm__code, row);
            }
        }

        /// <summary>
        /// levelとcodeを指定して行を取得
        /// </summary>
        /// <param name="level"></param>
        /// <param name="code"></param>
        /// <returns>levelとcodeに一致するRobotMessageTbl</returns>
        public RobotMessageTbl GetRow(int level, int code)
        {
            RobotMessageTbl row = null;
            if (this.ContainsKey(level))
            {
                Dictionary<int, RobotMessageTbl> dict = this[level];

                if (dict.ContainsKey(code))
                {
                    row = dict[code];
                }

            }
            return row;
        }
    }

    public class RobotMessageTbl : BaseModel
    {
        public int robotm_index;            //  インデックス  serial
        public int robotm_level;            //    状態レベル integer
        public int robotm__code;            //  状態コード   integer
        public Int16 robotm_disptype;       //  状態表示文字機能 smallint
        public string robotm_message;       // 状態表示文字  varchar
        public long robotm_color;           //    状態表示文字色 bigint
        public long robotm_backcolor;       //  状態表示背景色 bigint
        public int robotm_size;             //  状態表示文字サイズ integer
        public string robotm_font;          //  状態表示文字フォント  varchar
        public Int16 robotmrecov_disptype;  //     原因・対処文字機能 smallint
        public string robotmrecov_message;  //  原因、対処メッセージ varchar
        public long robotmrecov_color;      //  原因、対処表示文字色 bigint
        public long robotmrrecov_backcolor; //  原因、対処表示背景色 bigint
        public int robotmrecov_size;        //  原因、対処表示文字サイズ integer
        public string robotmrecov_font;     //  原因、対処表示文字フォント varchar

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.robotm_index = this.getValue<int>(dr, "robotm_index");
            this.robotm_level = this.getValue<int>(dr, "robotm_level");
            this.robotm__code = this.getValue<int>(dr, "robotm__code");
            this.robotm_disptype = this.getValue<Int16>(dr, "robotm_disptype");
            this.robotm_message = this.getValue<string>(dr, "robotm_message");
            this.robotm_color = this.getValue<long>(dr, "robotm_color");
            this.robotm_backcolor = this.getValue<long>(dr, "robotm_backcolor");
            this.robotm_size = this.getValue<int>(dr, "robotm_size");
            this.robotm_font = this.getValue<string>(dr, "robotm_font");
            this.robotmrecov_disptype = this.getValue<Int16>(dr, "robotmrecov_disptype");
            this.robotmrecov_message = this.getValue<string>(dr, "robotmrecov_message");
            this.robotmrecov_color = this.getValue<long>(dr, "robotmrecov_color");
            this.robotmrrecov_backcolor = this.getValue<long>(dr, "robotmrrecov_backcolor");
            this.robotmrecov_size = this.getValue<int>(dr, "robotmrecov_size");
            this.robotmrecov_font = this.getValue<string>(dr, "robotmrecov_font");
        }

    }
}
