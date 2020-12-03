using System;
//using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Mocs.Utils
{
    /// <summary>
    /// ���O�o�̓N���X
    ///   2017/03/17: Oguchi
    /// </summary>
    class CiLog
    {
#region "���ʕϐ�/�萔��`"

        private string outLogDir;
        private string outFilePrefix;

        object syncObject = new object();      // �t�@�C���r������p�I�u�W�F�N�g

        // �������b�Z�[�W��\�����Ȃ��悤�ɁA�O��̃��b�Z�[�W���i�[
        private string beforErrMessage;
        // Prefix
        private const string ErrLogPrefix = "Err";

#endregion

#region "�p�u���b�N���b�\�b�h"
        /// <summary>
        /// �R���X�g���N�^
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
        /// ۸ލ쐬
        /// </summary>
        /// <param name="sLogMessage">۸�ү����</param>
        /// <param name="inStackTrace">�X�^�b�N�g���[�X</param>
        /// <param name="eventType">����Ď��</param>
        public void WriteLog(string sLogMessage, string inStackTrace, System.Diagnostics.EventLogEntryType eventType)
        {
            try
            {

                //----- ���C���t�H�[����ListBox�ɕ\��
//                formMain.Form1Instance.listBoxErrText = sLogMessage;
            }
            catch (Exception ex)
            {
                // ۸�̧�ُo��
                this.WriteLogFile(ex.Message);
            }

            if (Mocs.Properties.Settings.Default.ErrLogOutMode == "ON")
            {
                // ۸�̧�ُo��(���b�Z�[�W�ƃX�^�b�N�g���[�X���o��)
                this.WriteLogFile(sLogMessage + " : " + inStackTrace);
            }
        }


        /// <summary>
        /// ۸�̧�ُ�������
        /// </summary>
        /// <param name="sLogMessage">��������ү����</param>
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

                // �t�@�C���r������ǉ�
                Monitor.Enter(syncObject); // ���b�N�擾
                try
                {
                    // �t�H���_ (�f�B���N�g��) �����݂��Ă��邩�ǂ����m�F����
                    if (System.IO.Directory.Exists(outLogDir) == false)
                    {
                        // �t�H���_ (�f�B���N�g��) ���쐬����
                        System.IO.Directory.CreateDirectory(outLogDir);
                    }

                    //�t�H���_����
                    sFileName = outLogDir + "/" + outFilePrefix + "_" + DateTime.Now.ToString("dd") + ".Log";
                    
                    // �f�[�^��������
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
                    Monitor.Exit(syncObject); // ���b�N���
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
    
