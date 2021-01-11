using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using Mocs.Models;

namespace Mocs.Controls.Converters
{
    class BaseConverter
    {
        protected DBAccess GetDBAccess()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            return mainWindow.GetDBAccess();
        }


        protected T GetValue<T>(string sql, string valueFieldName)
        {
            DBAccess db = GetDBAccess();

            return BaseModel.GetFirstValue<T>(db.Conn, sql, valueFieldName);
            
        }

    }
}
