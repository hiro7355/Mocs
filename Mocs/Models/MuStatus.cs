using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    class MuStatus : BaseModel
    {
        public int mu_stat_id;          //  MU識別子
        public Int16 mu_stat_enable;    //  MU有効/無効 1:有効　<>1 無効  搬送ロボット無効はメイン画面のモニタープログラムから無効設定する。
        public int mu_stat_com;         //  MU通信状態  0:未接続、1:接続
        public int mu_stat_errlevel;    //  MUエラーレベル  0:異常なし、<>0:エラーレベル
        public int mu_stat_errcode;     //  MUエラーコード 0:異常なし、<>0:エラーコード
        public Int16 mu_stat_ope_mode;  //  MU運転モード 0:不明 1:オフライン自動 2:オンライン自動 3:手動
        public int mu_stat_muorder_status;
        public int mu_stat_order_id;          //  搬送オーダーID

        public Int16 mu_stat_hospital_id;   // MU位置病棟ID
        public Int16 mu_stat_floor_id;      //  MU位置フロアID
        public long mu_stat_pos_x;          //   MU位置X座標 bigint
        public long mu_stat_pos_y;          //   MU位置Y座標 bigint
        public int mu_stat_point_last;      // MU最新ポイントID



        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.mu_stat_id = this.getValue<int>(dr, "mu_stat_id");
            this.mu_stat_enable = this.getValue<Int16>(dr, "mu_stat_enable");
            this.mu_stat_com = this.getValue<int>(dr, "mu_stat_com");
            this.mu_stat_errlevel = this.getValue<int>(dr, "mu_stat_errlevel");
            this.mu_stat_errcode = this.getValue<int>(dr, "mu_stat_errcode");
            this.mu_stat_ope_mode = this.getValue<Int16>(dr, "mu_stat_ope_mode");
            this.mu_stat_muorder_status = this.getValue<Int16>(dr, "mu_stat_muorder_status");
            this.mu_stat_order_id = this.getValue<int>(dr, "mu_stat_order_id");
            this.mu_stat_hospital_id = this.getValue<Int16>(dr, "mu_stat_hospital_id");
            this.mu_stat_floor_id = this.getValue<Int16>(dr, "mu_stat_floor_id");
            this.mu_stat_pos_x = this.getValue<long>(dr, "mu_stat_pos_x");
            this.mu_stat_pos_y = this.getValue<long>(dr, "mu_stat_pos_y");
            this.mu_stat_point_last = this.getValue<int>(dr, "mu_stat_point_last");

        }


        /// <summary>
        /// 発生日時を取得するSQL
        /// ・mu_status_logテーブルを参照する。
        ///  mu_log_mu_id　= mu_stat_id and mu_log_errcode = mu_stat_errcodeの条件を満たす
        ///  最新レコードのMU状態変化日時(mu_log_datetime)が発生日時になります。
        /// </summary>
        /// <param name="mu_stat_id"></param>
        /// <returns></returns>
        private static string SelectDateTimeSql(int mu_stat_id, int mu_stat_errcode)
        {
            return "SELECT max(mu_log_datetime) as datetime FROM mu_status_log WHERE mu_log_mu_id=" + mu_stat_id  + " AND mu_log_errcode=" + mu_stat_errcode;
        }


        public static DateTime GetDateTime(NpgsqlConnection conn, int mu_stat_id, int mu_stat_errcode)
        {
            string sql = SelectDateTimeSql(mu_stat_id, mu_stat_errcode);
            return BaseModel.GetFirstValue<DateTime>(conn, sql, "datetime");
        }

    }
}
