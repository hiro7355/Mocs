using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{


    /// <summary>
    /// movingm__codeをキー、MovingMessageを値とするディクショナリ
    /// </summary>
    public class MovingMessageDictionary : Dictionary<int, MovingMessage>
    {
        public MovingMessageDictionary(NpgsqlConnection conn, int level)
        {
            this.Load(conn, level);
        }

        public void Load(NpgsqlConnection conn, int level)
        {
            MovingMessage[] rows = BaseModel.GetRows<MovingMessage>(conn, "select * from moving_message where movinge_level=" + level);

            foreach (MovingMessage row in rows)
            {
                this.Add(row.movingm__code, row);
            }
        }
    }

    public class MovingMessage : BaseModel
    {
        public int movingm_index;           //  インデックス  serial
        public int movinge_level;           //   状態レベル integer
        public int movingm__code;           //  状態コード   integer
        public int movingm_disptype;        //    状態表示文字機能 integer
        public string movingm_message;      //  状態表示文字  varchar
        public long movingm_color;          //    状態表示文字色 bigint
        public long movingm_backcolor;      //  状態表示背景色 bigint
        public int movingm_size;            //    状態表示文字サイズ integer
        public string movingm_font;         //  状態表示文字フォント  varchar
        public int movingm_recov_disptype;  //  原因・対処文字機能 integer
        public string movingm_recov_message;// 原因、対処メッセージ varchar
        public long movingm_recov_color;     // 原因、対処表示文字色 bigint
        public long movingm_recov_backcolor;//  原因、対処表示背景色 bigint
        public int movbingm_recov_size;     //  原因、対処表示文字サイズ integer
        public string movingm_recov_font;   //  原因、対処表示文字フォント varchar

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.movingm_index = this.getValue<int>(dr, "movingm_index");
            this.movinge_level = this.getValue<int>(dr, "movinge_level");
            this.movingm__code = this.getValue<int>(dr, "movingm__code");
            this.movingm_disptype = this.getValue<int>(dr, "movingm_disptype");
            this.movingm_message = this.getValue<string>(dr, "movingm_message");
            this.movingm_color = this.getValue<long>(dr, "movingm_color");
            this.movingm_backcolor = this.getValue<long>(dr, "movingm_backcolor");
            this.movingm_size = this.getValue<int>(dr, "movingm_size");
            this.movingm_font = this.getValue<string>(dr, "movingm_font");
            this.movingm_recov_disptype = this.getValue<int>(dr, "movingm_recov_disptype");
            this.movingm_recov_message = this.getValue<string>(dr, "movingm_recov_message");
            this.movingm_recov_color = this.getValue<long>(dr, "movingm_recov_color");
            this.movingm_recov_backcolor = this.getValue<long>(dr, "movingm_recov_backcolor");
            this.movbingm_recov_size = this.getValue<int>(dr, "movbingm_recov_size");
            this.movingm_recov_font = this.getValue<string>(dr, "movingm_recov_font");
        }
    }
}
