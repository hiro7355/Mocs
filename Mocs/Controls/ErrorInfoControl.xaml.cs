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
        public ErrorInfoControl()
        {
            InitializeComponent();

        }

        protected override void Update()
        {
            //  CELLエラーを更新
            UpdateTreeViewItem(this.cellTreeViewItem, m_errorInfo.cellInfo);

            //  通信エラーを更新
            UpdateTreeViewItem(this.commuTreeViewItem, m_errorInfo.commuInfo);

            //  MUエラーを更新
            UpdateTreeViewItems(this.muTreeViewItem, m_errorInfo.muInfoByMu);
        }


        private void UpdateTreeViewItems(TreeViewItem parentItem, Dictionary<int, List<List<string>>> dict)
        {
            //  いったんすべての子アイテムを削除
            RemoveAllItems(parentItem);

            //  TODO: keyの順番でソート
            foreach (List<List<string>> titleValues in dict.Values)
            {

                //  アイテムを追加
                AddTreeViewItem(parentItem, titleValues);

            }

            ShowTreeViewItem(parentItem);
        }

        private void UpdateTreeViewItem(TreeViewItem parentItem, List<List<string>> titleValues)
        {
            //  いったんすべての子アイテムを削除
            RemoveAllItems(parentItem);

            //  アイテムを追加
            AddTreeViewItem(parentItem, titleValues);


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

        private void AddTreeViewItem(TreeViewItem parentItem, List<List<string>> titleValues)
        {
            if (titleValues.Count > 0)
            {
                StackPanel errorInfoStack = createErrorInfoStack(titleValues);

                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = errorInfoStack;
                childItem.Background = ColorUtil.brushFromColorName("Red");

                parentItem.Items.Add(childItem);

            }


        }

        private void RemoveAllItems(TreeViewItem item)
        {
            for (int i = item.Items.Count; i > 0; i--)
            {
                item.Items.RemoveAt(i-1);
            }
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
