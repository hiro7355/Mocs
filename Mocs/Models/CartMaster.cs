using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Mocs.Models
{
    /// <summary>
    /// カートマスタ
    /// </summary>
    public class CartMaster : BaseModel
    {
        public int cart_id;         //  カート識別子  integer
        public string cart_name;    //  カート名称 varchar
        public Int16 cart_active;   //  カート有効/無効 smallint
        public string cart_key;     //  カート暗証番号 varchar
        public Int16 cart_func;     //  カート機能属性 smallint


        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.cart_id = this.getValue<int>(dr, "cart_id");
            this.cart_name = this.getValue<string>(dr, "cart_name");
            this.cart_active = this.getValue<Int16>(dr, "cart_active");
            this.cart_key = this.getValue<string>(dr, "cart_key");
            this.cart_func = this.getValue<Int16>(dr, "cart_func");
        }
    }
}
