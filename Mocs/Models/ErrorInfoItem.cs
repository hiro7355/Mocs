using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mocs.Models
{
    public class ErrorInfoItem
    {
        DateTime m_sort_key;
        List<List<string>> m_title_value_list;
        Brush m_brush;

        public ErrorInfoItem()
        {
            m_title_value_list = new List<List<string>>();

        }

        public List<List<string>> GetTitleValues()
        {
            return m_title_value_list;
        }


        public void AddTitleValue(string title, string value)
        {
            List<string> title_value = new List<string>();
            title_value.Add(title);
            title_value.Add(value);

            m_title_value_list.Add(title_value);

        }

        public void SetSortKey(DateTime sort_key)
        {
            m_sort_key = sort_key;
        }

        public DateTime GetSortKey()
        {
            return m_sort_key;
        }

        public void SetBrush(Brush brush)
        {
            m_brush = brush;
        }

        public Brush GetBrush()
        {
            return m_brush;
        }
    }
}
