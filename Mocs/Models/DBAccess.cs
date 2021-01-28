using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Mocs.Models;
using Mocs.Utils;
using System.Data;


using System.Windows;

namespace Mocs
{

    public class DBAccess
    {
        public static DBAccess GetDBAccess()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            return mainWindow.GetDBAccess();
        }

        /// <summary>
        /// 当日条件のSQL文
        /// </summary>
        /// <param name="dateTimeFieldName"></param>
        /// <returns></returns>
        public static string GetTodayConditionSql(string dateTimeFieldName)
        {
            return ToDateSql(dateTimeFieldName) + " = " + DateTimeUtil.FormatDBDate(DateTime.Now);
        }
        private static string ToDateSql(string datetimeFieldName)
        {
            return "to_char(" + datetimeFieldName  + ", 'YYYY-MM-DD')";
        }
        

        private NpgsqlConnection conn = new NpgsqlConnection();

        private CiLog cErrlog = new CiLog();    // ログ出力クラス

        //  システム定義
        private SysMainTbl Sys_main_tbl;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectString"></param>
        public DBAccess(string connectString)
        {
            conn.ConnectionString = connectString;
        }


        /// <summary>
        /// 最初のDB読み込み
        /// </summary>
        public int InitialRead()
        {
            int ret = 0;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                // エラーログ出力
                cErrlog.WriteLog(ex.Message.ToString(), ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                return 1;
            }
            //システム定義読み込み
            this.Sys_main_tbl = BaseModel.GetFirst<SysMainTbl>(conn, "select * from sys_main_tbl");



            return ret;
        }



        public DataTable getDataTable(string sql)
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, this.conn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            return ds.Tables[0];
        }

        public  void execute(string sql)
        {
            NpgsqlConnection conn = this.conn;

            using (var transaction = conn.BeginTransaction())
            {
                var command = new NpgsqlCommand(sql, conn);

                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (NpgsqlException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public NpgsqlConnection Conn { get { return conn; } }

        public SysMainTbl sys_main_tbl { get { return Sys_main_tbl; } }


    }
}
