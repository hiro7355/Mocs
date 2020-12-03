using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    public class CellStatus : BaseModel
    {

        public int cellstat_id;                     //  
        public short cellstat_status;               //  CELL状態
        public short cellstat_level;                //  CELL状態レベル
        public short cellstat_mu_status;            //   MU状態要約
        public int cellstat_mu_err;                 // エラーMU識別子
        public DateTime cellstat_stat_update_datetime;   //   CELL状態更新日時

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.cellstat_id = this.getValue<int>(dr, "cellstat_id");
            this.cellstat_status = this.getValue<short>(dr, "cellstat_status");
            this.cellstat_level = this.getValue<short>(dr, "cellstat_level");
            this.cellstat_mu_status = this.getValue<short>(dr, "cellstat_mu_status");
            this.cellstat_mu_err = this.getValue<int>(dr, "cellstat_mu_err");
            this.cellstat_stat_update_datetime = this.getValue<DateTime>(dr, "cellstat_stat_update_datetime");
        }

        /// <summary>
        /// ステータスを取得
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CellStatus GetFirst(NpgsqlConnection conn)
        {
            return GetFirst<CellStatus>(conn, "select * from cell_status where cellstat_id=1");
        }

        /// <summary>
        /// テーブルを初期化
        /// ID１の行に０を設定
        /// </summary>
        /// <param name="conn"></param>
        public static void InitTable(NpgsqlConnection conn)
        {
            //  
            string sql = "INSERT INTO cell_status VALUES (1,0,0,0,0,CURRENT_TIMESTAMP ) ON CONFLICT ON CONSTRAINT cell_status_pkey DO UPDATE SET cellstat_status = 0, cellstat_level = 0, cellstat_mu_status = 0, cellstat_mu_err=0, cellstat_stat_update_datetime=CURRENT_TIMESTAMP ; ";
            update(conn, sql);

        }

    }
}
