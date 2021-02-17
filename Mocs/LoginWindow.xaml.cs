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
using System.Windows.Shapes;

namespace Mocs
{
    /// <summary>
    /// LoginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            //  ロゴ画像を設定ファイルに指定されているパスから読み込む
            string logoFilePath = Mocs.Properties.Settings.Default.LogoFile;
            if (System.IO.File.Exists(logoFilePath))
            {
                logoFilePath = System.IO.Path.GetFullPath(logoFilePath);
                this.logo.Source = new BitmapImage(new Uri(logoFilePath));
            }
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Mocs.Properties.Settings.Default.PW == textPassword.Password)
            {
                this.DialogResult = true;
            } 
            else
            {
                Mocs.Utils.CommonUtil.showErrorMessage(Mocs.Properties.Resources.ERROR_PASSWORD);

            }

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }
    }
}
