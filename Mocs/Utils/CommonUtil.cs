﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using Mocs.Models;
using Npgsql;
using System.Reflection;
using System.Windows;

namespace Mocs.Utils
{
    public class CommonUtil
    {
        private static int m_LastDBError = 0;
        private static int m_LastSocketError = 0;
        private static DateTime m_lastSocekErrorDateTime;

        private static Action m_callbackCellRequestDone = null;     //  CELLとの通信完了（応答受信またはタイムアウト）時のコールバック

        private static string m_cell_name = "";      //  CELL + cell_statusのcellstat_id
        public static int GetLastDBError()
        {
            return m_LastDBError;
        }

        internal static void SetLastDBError(int hResult)
        {
            m_LastDBError = hResult;
        }

        public static void SetCallbackCellRequestDone(Action callback)
        {
            m_callbackCellRequestDone = callback;
        }
            

        public static int GetLastSocketError()
        {
            return m_LastSocketError;
        }

        public static void SetLastSocketError(int hResult)
        {
            m_LastSocketError = hResult;
            m_lastSocekErrorDateTime = DateTime.Now;

            if (m_callbackCellRequestDone != null)
            {
                //  通信完了をコールバック
                m_callbackCellRequestDone();
            }
        }
        public static DateTime GetLastSocketErrorDateTime()
        {
            return m_lastSocekErrorDateTime;
        }

        /// <summary>
        /// CELL名称を取得
        /// CELL + cell_statusのcellstat_id（３桁）
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string GetCellName(NpgsqlConnection conn)
        {
            if (m_cell_name == "")
            {
                //  cell_statusの読み込み。
                CellStatus cellStatus = CellStatus.GetFirst(conn);
                if (cellStatus != null)
                {
                    m_cell_name = "CELL" + String.Format("{0:D3}", cellStatus.cellstat_id);
                }

            }
            return m_cell_name;

        }



        /// <summary>
        /// アプリ固有のロケールコードをWindowsのロケールコードに変換
        /// 
        /// アプリ固有のロケールコードはDBのフィールド名に対応する
        /// jp:日本語、en:英語、cn:中国語
        /// 
        /// Windowsのロケールコードは以下を参照
        ///
        /// 
        /// </summary>
        /// <param name="appLocaleCode">アプリ固有のロケールコード</param>
        /// <returns></returns>
        public static string GetLocaleCodeFromAppLocaleCode(string appLocaleCode)
        {
            string localeCode = null;
            if (appLocaleCode == "jp")
            {
                localeCode = "ja";
            }
            else if (appLocaleCode == "en")
            {
                localeCode = "en-us";
            }
            else if (appLocaleCode == "cn")
            {
                localeCode = "zh-cn";
            }
            return localeCode;
        }


        /// <summary>
        /// アプリ固有のロケールコードを設定から取得
        /// </summary>
        /// <returns></returns>
        public static string GetAppLocaleCode()
        {
            string language = Mocs.Properties.Settings.Default.Language.ToLower();
            if (language == "japanese")
            {
                return "jp";
            }
            else if(language == "english")
            {
                return "en";
            }
            else if(language == "chinese")
            {
                return "cn";
            }
            return "jp";
        }


        /// <summary>
        /// アプリの設定に従いロケールを変更
        /// </summary>
        public static void SetLocale()
        {
            string localeCode = GetLocaleCodeFromAppLocaleCode(GetAppLocaleCode());

            if (localeCode != null)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(localeCode, false);
                CultureInfo.CurrentCulture = new CultureInfo(localeCode, false);
            }
        }

        /// <summary>
        /// カンマ区切り文字列の指定インデックスに対応する文字列を取得
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetValueFromCSV(string csv, int index)
        {
            string[] values = csv.Split(',');
            if (values.Length > index)
            {
                return values[index];
            } else
            {
                return "";
            }
        }

        /// <summary>
        /// メッセージ一覧の出力メッセージをフォーマット
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MessageFormat(DateTime dt, string message, string type = null)
        {
            if (type != null)
            {
                type = " " + type + ",";
            } 
            else
            {
                type = " ";
            }
            return DateTimeUtil.FormatDateTimeString(dt) + type + message;
        }


        public static string DBErrorCodeFormat(int error)
        {
            return "Error:" + error.ToString();
        }



        /// <summary>
        /// 異常状態のMUごとのエラーメッセージ用
        /// 
        /// Settings.settingsにmu_errorinfo_code_[level]_[code]という名前のキーで値がm_error_info_masterのmu_errorinfo_codeになるように設定されていることが条件
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="locale_code"></param>
        /// <param name="mu_id"></param>
        /// <param name="level"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static string MuErrorMessageFormat(NpgsqlConnection conn, string locale_code, int mu_id, int level, int code)
        {
            string message = null;
            string key = "mu_errorinfo_code_" + level + "_" + code;
            //            string value = (string)Properties.Settings.Default[key];      //  これだと簡単でいいけど、keyがないと例外エラーになってしまう
            string value = null;
            Type t = Properties.Settings.Default.GetType();
            PropertyInfo pi = t.GetProperty(key);
            if (pi != null)
            {
                value = (string)pi.GetValue(Properties.Settings.Default, BindingFlags.Default, null, null, null);
            }
            
            if (value != null)
            {
                int mu_errorinfo_code = Int32.Parse(value);
                //  mu_error_info_maste(DB)に定義されている異常メッセージを取得
                MuErrorInfoMaster muErrorInfoMaster = BaseModel.GetFirst<MuErrorInfoMaster>(conn, MuErrorInfoMaster.GetSql(locale_code, mu_errorinfo_code));

                if (muErrorInfoMaster != null)
                {
                    message = muErrorInfoMaster.mu_errinfo_msg;
                }
            }

            if (message == null)
            {
                //  エラー設定されていない場合は、エラーレベルとコードを表示
                message = String.Format(Properties.Resources.MSG_ERROR_NO_ERRORCODE_SETTING, level, code);
            }

            return message;
        }

        /// <summary>
        /// リストにタイトルと値の行を追加
        /// タイトルと値の行は、inde0にタイトル、index1に値が設定された配列（list）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="title"></param>
        /// <param name="value"></param>
        public static void addTitleValueToList(List<List<string>> list, string title, string value)
        {
            List<string> keyValueRow = new List<string>();
            keyValueRow.Add(title);
            keyValueRow.Add(value);

            list.Add(keyValueRow);
        }


        public static int parseInt(string number, int defaultValue)
        {
            int result = defaultValue;
            Int32.TryParse(number, out result);
            return result;
        }


        public static MessageBoxResult showErrorMessage(string message)
        {
            return MessageBox.Show(message, Mocs.Properties.Resources.MSGBOX_TITLE_ERROR, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        /// <summary>
        /// カンマ区切り文字列の最初の値と次の値をint値にして取得
        /// </summary>
        /// <param name="value">カンマ区切り文字列</param>
        /// <param name="stat_com"></param>
        /// <param name="ope_mode"></param>
        public static void GetValue1_2(string value, out int value1, out int value2)
        {
            string[] values = value.Split(',');

            value1 = Int32.Parse(values[0]);
            value2 = Int32.Parse(values[1]);
        }
    }


}
