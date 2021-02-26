using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// 搬送オーダー状態履歴
    /// </summary>
    public class OrderStatusLog : BaseModel
    {



        //        order_log_index オーダ履歴インデックス serial
        //        order_log_datetime  オーダ履歴日時 timestamp
        //order_log_id オーダID   integer
        //order_log_req_src   搬送オーダー要求元 varchar
        //order_log_req_sect 搬送オーダー要求部署  integer
        //order_log_priority  搬送オーダー優先順位 smallint
        public int order_log_mu_id;     //  MU識別子   integer
        public int order_log_cart_id;   //   カート識別子 integer
        public int order_log_from_sect;     // 搬送元部署ID integer　　発部署
        public int order_log_from_pt;   //   搬送元ポイントID integer  発ステーション
                                        //order_log_inspect_sects 搬送行先監査部署    varchar
                                        //order_log_inspect_points    搬送行先監査ポイント varchar
        public string order_log_stop_to_sects;  // 搬送行先立寄り部署   varchar　着部署
        public string order_log_stop_to_points; //    搬送行先立寄りポイント varchar　着ステーション
        public Int16 order_log_round_flg;   // 搬送行先巡回フラグ   smallint
                                            //order_log_type	"搬送オーダー種別"	integer
                                            //order_log_reserve_datetime オーダ予約受付日時   timestamp
                                            //order_log_start_datetime    搬送開始日時 timestamp
                                            //order_log_status オーダ予約処理ステータス    integer
                                            //order_log_wait_permission_flg   実行許可待ちフラグ smallint
                                            //order_log_cartdirection カート荷積み方向ステータス   smallint
        public string order_log_forward_list;   //  不在転送リスト varchar　着部署
//order_log_next_stop_sect 次回立寄り部署ID   integer
//order_log_next_stop_pt  次回立寄りポイント integer
//order_log_cancelable_flg 搬送キャンセル可能フラグ    smallint


        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.order_log_mu_id = this.getValue<int>(dr, "order_log_mu_id");
            this.order_log_cart_id = this.getValue<int>(dr, "order_log_cart_id");

            this.order_log_from_sect = this.getValue<int>(dr, "order_log_from_sect");
            this.order_log_from_pt = this.getValue<int>(dr, "order_log_from_pt");
            this.order_log_stop_to_sects = this.getValue<string>(dr, "order_log_stop_to_sects");
            this.order_log_stop_to_points = this.getValue<string>(dr, "order_log_stop_to_points");
            this.order_log_round_flg = this.getValue<Int16>(dr, "order_log_round_flg");
            this.order_log_forward_list = this.getValue<string>(dr, "order_log_forward_list");
        }

    }
}
