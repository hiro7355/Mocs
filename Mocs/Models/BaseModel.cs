using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Mocs.Utils;
using Npgsql;


namespace Mocs.Models
{
    public abstract class BaseModel
    {
        /// <summary>
        /// メンバー変数にテーブル行の値を読み込むメソッド
        /// 発生クラスで実装する
        /// </summary>
        /// <param name="dr"></param>
        public abstract void LoadProp(NpgsqlDataReader dr);



        /// <summary>
        /// テーブルから一行読み込んで、modelクラスのインスタンスにロード
        /// </summary>
        /// <typeparam name="T">modelクラス（BaseModelの派生クラス）</typeparam>
        /// <param name="conn">DBへの接続インスタンス</param>
        /// <param name="sql">SQL文</param>
        /// <returns>modelクラスのインスタンス</returns>
        public static T GetFirst<T>(NpgsqlConnection conn, string sql) where T : BaseModel, new()
        {
            T model = null;
            string errorMessage = null;
                        NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            try
            {
                if (dr.HasRows)
                {
                    dr.Read();
                    // 最初の1レコードをメンバー変数に読み込む
                    model = new T();
                    model.LoadProp(dr);

                }
                else
                {
                    model = null;
                }
                 

            }
            catch (Exception ex)
            {   //DBアクセスエラー
                // エラーログ出力
                errorMessage = ex.Message.ToString();
                CiLog cErrlog = new CiLog();    // ログ出力クラス
                cErrlog.WriteLog(ex.Message.ToString(), ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }
            finally
            {
                cmd.Dispose();
                dr.Close();
            }
            if (errorMessage != null)
            {
                conn.Close();
                throw new Exception(errorMessage);
            }
            return model;

        }

        /// <summary>
        /// テーブルから複数行読み込んで、modelクラスインスタンスの配列にロード
        /// </summary>
        /// <typeparam name="T">modelクラス（BaseModelの派生クラス）</typeparam>
        /// <param name="conn">DBへの接続インスタンス</param>
        /// <param name="sql">SQL文</param>
        /// <param name="count">インスタンス数。省略時は行の終わりまで</param>
        /// <returns>modelクラスのインスタンス</returns>
        public static T[] GetRows<T>(NpgsqlConnection conn, string sql, int count = int.MaxValue) where T : BaseModel, new()
        {
            List<T> Models = new List<T>();

            string errorMessage = null;
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            try
            {
                for (int i = 0; dr.Read() && (i < count); i++) {
                    T model = new T();
                    model.LoadProp(dr);
                    Models.Add(model);
                }
            }
            catch (Exception ex)
            {   //DBアクセスエラー
                // エラーログ出力
                errorMessage = ex.Message.ToString();
                CiLog cErrlog = new CiLog();    // ログ出力クラス
                cErrlog.WriteLog(ex.Message.ToString(), ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }
            finally
            {
                cmd.Dispose();
                dr.Close();
            }
            if (errorMessage != null)
            {
                conn.Close();
                throw new Exception(errorMessage);
            }
            return Models.ToArray();

        }



        /// <summary>
        /// テーブル行のフィールドの値を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr">テーブルの行を示すDataReader</param>
        /// <param name="fieldName">テーブルのフィールド名</param>
        /// <returns></returns>
        public T getValue<T>(NpgsqlDataReader dr, string fieldName)
        {
            try
            {
                int index = dr.GetOrdinal(fieldName);
                if (dr.IsDBNull(index))
                {
                    //  null
                    return default(T);

                }
                return (T)dr[index];

            }
            catch (Exception)
            {
                //  null
                return default(T);
            }

        }



        protected static void update(NpgsqlConnection conn, string sql)
        {
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



    }


}
