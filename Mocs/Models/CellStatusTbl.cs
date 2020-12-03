using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    public class CellStatusTbl : BaseModel
    {
        public int cell_index;          //  インデックス  serial
        public Int16 cellsys_status;    //  システム状態ステータス smallint
        public Int16 cellsys_slevel;    //  CELL状態レベル   smallint
        public Int16 cellsys_robot;     //   搬送ロボットステータス smallint

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.cell_index = this.getValue<int>(dr, "cell_index");
            this.cellsys_status = this.getValue<Int16>(dr, "cellsys_status");
            this.cellsys_slevel = this.getValue<Int16>(dr, "cellsys_slevel");
            this.cellsys_robot = this.getValue<Int16>(dr, "cellsys_robot");
        }

        /// <summary>
        /// ステータスを取得
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CellStatusTbl GetFirst(NpgsqlConnection conn)
        {
            return GetFirst<CellStatusTbl>(conn, "select * from cell_status_tbl where cell_index=1");
        }

        /// <summary>
        /// テーブルを初期化
        /// ID１の行に０を設定
        /// </summary>
        /// <param name="conn"></param>
        public static void InitTable(NpgsqlConnection conn)
        {
            //  
            string sql = "INSERT INTO cell_status_tbl VALUES (1,0,0,0) ON CONFLICT ON CONSTRAINT cell_status_tbl_pkey DO UPDATE SET cellsys_status = 0, cellsys_slevel = 0, cellsys_robot = 0; ";
            update(conn, sql);

        }

    }
}
