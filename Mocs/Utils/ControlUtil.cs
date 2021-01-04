using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mocs.Utils
{
    public class ControlUtil
    {
        /// <summary>
        /// スタックパネルにテキストブロックを追加
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="text"></param>
        public static void AddTextToStack(StackPanel stack, string text)
        {

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            stack.Children.Add(textBlock);
        }

    }
}
