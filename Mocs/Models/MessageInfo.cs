using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Mocs.Utils;

namespace Mocs.Models
{
    public class MessageInfo
    {
        public Brush brush { get; set; }
        public string message { get; set; }

        private string m_raw_message;
        private string m_type;

        public MessageInfo(DateTime dt, Brush brush, string raw_message, string type)
        {
            this.brush = brush;
            m_raw_message = raw_message;
            m_type = type;


            this.message = CommonUtil.MessageFormat(dt, raw_message, type);
        }


        /// <summary>
        /// 同じエラーかチェック
        /// </summary>
        /// <param name="info"></param>
        public bool isSame(MessageInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return (this.m_raw_message == info.m_raw_message && this.m_type == info.m_type) ? true : false;
        }
    }
}
