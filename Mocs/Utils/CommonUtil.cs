using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using Mocs.Models;
using Npgsql;
using System.Reflection;

namespace Mocs.Utils
{
    public class CommonUtil
    {
        private static int LastDBError;
        public static int GetLastDBError()
        {
            return LastDBError;
        }

        internal static void SetLastDBError(int hResult)
        {
            LastDBError = hResult;
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
        public static string MessageFormat(string message, string type = null)
        {
            if (type != null)
            {
                type = " " + type + ":";
            } 
            else
            {
                type = " ";
            }
            return DateTimeUtil.CurrentDateTimeString() + type + message;
        }


        public static string DBErrorCodeFormat(int error)
        {
            return "Error:DB " + error.ToString();
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


    }


}
