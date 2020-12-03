using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Net;

namespace Mocs.Models
{
    public class RobotWifiLog : BaseModel
    {
        public int robot_wifi_index;        //  インデックス  serial
        public Int16 robot_wifi_code;       // 電文区分 smallint
        public int robot_wifi_order_id;     // オーダコード  varchar
        public string robot_wifi_id;                            //   搬送ロボット識別子 varchar
        public ValueTuple<IPAddress, int> robot_wifi_ipaddr;    // 搬送ロボットIPアドレス    varchar
        public int robot_wifi_port;         // 通信ポート番号 integer
        public DateTime robot_wifi_comdate; // 通信日 date
        public TimeSpan robot_wifi_comtime; //  通信時間 time
        public Int16 robot_wifi_kubun;      // 通信区分    smallint
        public string robot_wifi_data;      // 送受信電文 text

        public override void LoadProp(NpgsqlDataReader dr)
        {
            this.robot_wifi_index = this.getValue<int>(dr, "robot_wifi_index");
            this.robot_wifi_code = this.getValue<Int16>(dr, "robot_wifi_code");
            this.robot_wifi_order_id = this.getValue<int>(dr, "robot_wifi_order_id");
            this.robot_wifi_id = this.getValue<string>(dr, "robot_wifi_id");
            this.robot_wifi_ipaddr = this.getValue<ValueTuple<IPAddress, int>>(dr, "robot_wifi_ipaddr");
            this.robot_wifi_port = this.getValue<int>(dr, "robot_wifi_port");
            this.robot_wifi_comdate = this.getValue<DateTime>(dr, "robot_wifi_comdate");
            this.robot_wifi_comtime = this.getValue<TimeSpan>(dr, "robot_wifi_comtime");
            this.robot_wifi_kubun = this.getValue<Int16>(dr, "robot_wifi_kubun");
            this.robot_wifi_data = this.getValue<string>(dr, "robot_wifi_data");
        }

        /// <summary>
        /// 日付範囲ないの一覧を取得
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static RobotWifiLog[] getRowsBetweenDate(NpgsqlConnection conn, DateTime start, DateTime end)
        {
            return BaseModel.GetRows<RobotWifiLog>(conn, @"select * from robot_wifi_log where " + getConditionBetweenDate(start, end));
        }

        /// <summary>
        /// 日付範囲ないの重複しないオーダーコード一覧を取得
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static RobotWifiLog[] getUniqOrderIdsBetweenDate(NpgsqlConnection conn, DateTime start, DateTime end)
        {
            return BaseModel.GetRows<RobotWifiLog>(conn, @"select robot_wifi_order_id from robot_wifi_log where " + getConditionBetweenDate(start, end) + @" group by robot_wifi_order_id");
        }

        private static string getConditionBetweenDate(DateTime start, DateTime end)
        {
            return @" robot_wifi_comdate >= '" + start.ToString("yyyy-MM-dd") + @"' and robot_wifi_comdate <= '" + end.ToString("yyyy-MM-dd") + @"'";
        }
    }
}
