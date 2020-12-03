using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{

    /// <summary>
    /// func_mesnoをキー、FuncMessageを値とするディクショナリ
    /// </summary>
    public class FuncMessageDictionary : Dictionary<int, FuncMessage>
    {
        public FuncMessageDictionary(NpgsqlConnection conn, int menuType, int submenuType)
        {
            this.Load(conn, menuType, submenuType);
        }

        public void Load(NpgsqlConnection conn, int menuType, int submenuType)
        {
            FuncMessage[] rows = BaseModel.GetRows<FuncMessage>(conn, "select * from func_message where sys_menu_screen_id=" + menuType +  " and sys_submenu_screen_id=" + submenuType);

            foreach (FuncMessage row in rows)
            {
                this.Add(row.func_mesno, row);
            }
        }
    }

    public class FuncMessage : BaseModel
    {

        public int funmes_index;            // INDEX   serial
        public int sys_menu_screen_id;      //  タイトル機能ID integer
        public Int16 sys_submenu_screen_id; // サブタイトル機能ID  smallint
        public Int16 func_mesno;            //   メッセージ番号 smallint
        public Int16 func_disptype;         //  メッセージ表示文字機能 smallint
        public string func_message;         //   メッセージ表示文字 varchar
        public long func_color;             // メッセージ表示文字色  bigint
        public long func_backcolor;         //  メッセージ表示背景色 bigint
        public int func_size;               //  メッセージ表示文字サイズ    integer
        public string func_font;            //   メッセージ表示文字フォント varchar

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.funmes_index = this.getValue<int>(dr, "funmes_index");
            this.sys_menu_screen_id = this.getValue<int>(dr, "sys_menu_screen_id");
            this.sys_submenu_screen_id = this.getValue<Int16>(dr, "sys_submenu_screen_id");
            this.func_mesno = this.getValue<Int16>(dr, "func_mesno");
            this.func_disptype = this.getValue<Int16>(dr, "func_disptype");
            this.func_message = this.getValue<string>(dr, "func_message");
            this.func_color = this.getValue<long>(dr, "func_color");
            this.func_backcolor = this.getValue<long>(dr, "func_backcolor");
            this.func_size = this.getValue<int>(dr, "func_size");
            this.func_font = this.getValue<string>(dr, "func_font");

        }
    }
}
