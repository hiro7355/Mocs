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


        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.mu_stat_id = this.getValue<int>(dr, "mu_stat_id");
            this.mu_stat_enable = this.getValue<Int16>(dr, "mu_stat_enable");
            this.mu_stat_com = this.getValue<int>(dr, "mu_stat_com");
            this.mu_stat_errlevel = this.getValue<int>(dr, "mu_stat_errlevel");
            this.mu_stat_errcode = this.getValue<int>(dr, "mu_stat_errcode");
            this.mu_stat_ope_mode = this.getValue<Int16>(dr, "mu_stat_ope_mode");
        }
    }
}
