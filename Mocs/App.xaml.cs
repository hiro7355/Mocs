using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mocs.Models;
using Mocs.Utils;

namespace Mocs
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private bool is_login;      //  ログインしたかどうか
        public DBAccess mocs_cell_db;
        private CiLog cErrlog = new CiLog();    // ログ出力クラス

        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool startMinimized = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/StartMinimized")
                {
                    startMinimized = true;
                }
            }

            if (Load_db())
            {
                // Create main application window, starting minimized if specified
                MainWindow mainWindow = new MainWindow(this.mocs_cell_db);
                if (startMinimized)
                {
                    mainWindow.WindowState = WindowState.Minimized;
                }
                mainWindow.Show();

            } else {
                this.Shutdown();
            }
        }

        /// <summary>
        /// DBから初期情報をロード
        /// </summary>
        private bool Load_db()
        {
            int ret = 0;
            try
            {
                this.is_login = false;

                //設定ファイルの有無チェック
                string configfile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
                if (System.IO.File.Exists(configfile) == false)
                {
                    MessageBox.Show("設定ファイル(" + configfile + ")が見つかりません");
                    return false;
                }
                int portno = Mocs.Properties.Settings.Default.DB_Port;
                if ((portno > 65535) || (portno < 0))
                {   //設定ファイルエラー
                    MessageBox.Show("設定ファイルエラー：DB_Port=" + portno.ToString());
                    return false;
                }
                StringBuilder sb = new StringBuilder(128);
                sb.Append("Server=").Append(Mocs.Properties.Settings.Default.DB_HOST).Append(";Port=").Append(portno.ToString());
                sb.Append(";User Id=").Append(Mocs.Properties.Settings.Default.DB_User).Append(";Password=").Append(Mocs.Properties.Settings.Default.DB_Pass);
                sb.Append(";Database=").Append(Mocs.Properties.Settings.Default.DB_Name).Append(";");
                mocs_cell_db = new DBAccess(sb.ToString());
                if ((ret = mocs_cell_db.InitialRead()) > 0)
                {   //DBエラー
                    MessageBox.Show("DB接続エラー");
                    return false;
                }

                /*
                //ログイン画面の表示
                LoginDialog dialog = new LoginDialog();
                dialog.LoginDialog_Load(mocs_cell_db.system_screen, mocs_cell_db.sys_main_tbl.login_name, mocs_cell_db.sys_main_tbl.login_pass);

                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    return false;
                }
                this.is_login = true;
                */


                //  ステータステーブルを初期化
                CellStatus.InitTable(this.mocs_cell_db.Conn);

                /*
                //  ステータス検出のタイマースタート
                this.timer.Interval = Mocs.Properties.Settings.Default.moniter_time;

                if (!this.timer.Enabled)
                {
                    //  タイマーはまだスタートしていない
                    //  タイマースタート
                    this.timer.Start();
                }
                */
            }
            catch (Exception ex)
            {
                // エラーログ出力
                cErrlog.WriteLog(ex.Message.ToString(), ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);

                MessageBox.Show(ex.Message.ToString());
                return false;

            }
            return true;
        }
    }
}
