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
using Mocs.Models;
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// ErrorInfoControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorInfoControl : TimerBaseControl
    {
        // イベントを定義
        public event EventHandler OnCellStatus;


        private int m_errorLevel;

        private Brush m_red;
        private Brush m_black;
        private Brush m_white;

        public ErrorInfoControl()
        {
            InitializeComponent();

            m_red = Brushes.Red;
            m_black = Brushes.Black;
            m_white = Brushes.White;


        }

        private void UpdateErrorLevel(int level)
        {
            if (level > m_errorLevel)
            {
                m_errorLevel = level;
            }
        }

        /// <summary>
        /// エラー状態を取得
        /// </summary>
        /// <returns>0:エラーは発生していない。1:異常状態。2:警告状態
        /// </returns>
        public  int GetErrorLevel()
        {
            return m_errorLevel;

        }

        protected override void Update()
        {
            m_errorLevel = 0;

            //  CELLエラーを更新
            UpdateTreeViewItems(this.cellTreeViewItem, m_errorInfo.cellInfos);

            //  通信エラーを更新
            UpdateTreeViewItems(this.commuTreeViewItem, m_errorInfo.commuInfos);

            //  MUエラーを更新
            UpdateTreeViewItems(this.muTreeViewItem, m_errorInfo.muInfos);

            //  親に通知
            OnCellStatus(this, EventArgs.Empty);

        }


        private void UpdateTreeViewItems(TreeViewItem parentItem, List<ErrorInfoItem> errorInfos)
        {
            //  いったんすべての子アイテムを削除
            RemoveAllItems(parentItem);

            //  ソート
            errorInfos.Sort((a, b) => b.GetSortKey().CompareTo(a.GetSortKey()));


            foreach (ErrorInfoItem errorInfo in errorInfos)
            {

                //  アイテムを追加
                AddTreeViewItem(parentItem, errorInfo);

            }

            ShowTreeViewItem(parentItem);
        }


        private void ShowTreeViewItem(TreeViewItem parentItem)
        {

            if (parentItem.Items.Count > 0)
            {
                parentItem.IsExpanded = true;
                parentItem.Visibility = Visibility.Visible;

            }
            else
            {
                //  一つもエラーがないときは非表示
                parentItem.Visibility = Visibility.Collapsed;

            }
        }

        private void AddTreeViewItem(TreeViewItem parentItem, ErrorInfoItem errorInfo)
        {
            

            List<List<string>> titleValues = errorInfo.GetTitleValues();
            if (titleValues.Count > 0)
            {
                StackPanel errorInfoStack = createErrorInfoStack(titleValues);

                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = errorInfoStack;


                Brush brush = errorInfo.GetBrush();
                childItem.Background = brush;

//                childItem.Foreground = m_white;

                parentItem.Items.Add(childItem);

                parentItem.Foreground = brush;


                if (brush == Brushes.Orange)
                {
                    UpdateErrorLevel(1);
                } 
                else
                {
                    UpdateErrorLevel(2);
                }

            }


        }

        private void RemoveAllItems(TreeViewItem item)
        {
            for (int i = item.Items.Count; i > 0; i--)
            {
                item.Items.RemoveAt(i-1);
            }
            item.Foreground = m_black;
        }

        private StackPanel createErrorInfoStack(List<List<string>> titleValues)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            StackPanel titleStack = new StackPanel();
            StackPanel valueStack = new StackPanel();

            foreach (List<string> titleValue in titleValues)
            {
                string title = titleValue[0];
                string value = titleValue[1];

                ControlUtil.AddTextToStack(titleStack, title);

                ControlUtil.AddTextToStack(valueStack, ":" + value);

            }

            stack.Children.Add(titleStack);
            stack.Children.Add(valueStack);


            return stack;

        }




    }
}
