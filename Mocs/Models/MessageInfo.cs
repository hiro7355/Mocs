using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mocs.Models
{
    public class MessageInfo
    {
        public Brush brush { get; set; }
        public string message { get; set; }

        public MessageInfo(Brush brush, string message)
        {
            this.brush = brush;
            this.message = message;
        }
    }
}
