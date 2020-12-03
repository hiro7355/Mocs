using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{


    /// <summary>
    /// サブメニューtypeをキー、メニューを値とするディクショナリ
    /// </summary>
    public class SubmenuScreenDictionary : Dictionary<int, SubmenuScreen>
    {
        public SubmenuScreenDictionary(NpgsqlConnection conn, int menuType)
        {
            this.Load(conn, menuType);
        }

        public void Load(NpgsqlConnection conn, int menuType)
        {
            SubmenuScreen[] rows = BaseModel.GetRows<SubmenuScreen>(conn, "select * from submenu_screen where menu_screen_tbl_id=" + menuType);

            foreach (SubmenuScreen row in rows)
            {
                this.Add(row.submenu_type, row);
            }
        }
    }

    /**
     * 
     * "テーブル名：submenu_screen

  システム画面の「②ファンクション表示部」で、
　menu_number =1 (左）の menu_typeの設定により
　サブ・メニューをプルダウン表示する。

　menu_type=1(機能）の場合、submenu_screenの
　menu_screen_tbl_id=1,submenu_number=1～n
  （submenu_message=NULLで終了）までのメニュー
　文字を表示する。尚、サブメニュールの機能は
　submenu_typeで指定。（1:システム起動、2:ロボット
　状態　3:システム停止）
  *submenu_typeはユニーク番号
  
　
"
*/



    /// <summary>
    /// 
    /// 
    /// </summary>
    public class SubmenuScreen : BaseModel
    {

        public Int16 menu_screen_tbl_id;        //  メインタイトル機能   smallint
        public Int16 submenu_number;            //  サブタイトル番号 smallint
        public Int16 submenu_type;              //  サブタイトル機能    smallint
        public Int16 submenu_id;                //  サブメッセージID smallint
        public string submenu_message;          //  サブタイトル表示文字  varchar
        public long submenu_color;              //  サブタイトル文字色 bigint
        public long submenu_backcolor;          //  サブタイトル背景色   bigint
        public int submenu_size;                //  サブタイトル文字サイズ integer
        public string submenu_font;             //  サブタイトル文字フォント    varchar



        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.menu_screen_tbl_id = this.getValue<Int16>(dr, "menu_screen_tbl_id");
            this.submenu_number = this.getValue<Int16>(dr, "submenu_number");
            this.submenu_type = this.getValue<Int16>(dr, "submenu_type");
            this.submenu_id = this.getValue<Int16>(dr, "submenu_id");
            this.submenu_message = this.getValue<string>(dr, "submenu_message");
            this.submenu_color = this.getValue<long>(dr, "submenu_color");
            this.submenu_backcolor = this.getValue<long>(dr, "submenu_backcolor");
            this.submenu_size = this.getValue<int>(dr, "submenu_size");
            this.submenu_font = this.getValue<string>(dr, "submenu_font");
        }
    }
}
