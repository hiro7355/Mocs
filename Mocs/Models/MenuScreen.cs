using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{

    /// <summary>
    /// メニューtypeをキー、メニューを値とするディクショナリ
    /// </summary>
    public class MenuScreenDictionary : Dictionary<int, MenuScreen>
    {
        public MenuScreenDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            MenuScreen[] rows = BaseModel.GetRows<MenuScreen>(conn, "select * from menu_screen");

            foreach (MenuScreen row in rows)
            {
                this.Add(row.menu_type, row);
            }
        }
    }


    public class MenuScreen : BaseModel
    {
        public Int16 menu_number;    //   ファンクションタイトル番号   smallint
        public Int16 menu_type;      // "ファンクションタイトル番号の機能"	smallint
        public string menu_message;  // タイトル表示文字    varchar
        public long menu_color;     //    タイトル表示文字色 bigint
        public long menu_backcolor; // タイトル表示背景色   bigint
        public int menu_size;       //    タイトル表示文字サイズ integer
        public string menu_font;    // タイトル表示文字フォント    varchar


        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.menu_number = this.getValue<Int16>(dr, "menu_number");
            this.menu_type = this.getValue<Int16>(dr, "menu_type");
            this.menu_message = this.getValue<string>(dr, "menu_message");
            this.menu_color = this.getValue<long>(dr, "menu_color");
            this.menu_backcolor = this.getValue<long>(dr, "menu_backcolor");
            this.menu_size = this.getValue<int>(dr, "menu_size");
            this.menu_font = this.getValue<string>(dr, "menu_font");
        }
    }
}
