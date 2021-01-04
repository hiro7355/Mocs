using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;
using Mocs.Properties;

namespace Mocs.Models
{
    public class ErrorInfo
    {
        private List<List<string>> m_cellInfo;      //  CELLエラー表示情報
        private List<List<string>> m_commuInfo;     //  ソケット通信エラー表示情報
        private Dictionary<int, List<List<string>>> m_muInfoByMu;//  MUごとのMUエラー表示情報


        public List<List<string>> cellInfo { get { return m_cellInfo; } }
        public List<List<string>> commuInfo { get { return m_commuInfo; } }
        public Dictionary<int, List<List<string>>> muInfoByMu { get { return m_muInfoByMu; } }

        public ErrorInfo()
        {
            ResetCellAndCommuInfo();
            ResetMuInfo();
        }

        public void ResetCellAndCommuInfo()
        {
            m_cellInfo = new List<List<string>>();
            m_commuInfo = new List<List<string>>();
        }

        public void ResetMuInfo()
        {
            m_muInfoByMu = new Dictionary<int, List<List<string>>>();
        }

        /// <summary>
        /// CELLのエラーを設定
        /// CELLエラー発生時によびだされる
        /// </summary>
        public void UpdateCellError()
        {
            CommonUtil.addTitleValueToList(m_cellInfo, "CELL", Resources.CELL_STOPPED);
            CommonUtil.addTitleValueToList(m_cellInfo, Resources.DATETIME, DateTimeUtil.CurrentDateTimeString());
            CommonUtil.addTitleValueToList(m_cellInfo, Resources.COPING, Resources.START_CELL);

        }

        /// <summary>
        /// socket通信エラーを設定
        /// </summary>
        public void UpdateCommuError()
        {
            CommonUtil.addTitleValueToList(m_cellInfo, "CELL", Resources.SOCKET_ERROR);
            CommonUtil.addTitleValueToList(m_cellInfo, Resources.DATETIME, DateTimeUtil.CurrentDateTimeString());
            CommonUtil.addTitleValueToList(m_cellInfo, Resources.COPING, Resources.CHECK_CELL);
        }


        public void UpdateMuError(int mu_id, string mu_name, string error_message)
        {
            if (error_message == null)
            {
                //  正常のとき

                if (m_muInfoByMu.ContainsKey(mu_id))
                {
                    //  muを削除
                    m_muInfoByMu.Remove(mu_id);
                }
            }
            else
            {
                //  エラー発生
                List<List<string>> muInfo = new List<List<string>>();


                CommonUtil.addTitleValueToList(muInfo, mu_name, error_message);
                CommonUtil.addTitleValueToList(m_cellInfo, Resources.OCCURENCE_DATETIME, DateTimeUtil.CurrentDateTimeString());
                //  TODO: 発部署

                //  TODO: 棟名称-フロア

                //  TODO:通過ポイント
                string point_name = "";
                CommonUtil.addTitleValueToList(m_cellInfo, Resources.PASSING_POINT, point_name);

            }
        }


    }
}
