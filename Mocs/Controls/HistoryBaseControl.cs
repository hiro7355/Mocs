using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Mocs.Models;
using Mocs.Utils;

namespace Mocs.Controls
{
    abstract public class HistoryBaseControl : UserControl
    {
        protected DBAccess m_db;


        public void Init(DBAccess db)
        {
            this.m_db = db;
            this.Update();
        }

        /// <summary>
        /// 派生クラスで実装すること
        /// init時に呼び出される
        /// </summary>
        abstract protected void Update();

    }
}
