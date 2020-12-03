using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// システム画面定義
    /// 
    /// TODO:  確認
    /// 定義書のlogin_titleはDBになかったけれど、ログイン画面で使っているのでDBに追加
    /// 定義書のlogin_useをDBにあわせて、login_userに変更
    /// 定義書のscreen_robost_font　をDBにあわせて、screen_robotst_fontに変更
    /// login_cansel のスペル
    /// 
    /// </summary>
    public class SystemScreen : BaseModel
    {
        public int screen_index;                    // INDEX   serial
        public string login_wintitle_name;          // ログインWindowタイトル文字 varchar
        public string login_title;                  // ログインタイトル文字  varchar
        public string login_user;                   // ログインユーザＩＤ文字 varchar
        public string login_pass;                   // ログインパス  varchar
        public string login_buttern;                // ログインボタン文字 varchar
        public string login_cansel;                 // ログインキャンセルボタン文字  varchar
        public string login_errmes;
        public string screen_title_name;            // 運用画面Windowタイトル文字 varchar
        public string screen_system_name;           // 運用画面のシステム名称表示文字 varchar
        public Int64 screen_system_color;          // 運用画面のシステム名称表示文字色 bigint
        public Int64 screen_system_backcolor;       // 運用画面のシステム名称表示背景色    bigint
        public int screen_system_size;              // 運用画面のシステム名称表示文字サイズ integer
        public string screen_system_font;           // 運用画面のシステム名称表示文字フォント varchar
        public string screen_hospital_name;         // 病院名称表示文字 varchar
        public Int64 screen_hospital_color;         // 病院名称表示文字色   bigint
        public Int64 screen_hospital_backcolor;     // 病院名称表示文字背景色 bigint
        public int screen_hospital_size;            // 病院名称表示文字サイズ integer
        public string screen_hospital_font;         // 病院名称表示文字フオント varchar
        public string screen_sysst_name;            // システム状態タイトル文字名称  varchar
        public Int64 screen_sysst_color;            // システム状態タイトル文字色 bigint
        public Int64 screen_sysst_backcolor;        // システム状態タイトル文字背景色 bigint
        public int screen_sysst_size;               // システム状態タイトル文字サイズ integer
        public string screen_sysst_font;            // システム状態タイトル文字フォント    varchar
        public string screen_robotst_name;          // 搬送ロボット状態タイトル名称表示文字 varchar
        public Int64 screen_robotst_color;          // 搬送ロボット状態タイトル名称表示文字色 bigint
        public Int64 screen_robotst_backcolor;      //  搬送ロボット状態タイトル名称表示背景色 bigint
        public int screen_robotst_size;             // 搬送ロボット状態タイトル名称表示文字サイズ   integer
        public string screen_robotst_font;          // 搬送ロボット状態タイトル名称表示文字フォント varchar

        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// ベースクラスのGetFirst / GetRows から呼び出される
        /// </summary>
        /// <param name="dr"></param>
        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.screen_index = this.getValue<int>(dr, "screen_index");
            this.login_wintitle_name = this.getValue<string>(dr, "login_wintitle_name");
            this.login_title = this.getValue<string>(dr, "login_title");
            this.login_user = this.getValue<string>(dr, "login_user");
            this.login_pass = this.getValue<string>(dr, "login_pass");
            this.login_buttern = this.getValue<string>(dr, "login_buttern");
            this.login_cansel = this.getValue<string>(dr, "login_cansel");
            this.login_errmes = this.getValue<string>(dr, "login_errmes");
            this.screen_title_name = this.getValue<string>(dr, "screen_title_name");
            this.screen_system_name = this.getValue<string>(dr, "screen_system_name");
            this.screen_system_color = this.getValue<Int64>(dr, "screen_system_color");
            this.screen_system_backcolor = this.getValue<Int64>(dr, "screen_system_backcolor");
            this.screen_system_size = this.getValue<int>(dr, "screen_system_size");
            this.screen_system_font = this.getValue<string>(dr, "screen_system_font");
            this.screen_hospital_name = this.getValue<string>(dr, "screen_hospital_name");
            this.screen_hospital_color = this.getValue<Int64>(dr, "screen_hospital_color");
            this.screen_hospital_backcolor = this.getValue<Int64>(dr, "screen_hospital_backcolor");
            this.screen_hospital_size = this.getValue<int>(dr, "screen_hospital_size");
            this.screen_hospital_font = this.getValue<string>(dr, "screen_hospital_font");
            this.screen_sysst_name = this.getValue<string>(dr, "screen_sysst_name");
            this.screen_sysst_color = this.getValue<Int64>(dr, "screen_sysst_color");
            this.screen_sysst_backcolor = this.getValue<Int64>(dr, "screen_sysst_backcolor");
            this.screen_sysst_size = this.getValue<int>(dr, "screen_sysst_size");
            this.screen_sysst_font = this.getValue<string>(dr, "screen_sysst_font");
            this.screen_robotst_name = this.getValue<string>(dr, "screen_robotst_name");
            this.screen_robotst_color = this.getValue<Int64>(dr, "screen_robotst_color");
            this.screen_robotst_backcolor = this.getValue<Int64>(dr, "screen_robotst_backcolor");
            this.screen_robotst_size = this.getValue<int>(dr, "screen_robotst_size");
            this.screen_robotst_font = this.getValue<string>(dr, "screen_robotst_font");
        }
    }
}
