using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// order_idをキー、OrderReserveを値とするディクショナリ
    /// </summary>
    public class OrderReserveDictionary : Dictionary<int, OrderReserve>
    {
        public OrderReserveDictionary(NpgsqlConnection conn)
        {
            this.Load(conn);
        }

        public void Load(NpgsqlConnection conn)
        {
            OrderReserve[] rows = BaseModel.GetRows<OrderReserve>(conn, "select * from order_reserve");

            foreach (OrderReserve row in rows)
            {
                this.Add(row.order_id, row);
            }
        }
    }


    public class OrderReserve : BaseModel
    {
        public int order_id;                //  オーダコード  integer
        public string order_req_src;        // オーダー要求元 varchar
        public Int16 order_priority;        //  オーダ優先順位 smallint
        public int order_robot_id;       //  搬送ロボット識別子 varchar
        public int order_cart_id;        // カート識別コード    varchar
        public string order_cart_key;       //  搬送要求のカート暗証番号 varchar
        public int order_from_st;           //	"オーダ搬送FROMステーションID"	integer
        public int order_to_st;             //	"オーダ搬送TOステーションID"	integer
        public int order_from_pt;           //	"オーダ搬送FROMポイントID"	integer
        public int order_to_pt;             //	"オーダ搬送TOポイントID"	integer
        public int order_kind;              //	"オーダ搬送 区分"	integer
        public Int16 order_loding_type;     // オーダ荷積み区分    smallint
        public Int16 order_unloading_type;  //    オーダ荷降し区分 smallint
        public DateTime order_reserve_date;    // オーダ予約受付日    date
        public TimeSpan order_reserve_time;    //  オーダ予約受付時間 time
        public int order_status;            // オーダ予約処理ステータス    integer
        public Int16 order_cartdirection;   // カート荷積み方向ステータス smallint

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.order_id = this.getValue<int>(dr, "order_id");
            this.order_req_src = this.getValue<string>(dr, "order_req_src");
            this.order_priority = this.getValue<Int16>(dr, "order_priority");
            this.order_robot_id = this.getValue<int>(dr, "order_robot_id");
            this.order_cart_id = this.getValue<int>(dr, "order_cart_id");
            this.order_cart_key = this.getValue<string>(dr, "order_cart_key");
            this.order_from_st = this.getValue<int>(dr, "order_from_st");
            this.order_to_st = this.getValue<int>(dr, "order_to_st");
            this.order_from_pt = this.getValue<int>(dr, "order_from_pt");
            this.order_to_pt = this.getValue<int>(dr, "order_to_pt");
            this.order_kind = this.getValue<int>(dr, "order_kind");
            this.order_loding_type = this.getValue<Int16>(dr, "order_loding_type");
            this.order_unloading_type = this.getValue<Int16>(dr, "order_unloading_type");
            this.order_reserve_date = this.getValue<DateTime>(dr, "order_reserve_date");
            this.order_reserve_time = this.getValue<TimeSpan>(dr, "order_reserve_time");
            this.order_status = this.getValue<int>(dr, "order_status");
            this.order_cartdirection = this.getValue<Int16>(dr, "order_cartdirection");
        }
    }
}
