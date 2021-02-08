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
        public DBAccess mocs_cell_db;
        private CiLog cErrlog = new CiLog();    // ログ出力クラス

        void App_Startup(object sender, StartupEventArgs e)
        {

            //  言語設定を変更
            CommonUtil.SetLocale();

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
                //  メイン画面の生成（ログイン画面生成前に行う必要がある）
                MainWindow mainWindow = new MainWindow(this.mocs_cell_db);

                //ログイン画面の表示
                LoginWindow loginWindow = new LoginWindow();
                
                if (loginWindow.ShowDialog() == true)
                {
                    if (startMinimized)
                    {
                        mainWindow.WindowState = WindowState.Minimized;
                    }
                    mainWindow.InitWithDB();

                    //  メイン画面の表示
                    mainWindow.Show();
                } 
                else
                {
                    this.Shutdown();
                }
            }
            else
            {
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

                //設定ファイルの有無チェック
                string configfile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
                if (System.IO.File.Exists(configfile) == false)
                {
                    MessageBox.Show(string.Format(Mocs.Properties.Resources.ERROR_SETTING_FILE, configfile));
                    return false;
                }
                int portno = Mocs.Properties.Settings.Default.DB_Port;
                if ((portno > 65535) || (portno < 0))
                {   //設定ファイルエラー
                    MessageBox.Show(string.Format(Mocs.Properties.Resources.ERRPR_SETTING_FILE_DBPORT, portno.ToString()));
                    return false;
                }
                StringBuilder sb = new StringBuilder(128);
                sb.Append("Server=").Append(Mocs.Properties.Settings.Default.DB_HOST).Append(";Port=").Append(portno.ToString());
                sb.Append(";User Id=").Append(Mocs.Properties.Settings.Default.DB_User).Append(";Password=").Append(Mocs.Properties.Settings.Default.DB_Pass);
                sb.Append(";Database=").Append(Mocs.Properties.Settings.Default.DB_Name).Append(";");
                mocs_cell_db = new DBAccess(sb.ToString());
                if ((ret = mocs_cell_db.InitialRead()) > 0)
                {   //DBエラー
                    MessageBox.Show(Mocs.Properties.Resources.ERRPR_CONNECT_DB);
                    return false;

                }

                //  ステータステーブルを初期化
                CellStatus.InitTable(this.mocs_cell_db.Conn);

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
