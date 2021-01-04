using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Mocs.Models;
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// MuListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class MuListControl : TimerBaseControl
    {
        ObservableCollection<MessageInfo> m_messageList;
        Dictionary<int, string> m_mu_id_to_message_for_error; //  mu_idとmessageのディクショナリ（異常状態監視でmuごとにメッセージを表示するときに状態変化があったかどうか判断するのに利用）


        public MuListControl()
        {
            InitializeComponent();

            // 変更通知してくれるObservableCollectionを使用すると、コレクションに要素を追加・削除すると、自動的にListBoxにも反映します
            m_messageList = new ObservableCollection<MessageInfo>();
            this.messageList.ItemsSource = m_messageList;
            ResetMuError();

        }
        private void ResetMuError()
        {
            m_mu_id_to_message_for_error = new Dictionary<int, string>();

        }
        /// <summary>
        /// 異常状態チェックで
        /// MUエラーチェック
        /// </summary>
        private void UpdateErrorForMu()
        {
            string locale_code = CommonUtil.GetAppLocaleCode();

            MuStatus[] mu_statuses = BaseModel.GetRows<MuStatus>(m_db.Conn, "SELECT * FROM mu_status WHERE mu_stat_enable=1");

            foreach (MuStatus mu_status in mu_statuses)
            {
                string error_message = null;
                int mu_id = mu_status.mu_stat_id;
                if (mu_status.mu_stat_com == 0)
                {
                    //  Wifi未接続
                    error_message = Properties.Resources.MSG_ERROR_NO_WIFI;

                }
                else if (mu_status.mu_stat_errlevel != 0 || mu_status.mu_stat_errcode != 0)
                {
                    //  levelかcodeに異常がある場合はメッセージを取得
                    error_message = CommonUtil.MuErrorMessageFormat(m_db.Conn, locale_code, mu_id, mu_status.mu_stat_errlevel, mu_status.mu_stat_errcode);

                }


                bool do_update = false;
                bool is_success = false;
                //  メッセージに更新があるかチェック
                if (this.m_mu_id_to_message_for_error.ContainsKey(mu_id))
                {
                    if (this.m_mu_id_to_message_for_error[mu_id] != error_message)
                    {
                        do_update = true;

                        if (error_message == null)
                        {
                            //  正常に復帰したとき
                            error_message = Properties.Resources.MSG_ERROR_MU_RECOVER;
                            is_success = true;
                        }
                    }
                }
                else
                {
                    if (error_message != null)
                    {
                        //  エラーが発生している場合のみメッセージ追加
                        do_update = true;
                    }
                }

                if (do_update)
                {
                    MuMaster muMaster = BaseModel.GetFirst<MuMaster>(m_db.Conn, MuMaster.GetSql(locale_code, mu_id));

                    if (muMaster != null)
                    {
                        //  メッセージを追加
                        this.addMessage(is_success ? "White" : "Red", error_message, muMaster.mu_name);


                        string message = is_success ? null : error_message;

                        //  次回更新チェック用にメッセージを保存
                        this.m_mu_id_to_message_for_error[mu_id] = message;


                        //  異常情報ようにエラー情報設定 (正常に復帰したときはmessageはnull)
                        m_errorInfo.UpdateMuError(mu_id, muMaster.mu_name, message);
                    }

                }

            }

        }
        private void addMessage(String bgColorName, String message, String type)
        {
            m_messageList.Insert(0, new MessageInfo(ColorUtil.brushFromColorName(bgColorName), CommonUtil.MessageFormat(message, type)));
        }


        protected override void Update()
        {
            string sql = SqlForMuList.GetListSql();

            System.Data.DataTable table = m_db.getDataTable(sql);

            muList.DataContext = table;

            this.UpdateErrorForMu();
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;

            DataRowView rView = row.Item as DataRowView;

            if (rView != null)
            {
                Brush newBrush = null;

                Object[] values = rView.Row.ItemArray;

                int ope_mode = Int16.Parse(values[1].ToString());
                int muorder_status = Int16.Parse(values[7].ToString());

                if (ope_mode == 0 || ope_mode == 1)
                {
                    newBrush = Brushes.White;
                }
                else if (ope_mode == 2)
                {
                    if (muorder_status == 0 || muorder_status == 10 || muorder_status == 11)
                    {
                        newBrush = Brushes.White;
                    }
                    else if (muorder_status == 1)
                    {
                        newBrush = Brushes.Gray;
                    }
                    else if (muorder_status == 13 || muorder_status == 20 || muorder_status == 21)
                    {
                        newBrush = Brushes.Orange;
                    }
                    else if (muorder_status == 12 || muorder_status == 90)
                    {
                        newBrush = Brushes.Red;
                    }
                }
                else if(ope_mode == 4)
                {
                    newBrush = Brushes.Yellow;

                }

                if (newBrush == null)
                {
                    //  上記にあてはまらないときは白にする
                    newBrush = Brushes.White;
                }

                row.Background = newBrush;

            }
        }
    }
}
