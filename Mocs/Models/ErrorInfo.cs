using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocs.Utils;
using Mocs.Properties;
using System.Windows.Media;

namespace Mocs.Models
{
    public class ErrorInfo
    {
//        private List<List<string>> m_cellInfo;      //  CELLエラー表示情報
//        private List<List<string>> m_commuInfo;     //  ソケット通信エラー表示情報
//        private Dictionary<int, List<List<string>>> m_muInfoByMu;//  MUごとのMUエラー表示情報

        private List<ErrorInfoItem> m_cellInfos;      //  CELLエラー表示情報 （エラー発生時刻と）
        private List<ErrorInfoItem> m_commuInfos;     //  ソケット通信エラー表示情報
        private List<ErrorInfoItem> m_muInfos;//  MUごとのMUエラー表示情報


        public List<ErrorInfoItem> cellInfos { get { return m_cellInfos; } }
        public List<ErrorInfoItem> commuInfos { get { return m_commuInfos; } }
        public List<ErrorInfoItem> muInfos { get { return m_muInfos; } }

        private Brush m_red;
        public ErrorInfo()
        {
            ResetCellInfo();
            ResetCommuInfo();
            ResetMuInfo();

            m_red = Brushes.Red;

        }

        public void ResetCellInfo()
        {
            m_cellInfos = new List<ErrorInfoItem>();
        }
        public void ResetCommuInfo()
        {
            m_commuInfos = new List<ErrorInfoItem>();
        }

        public void ResetMuInfo()
        {
            m_muInfos = new List<ErrorInfoItem>();
        }

        /// <summary>
        /// CELLのエラーを設定
        /// CELLエラー発生時によびだされる
        /// </summary>
        public void UpdateCellError(DateTime dt, string message, string coping, Brush brush)
        {
            ErrorInfoItem item = new ErrorInfoItem();
            item.SetBrush(brush);
            item.SetSortKey(dt);

            item.AddTitleValue("CELL", message);
            item.AddTitleValue(Resources.DATETIME, DateTimeUtil.FormatDateTimeString(dt));
            item.AddTitleValue(Resources.COPING, coping);

            m_cellInfos.Add(item);

        }

        /// <summary>
        /// socket通信エラーを設定
        /// </summary>
        public void UpdateCommuError(DateTime dt, string message, int errorType)
        {
            ErrorInfoItem item = new ErrorInfoItem();
            item.SetBrush(m_red);
            item.SetSortKey(dt);
            item.SetErrorType(errorType);

//            string message = Resources.SOCKET_ERROR;
            string coping = Resources.CHECK_CELL;

            item.AddTitleValue("CELL", message);
            item.AddTitleValue(Resources.DATETIME, DateTimeUtil.FormatDateTimeString(dt));
            item.AddTitleValue(Resources.COPING, coping);

            m_commuInfos.Add(item);

        }

        /// <summary>
        /// タイプの一致するエラーを削除
        /// </summary>
        /// <param name="errorType"></param>
        public void RemoveCommErrorByType(int errorType)
        {
            List<ErrorInfoItem> ar = new List<ErrorInfoItem>();

            foreach (ErrorInfoItem item in m_commuInfos)
            {
                if (item.GetErrorType() == errorType)
                {
                    ar.Add(item);
                }
            }

            foreach(ErrorInfoItem item in ar)
            {
                m_commuInfos.Remove(item);
            }
        }


        public void UpdateMuError(DateTime dt, Brush brush, int mu_id, string mu_name, string error_message, string fromSectNames, string toSectNames, string hospital_name, string floor_name, long mu_stat_pos_x, long mu_stat_pos_y, string point_name)
        {

            ErrorInfoItem item = new ErrorInfoItem();
            item.SetBrush(brush);
            item.SetSortKey(dt);

            string message = Resources.SOCKET_ERROR;
            string coping = Resources.CHECK_CELL;

            item.AddTitleValue(mu_name, error_message);
            item.AddTitleValue(Resources.OCCURENCE_DATETIME, DateTimeUtil.FormatDateTimeString(dt));
            //  発部署
            if (fromSectNames != null)
            {
                item.AddTitleValue(Resources.EHH_REQ_SECT, fromSectNames);
            }
            //  着部署
            if (toSectNames != null)
            {
                item.AddTitleValue(Resources.EHH_TO_SECT, toSectNames);
            }

            //  棟名称-フロア
            if (hospital_name != null && floor_name != null)
            {
                string title = hospital_name + "-" + floor_name;
                string value = "X=" + mu_stat_pos_x + "," + "Y=" + mu_stat_pos_y;
                item.AddTitleValue(title, value);
            }

            //  通過ポイント
            if (point_name != null)
            {
                item.AddTitleValue(Resources.PASSING_POINT, point_name);
            }

            m_muInfos.Add(item);

            /*
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
                CommonUtil.addTitleValueToList(muInfo, Resources.OCCURENCE_DATETIME, DateTimeUtil.CurrentDateTimeString());
                //  TODO: 発部署

                //  TODO: 棟名称-フロア

                //  TODO:通過ポイント
                string point_name = "";
                CommonUtil.addTitleValueToList(muInfo, Resources.PASSING_POINT, point_name);


                m_muInfoByMu.Add(mu_id, muInfo);

            }
            */

        }


    }
}
