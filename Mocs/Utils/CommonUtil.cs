using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace Mocs.Utils
{
    public class CommonUtil
    {
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
            return Mocs.Properties.Settings.Default.Locale_Code;
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

    }
}
