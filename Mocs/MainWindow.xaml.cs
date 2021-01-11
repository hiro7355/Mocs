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
namespace Mocs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        
        DBAccess m_db;

        /// <summary>
        /// converterからDBへアクセスできるようにするため
        /// </summary>
        /// <returns></returns>
        public DBAccess GetDBAccess()
        {
            return m_db;
        }

        public MainWindow(DBAccess db)
        {
            m_db = db;
            InitializeComponent();


            ErrorInfo errorInfo = new ErrorInfo();

            this.systemStatusControl.Init(m_db, errorInfo);
            this.muListControl.Init(m_db, errorInfo);
            this.errorInfoControl.Init(m_db, errorInfo);
            this.orderListControl.Init(m_db, errorInfo);
            this.cartListControl.Init(m_db);
            this.tabletListControl.Init(m_db);
            this.floorListControl.Init(m_db);
            this.stationListControl.Init(m_db);
            this.historyTabControl.Init(m_db);


        }

        private void ShowPannel(bool showSystemStatus, UserControl control)
        {


            if (showSystemStatus)
            {
                this.systemStatusPanel.Visibility = Visibility.Visible;
                this.historyTabControl.Visibility = Visibility.Collapsed;

                //  いったんシステム状態の隣のコントロールをすべて非表示にする
                this.hideAllNextSystemStatusControl();

                if (control != null)
                {
                    //  指定されたコントロールを表示する
                    control.Visibility = Visibility.Visible;

                }
            }
            else
            {
                this.systemStatusPanel.Visibility = Visibility.Collapsed;
                this.historyTabControl.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// システム状態の隣のコントロールをすべて非表示にする
        /// </summary>
        private void hideAllNextSystemStatusControl()
        {
            this.muListControl.Visibility = Visibility.Collapsed;
            this.errorInfoControl.Visibility = Visibility.Collapsed;
            this.cartListControl.Visibility = Visibility.Collapsed;
            this.tabletListControl.Visibility = Visibility.Collapsed;
            this.floorListControl.Visibility = Visibility.Collapsed;
            this.stationListControl.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// 履歴ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(false, null);

        }

        /// <summary>
        /// 異常情報ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void errorInfoButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, this.errorInfoControl);

        }

        /// <summary>
        /// カート一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cartListButton_Click(object sender, RoutedEventArgs e)
        {

            ShowPannel(true, this.cartListControl);
        }

        /// <summary>
        /// フロア一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void floorListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, floorListControl);

        }

        /// <summary>
        /// ステーション一覧ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stationListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, stationListControl);

        }

        /// <summary>
        /// タブレット一覧クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabletListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, tabletListControl);

        }

        private void deviceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, muListControl);

        }

        private void systemStatusButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPannel(true, muListControl);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //ログイン画面の表示
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Owner = GetWindow(this);
            bool? result = loginWindow.ShowDialog();
            if (result == false)
            {
                this.Close();
            }
            this.Show();
        }


        /// <summary>
        /// クローズメニュークリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        /// <summary>
        /// ヘルプメニュークリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new Microsoft.Win32.OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "PDFファイル (*.pdf)|*.pdf";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // pdfを開く
                System.Diagnostics.Process.Start(dialog.FileName);
            }
        }
    }
}
