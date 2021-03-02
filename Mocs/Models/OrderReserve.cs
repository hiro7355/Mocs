using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Mocs.Utils;

namespace Mocs.Models
{
    class OrderReserve : BaseModel
    {

        public int order_mu_id;     //  MU識別子   integer
        public int order_cart_id;   //   カート識別子 integer
        public int order_from_sect;     // 搬送元部署ID integer　　発部署
        public int order_from_pt;   //   搬送元ポイントID integer  発ステーション
        public string order_stop_to_sects;  // 搬送行先立寄り部署   varchar　着部署
        public string order_stop_to_points; //    搬送行先立寄りポイント varchar　着ステーション
        public Int16 order_round_flg;   // 搬送行先巡回フラグ   smallint
        public string order_forward_list;   //  不在転送リスト varchar　着部署

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.order_mu_id = this.getValue<int>(dr, "order_mu_id");
            this.order_cart_id = this.getValue<int>(dr, "order_cart_id");

            this.order_from_sect = this.getValue<int>(dr, "order_from_sect");
            this.order_from_pt = this.getValue<int>(dr, "order_from_pt");
            this.order_stop_to_sects = this.getValue<string>(dr, "order_stop_to_sects");
            this.order_stop_to_points = this.getValue<string>(dr, "order_stop_to_points");
            this.order_round_flg = this.getValue<Int16>(dr, "order_round_flg");
            this.order_forward_list = this.getValue<string>(dr, "order_forward_list");
        }
    
        /// <summary>
        /// 発部署名と着部署名を取得
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        static public (string fromSectNames, string toSectNames) GetSectNames(NpgsqlConnection conn, int order_id)
        {
            string fromSectNames = null;     //  発部署名
            string toSectNames = null;      //  着部署名


            if (order_id != 0)
            {
                OrderReserve order = BaseModel.GetFirst<OrderReserve>(conn, "SELECT * FROM order_reserve WHERE order_id=" + order_id);

                if (order != null)
                {

                    string localeCode = CommonUtil.GetAppLocaleCode();

                    //  部署ID一覧から部署名をカンマ区切りで取得
                    fromSectNames = BaseModel.GetFirstValue<string>(conn, SectionMaster.SelectNamesSql(localeCode, order.order_from_sect.ToString()), "value");


                    string ids;
                    if (order.order_round_flg == 1 || order.order_forward_list == null)
                    {
                        ids = order.order_stop_to_sects;
                    }
                    else
                    {
                        ids = order.order_forward_list;
                    }

                    //  id一覧から部署名一覧に変換
                    toSectNames = SectionMaster.GetSectionNames(conn, ids);
                }

            }

            return (fromSectNames, toSectNames);
        }

    }
}
