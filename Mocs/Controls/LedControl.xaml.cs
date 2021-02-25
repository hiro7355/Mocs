using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// LedControl.xaml の相互作用ロジック
    /// </summary>
    public partial class LedControl : UserControl
    {
        private string[] m_messages;

        public string GetLastMessage(int index)
        {
            return m_messages[index];
        }
        public void SetLastMessage(int index, string message)
        {
            m_messages[index] = message;
        }

        public LedControl()
        {
            InitializeComponent();

            m_messages = new string[10];

        }

        public void SetColorFromName(String name)
        {

            this.Foreground = ColorUtil.brushFromColorName(name);

        }


    }
}
