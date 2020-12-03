using System;
//using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Mocs.Utils
{
    /// <summary>
    /// ログ出力クラス
    ///   2017/03/17: Oguchi
    /// </summary>
    class CiLog
    {
#region "共通変数/定数定義"

        private string outLogDir;
        private string outFilePrefix;

        object syncObject = new object();      // ファイル排他制御用オブジェクト

        // 同じメッセージを表示しないように、前回のメッセージを格納
        private string beforErrMessage;
        // Prefix
        private const string ErrLogPrefix = "Err";

#endregion

#region "パブリックメッソッド"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name=""></param>
        public CiLog()
        {
            outLogDir = Mocs.Properties.Settings.Default.ErrLogDirectory;
            outFilePrefix = ErrLogPrefix;
        }

        //public CiLog(formMain fMainPar)
        //{
        //    fMain = fMainPar;
        //}

        /// <summary>
        /// ﾛｸﾞ作成
        /// </summary>
        /// <param name="sLogMessage">ﾛｸﾞﾒｯｾｰｼﾞ</param>
        /// <param name="inStackTrace">スタックトレース</param>
        /// <param name="eventType">ｲﾍﾞﾝﾄ種別</param>
        public void WriteLog(string sLogMessage, string inStackTrace, System.Diagnostics.EventLogEntryType eventType)
        {
            try
            {

                //----- メインフォームのListBoxに表示
//                formMain.Form1Instance.listBoxErrText = sLogMessage;
            }
            catch (Exception ex)
            {
                // ﾛｸﾞﾌｧｲﾙ出力
                this.WriteLogFile(ex.Message);
            }

            if (Mocs.Properties.Settings.Default.ErrLogOutMode == "ON")
            {
                // ﾛｸﾞﾌｧｲﾙ出力(メッセージとスタックトレースを出力)
                this.WriteLogFile(sLogMessage + " : " + inStackTrace);
            }
        }


        /// <summary>
        /// ﾛｸﾞﾌｧｲﾙ書き込み
        /// </summary>
        /// <param name="sLogMessage">書き込みﾒｯｾｰｼﾞ</param>
        public void WriteLogFile(string sMsg)
        {

            try
            {
                if (Mocs.Properties.Settings.Default.ErrLogOutMode == "OFF")
                {
                    return;
                }
                beforErrMessage = sMsg;
            }
            catch ( Exception)
            {
                //EventLog ciEvents = new EventLog();
                //ciEvents.Source = "EigwCheck";
                //ciEvents.WriteEntry(ex.Message, EventLogEntryType.Error);

                return;
            }

            StreamWriter sw;
            try
            {
                string sFileName;
                string sWtMsg;

                // ファイル排他制御追加
                Monitor.Enter(syncObject); // ロック取得
                try
                {
                    // フォルダ (ディレクトリ) が存在しているかどうか確認する
                    if (System.IO.Directory.Exists(outLogDir) == false)
                    {
                        // フォルダ (ディレクトリ) を作成する
                        System.IO.Directory.CreateDirectory(outLogDir);
                    }

                    //フォルダ検索
                    sFileName = outLogDir + "/" + outFilePrefix + "_" + DateTime.Now.ToString("dd") + ".Log";
                    
                    // データ書き込み
                    sWtMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "," + sMsg;

                    if ((File.Exists(sFileName)) == false || (File.GetLastWriteTime(sFileName).Date != DateTime.Now.Date))
                    {
                        sw = File.CreateText(sFileName);
                        sw.Close();
                    }

                    sw = File.AppendText(sFileName);
                    sw.WriteLine(sWtMsg);
                    sw.Close();
                }
                finally
                {
                    Monitor.Exit(syncObject); // ロック解放
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be write:");
                Console.WriteLine(ex.Message);

                // 2016/05/26 Delete
                //EventLog ciEvents = new EventLog();
                //ciEvents.Source = "EigwCheck";
                //ciEvents.WriteEntry(ex.Message, EventLogEntryType.Error);
                throw ex;
            }
            finally
            {
            }
        }
    }
#endregion
}   
    
