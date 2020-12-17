using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Properties;
namespace Mocs.Utils
{
    class DateTimeUtil
    {
        public static string CurrentDateTimeString()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString(Resources.FORMAT_DATETIME);
        }
    }
}
