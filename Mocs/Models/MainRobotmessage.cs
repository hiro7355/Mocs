using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// system__codeをキー、SystemstatMessageを値とするディクショナリ
    /// </summary>
    public class MainRobotmessageDictionary : Dictionary<int, MainRobotmessage>
    {
        public MainRobotmessageDictionary(NpgsqlConnection conn, int level)
        {
            this.Load(conn, level);
        }

        public void Load(NpgsqlConnection conn, int level)
        {
            MainRobotmessage[] rows = BaseModel.GetRows<MainRobotmessage>(conn, "select * from main_robotmessage where mainrobotm_level=" + level);

            foreach (MainRobotmessage row in rows)
            {
                this.Add(row.mainrobotm__code, row);
            }
        }
    }


    public class MainRobotmessage : BaseModel
    {
        public int mainrobotm_index;            // インデックス  serial
        public int mainrobotm_level;            //    状態レベル integer
        public int mainrobotm__code;            // 状態コード   integer
        public int mainrobotm_disptype;         // 状態表示文字機能 integer
        public string mainrobotm_message;       // 状態表示文字  varchar
        public long mainrobotm_color;           //    状態表示文字色 bigint
        public long mainrobotm_backcolor;       // 状態表示背景色 bigint
        public int mainrobotm_size;             // 状態表示文字サイズ integer
        public string mainrobotm_font;          // 状態表示文字フォント  varchar
        public int mainrobotm_recov_disptype;   //   原因・対処文字機能 integer
        public string mainrobotm_recov_message; // 原因、対処メッセージ varchar
        public long mainrobotm_recov_color;     // 原因、対処表示文字色 bigint
        public long mainrobotm_recov_backcolor; // 原因、対処表示背景色 bigint
        public int mainrobotm_recov_size;       // 原因、対処表示文字サイズ integer
        public string mainrobotm_recov_font;    // 原因、対処表示文字フォント varchar

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.mainrobotm_index = this.getValue<int>(dr, "mainrobotm_index");
            this.mainrobotm_level = this.getValue<int>(dr, "mainrobotm_level");
            this.mainrobotm__code = this.getValue<int>(dr, "mainrobotm__code");
            this.mainrobotm_disptype = this.getValue<int>(dr, "mainrobotm_disptype");
            this.mainrobotm_message = this.getValue<string>(dr, "mainrobotm_message");
            this.mainrobotm_color = this.getValue<long>(dr, "mainrobotm_color");
            this.mainrobotm_backcolor = this.getValue<long>(dr, "mainrobotm_backcolor");
            this.mainrobotm_size = this.getValue<int>(dr, "mainrobotm_size");
            this.mainrobotm_font = this.getValue<string>(dr, "mainrobotm_font");
            this.mainrobotm_recov_disptype = this.getValue<int>(dr, "mainrobotm_recov_disptype");
            this.mainrobotm_recov_message = this.getValue<string>(dr, "mainrobotm_recov_message");
            this.mainrobotm_recov_color = this.getValue<long>(dr, "mainrobotm_recov_color");
            this.mainrobotm_recov_backcolor = this.getValue<long>(dr, "mainrobotm_recov_backcolor");
            this.mainrobotm_recov_size = this.getValue<int>(dr, "mainrobotm_recov_size");
            this.mainrobotm_recov_font = this.getValue<string>(dr, "mainrobotm_recov_font");
        }
    }
}
