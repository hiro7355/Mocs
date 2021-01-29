using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mocs.CellMonTabNet;

namespace Mocs
{
	public class SurveyMonitor
	{
		/// <summary>
		/// Cell運転操作要求
		/// </summary>
		public class OperationNotifyCtrl
		{
			/// <summary>
			/// Cell運転操作要求制御状態
			/// </summary>
			public enum eStatus
			{
				Init = 0,               // 初期処理
				SendOperation,          // Cell運転操作要求通知
				WaitResponse,           // Cell運転操作要求応答待ち
				Complete,               // Cell運転操作要求通知完了
				Error,                  // Cell運転操作要求通知異常
			}

			/// <summary>
			/// Cell運転操作要求状態
			/// </summary>
			public eStatus notifyStatus;

			/// <summary>
			/// Cell運転操作種別
			/// </summary>
			public CellOperationType.eType opeType;

			/// <summary>
			/// Cell運転操作要求メッセージ
			/// </summary>
			public CellMonTabMsgGetOperation operationMsg = null;

			/// <summary>
			/// 応答異常
			/// </summary>
			public CellMonTabMsgError.eCmdError ResError = CellMonTabMsgError.eCmdError.None;
		}

		/// <summary>
		/// テーブル更新通知
		/// </summary>
		public class UpdateTableNotifyCtrl
		{
			/// <summary>
			/// データベース更新要求制御状態
			/// </summary>
			public enum eStatus
			{
				Init = 0,               // 初期処理
				SendUpdateTable,        // テーブル更新通知通知
				WaitResponse,           // テーブル更新通知応答待ち
				Complete,               // テーブル更新通知通知完了
				Error,                  // テーブル更新通知通知異常
			}

			/// <summary>
			/// テーブル更新通知状態
			/// </summary>
			public eStatus notifyStatus;

			/// <summary>
			/// 更新テーブルリスト
			/// </summary>
			public List<string> tabList = new List<string>();

			/// <summary>
			/// テーブル更新通知メッセージ
			/// </summary>
			public CellMonTabMsgPutUpdateTable updateTableMsg = null;

			/// <summary>
			/// 応答異常
			/// </summary>
			public CellMonTabMsgError.eCmdError ResError = CellMonTabMsgError.eCmdError.None;
		}

		/// <summary>
		/// データベース更新要求
		/// </summary>
		public class UpdateDbNotifyCtrl
		{
			/// <summary>
			/// データベース更新要求制御状態
			/// </summary>
			public enum eStatus
			{
				Init = 0,               // 初期処理
				SendUpdateDb,           // データベース更新要求通知
				WaitResponse,           // データベース更新要求応答待ち
				Complete,               // データベース更新要求通知完了
				Error,                  // データベース更新要求通知異常
			}

			/// <summary>
			/// データベース更新要求状態
			/// </summary>
			public eStatus notifyStatus;

			/// <summary>
			/// レイアウトデータ設定ファイルパス
			/// </summary>
			public string layDataPath = "";

			/// <summary>
			/// マスタデータ設定ファイルパス
			/// </summary>
			public string masterDataPath = "";

			/// <summary>
			/// データベース更新要求メッセージ
			/// </summary>
			public CellMonTabMsgGetUpdateDb updateDbMsg = null;

			/// <summary>
			/// 応答異常
			/// </summary>
			public CellMonTabMsgError.eCmdError ResError = CellMonTabMsgError.eCmdError.None;
		}

		/// <summary>
		/// MU挿入要求
		/// </summary>
		public class InsertPointNotifyCtrl
		{
			/// <summary>
			/// MU挿入要求制御状態
			/// </summary>
			public enum eStatus
			{
				Init = 0,               // 初期処理
				SendInsertPoint,        // 挿入ポイント通知通知
				WaitResponse,           // 挿入ポイント通知応答待ち
				Complete,               // 挿入ポイント通知通知完了
				Error,                  // 挿入ポイント通知通知異常
			}

			/// <summary>
			/// MU挿入要求状態
			/// </summary>
			public eStatus notifyStatus;

			/// <summary>
			/// MU識別子
			/// </summary>
			public UInt32 MuId;

			/// <summary>
			/// 挿入ポイントID
			/// </summary>
			public UInt32 PointId;

			/// <summary>
			/// MU挿入要求メッセージ
			/// </summary>
			public CellMonTabMsgGetInsert insertMsg = null;

			/// <summary>
			/// 応答異常
			/// </summary>
			public CellMonTabMsgError.eCmdError ResError = CellMonTabMsgError.eCmdError.None;
		}

		/// <summary>
		/// MU解除要求
		/// </summary>
		public class EjectNotifyCtrl
		{
			/// <summary>
			/// MU解放要求制御状態
			/// </summary>
			public enum eStatus
			{
				Init = 0,               // 初期処理
				SendEject,              // MU解放要求通知
				WaitResponse,           // MU解放要求応答待ち
				Complete,               // MU解放要求通知完了
				Error,                  // MU解放要求通知異常
			}

			/// <summary>
			/// MU解放要求制御状態
			/// </summary>
			public eStatus notifyStatus;

			/// <summary>
			/// MU識別子
			/// </summary>
			public UInt32 MuId;

			/// <summary>
			/// MU解除要求メッセージ
			/// </summary>
			public CellMonTabMsgGetEject ejectMsg = null;

			/// <summary>
			/// 応答異常
			/// </summary>
			public CellMonTabMsgError.eCmdError ResError = CellMonTabMsgError.eCmdError.None;
		}

		/// <summary>
		/// 監視モニタID
		/// </summary>
		private UInt32 _Id;

		/// <summary>
		/// 監視モニタ名称
		/// </summary>
		private string _Name;

		/// <summary>
		/// Cell IPアドレス
		/// </summary>
		private string _CellIpAddress;

		/// <summary>
		/// 監視モニタリモートポート番号
		/// </summary>
		private UInt16 _CellPortNo;

		/// <summary>
		/// 監視モニタIPアドレス
		/// </summary>
		private string _MonitorIpAddress;

		/// <summary>
		/// 監視モニタローカルポート番号
		/// </summary>
		private UInt16 _MonitorPortNo;

		/// <summary>
		/// Cell運転操作要求
		/// </summary>
		private OperationNotifyCtrl _operationNotifyCtrl;

		/// <summary>
		/// テーブル更新通知
		/// </summary>
		private UpdateTableNotifyCtrl _updateTableNotifyCtrl;

		/// <summary>
		/// データベース更新要求
		/// </summary>
		private UpdateDbNotifyCtrl _updateDbNotifyCtrl;

		/// <summary>
		/// MU挿入要求
		/// </summary>
		private InsertPointNotifyCtrl _insertPointNotifyCtrl;

		/// <summary>
		/// MU解除要求
		/// </summary>
		private EjectNotifyCtrl _ejectNotifyCtrl;

		/// <summary>
		/// 通信接続フラグ（メッセージ応答による判定）
		/// </summary>
		private bool IsMsgConnected;

		/// <summary>
		/// 監視モニタータブレット通信メッセージ制御
		/// </summary>
		private CellMonTabMessageCtrl _cellMonTabMsgCtrl;

		/// <summary>
		/// 監視モニタータブレット通信ローカルポートオフセット
		/// </summary>
		private int monTabConnPortOffset = 0;

		/// <summary>
		/// 定期処理実行、Cell,監視モニタータブレット通信ロックオブジェクト
		/// </summary>
		object lockObjExecAndCom = new object();

		#region プロパティー
		/// <summary>
		/// 監視モニタID
		/// </summary>
		public UInt32 Id
		{
			get { return this._Id; }
			set { this._Id = value; }
		}

		/// <summary>
		/// 監視モニタ名称
		/// </summary>
		public string Name
		{
			get { return this._Name; }
			set { this._Name = value; }
		}

		/// <summary>
		/// Cell IPアドレス
		/// </summary>
		public string CellIpAddress
		{
			get { return this._CellIpAddress; }
			set { this._CellIpAddress = value; }
		}

		/// <summary>
		/// 監視モニタリモートポート番号
		/// </summary>
		public UInt16 CellPortNo
		{
			get { return this._CellPortNo; }
			set { this._CellPortNo = value; }
		}

		/// <summary>
		/// 監視モニタIPアドレス
		/// </summary>
		public string MonitorIpAddress
		{
			get { return this._MonitorIpAddress; }
			set { this._MonitorIpAddress = value; }
		}

		/// <summary>
		/// 監視モニタローカルポート番号
		/// </summary>
		public UInt16 MonitorPortNo
		{
			get { return this._MonitorPortNo; }
			set { this._MonitorPortNo = value; }
		}

		/// <summary>
		/// 監視モニタータブレット通信メッセージ制御
		/// </summary>
		public CellMonTabMessageCtrl cellMonTabMsgCtrl
		{
			get { return this._cellMonTabMsgCtrl; }
		}

		#endregion

		/// <summary>
		/// コンストラクター
		/// </summary>
		public SurveyMonitor()
		{

		}

		/// <summary>
		/// 監視モニター制御実行
		/// </summary>
		public void execCtrlProc()
		{
			lock (lockObjExecAndCom)
			{
				// メッセージ制御、通信設定
				if (this._cellMonTabMsgCtrl == null)
				{
					this.IsMsgConnected = false;

					Console.WriteLine("tryCon:{0}", this._Id);
					int sendPortNo = 0;
					int recvPortNo = 0;

					if (Properties.Settings.Default.UseSimProcCom == true)
					{
						string[] ipElements1 = this._MonitorIpAddress.Split('.');
						sendPortNo = int.Parse(ipElements1[3]) + monTabConnPortOffset * 256 + Properties.Settings.Default.SimProcMonTabComPortNoBase;
						recvPortNo = int.Parse(ipElements1[3]) + (monTabConnPortOffset + 1) * 256 + Properties.Settings.Default.SimProcMonTabComPortNoBase;
					}
					else
					{
						sendPortNo = 0;
						recvPortNo = this._MonitorPortNo;
					}

					CellMonTabCom com = new CellMonTabCom(sendPortNo, recvPortNo);
					if (true == com.Connect(this._CellIpAddress, (int)this._CellPortNo))
					{
						this._cellMonTabMsgCtrl = new CellMonTabMessageCtrl(com);
						this._cellMonTabMsgCtrl.ownerMonTabManagerId = this._Id;
						this._cellMonTabMsgCtrl.sentMessageEvent += new CellMonTabMessageCtrl.SentMessageEvent(CellMonTabComSentMessageEventProc);
						this._cellMonTabMsgCtrl.sendMessageFailEvent += new CellMonTabMessageCtrl.SendMessageFailEvent(CellMonTabComSendMessageFailEventProc);
						this._cellMonTabMsgCtrl.recvdMessageEvent += new CellMonTabMessageCtrl.ReceivedMessageEvent(CellMonTabComReceivedMessageEventProc);
						this._cellMonTabMsgCtrl.Start();

					}
				}
				else if (this._cellMonTabMsgCtrl.IsConnected != true)
				{
					// メッセージ通信切断状態
					this._cellMonTabMsgCtrl.Stop();
					this._cellMonTabMsgCtrl.Dispose();
					this._cellMonTabMsgCtrl = null;
					this.IsMsgConnected = false;
				}
				else
				{
					// Cell運転操作要求
					if (this._operationNotifyCtrl != null)
					{
						OperationProc();
					}
					// テーブル更新通知
					if (this._updateTableNotifyCtrl != null)
					{
						UpdateTableProc();
					}
					// データベース更新要求
					if (this._updateDbNotifyCtrl != null)
					{
						UpdateDbProc();
					}
					// MU挿入要求
					if (this._insertPointNotifyCtrl != null)
					{
						InsertPointProc();
					}
					// MU解除要求
					if (this._ejectNotifyCtrl != null)
					{
						EjectProc();
					}
				}

			}
		}

		/// <summary>
		/// Cell運転操作要求
		/// </summary>
		/// <param name="opeType"></param>
		/// <returns></returns>
		public bool ReqOperation(CellOperationType.eType opeType)
		{
			bool ret = false;

			if (this._operationNotifyCtrl == null)
			{
				this._operationNotifyCtrl = new OperationNotifyCtrl();
				this._operationNotifyCtrl.opeType = opeType;

				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// Cell運転操作要求制御設定確認
		/// </summary>
		public bool IsSetOperation()
		{
			bool ret = false;

			if (this._operationNotifyCtrl != null)
			{
				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// テーブル更新通知要求
		/// </summary>
		/// <param name="tableList"></param>
		/// <returns></returns>
		public bool ReqUpdateTable(List<string> tableList)
		{
			bool ret = false;

			if (this._updateTableNotifyCtrl == null)
			{
				this._updateTableNotifyCtrl = new UpdateTableNotifyCtrl();
				this._updateTableNotifyCtrl.tabList = tableList;

				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// テーブル更新通知制御設定確認
		/// </summary>
		public bool IsSetUpdateTable()
		{
			bool ret = false;

			if (this._updateTableNotifyCtrl != null)
			{
				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// データベース更新要求
		/// </summary>
		/// <param name="layDataPath"></param>
		/// <param name="masterDataPath"></param>
		/// <returns></returns>
		public bool ReqUpdateDb(string layDataPath, string masterDataPath)
		{
			bool ret = false;

			if (this._updateDbNotifyCtrl == null)
			{
				this._updateDbNotifyCtrl = new UpdateDbNotifyCtrl();
				this._updateDbNotifyCtrl.layDataPath = layDataPath;
				this._updateDbNotifyCtrl.masterDataPath = masterDataPath;

				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// データベース更新要求制御設定確認
		/// </summary>
		public bool IsSetUpdateDb()
		{
			bool ret = false;

			if (this._updateDbNotifyCtrl != null)
			{
				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// MU挿入要求
		/// </summary>
		/// <param name="muId"></param>
		/// <param name="pointId"></param>
		/// <returns></returns>
		public bool ReqInsertPoint(UInt32 muId, UInt32 pointId)
		{
			bool ret = false;

			if (this._insertPointNotifyCtrl == null)
			{
				this._insertPointNotifyCtrl = new InsertPointNotifyCtrl();
				this._insertPointNotifyCtrl.MuId = muId;
				this._insertPointNotifyCtrl.PointId = pointId;

				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// MU挿入要求制御設定確認
		/// </summary>
		public bool IsSetInsertPoint()
		{
			bool ret = false;

			if (this._insertPointNotifyCtrl != null)
			{
				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// MU解除要求
		/// </summary>
		public bool ReqEject(UInt32 muId)
		{
			bool ret = false;

			if (this._ejectNotifyCtrl == null)
			{
				this._ejectNotifyCtrl = new EjectNotifyCtrl();
				this._ejectNotifyCtrl.MuId = muId;

				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// MU解除要求制御設定確認
		/// </summary>
		public bool IsSetEject()
		{
			bool ret = false;

			if (this._ejectNotifyCtrl != null)
			{
				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// Cell運転操作要求処理
		/// </summary>
		/// <param name="opeType"></param>
		/// <returns></returns>
		public void OperationProc()
		{
			switch (this._operationNotifyCtrl.notifyStatus)
			{
				case OperationNotifyCtrl.eStatus.Init:
					// 初期処理
					this._operationNotifyCtrl.notifyStatus = OperationNotifyCtrl.eStatus.SendOperation;
					break;
				case OperationNotifyCtrl.eStatus.SendOperation:
					// Cell運転操作要求
					this._operationNotifyCtrl.operationMsg = new CellMonTabMsgGetOperation();
					this._operationNotifyCtrl.operationMsg.opeType = this._operationNotifyCtrl.opeType;

					Console.WriteLine("Cell運転操作要求送信");
					// Cell運転操作要求メッセージ送信
					if (true == this._cellMonTabMsgCtrl.SendMessage(this._operationNotifyCtrl.operationMsg))
					{
						this._operationNotifyCtrl.notifyStatus = OperationNotifyCtrl.eStatus.WaitResponse;
					}
					break;
				case OperationNotifyCtrl.eStatus.WaitResponse:
					// Cell運転操作要求応答待ち
					if (this._operationNotifyCtrl.operationMsg.SendStatus == CellMonTabMessage.eSendStatus.SendNg)
					{
						// 送信失敗
						this._operationNotifyCtrl.operationMsg = null;
						this._operationNotifyCtrl.notifyStatus = OperationNotifyCtrl.eStatus.Error;

					}
					else if (this._operationNotifyCtrl.ResError != CellMonTabMsgError.eCmdError.None)
					{
						// 異常応答
						this._operationNotifyCtrl.operationMsg = null;
						this._operationNotifyCtrl.notifyStatus = OperationNotifyCtrl.eStatus.Error;

					}
					else if (this._operationNotifyCtrl.operationMsg.SendStatus == CellMonTabMessage.eSendStatus.RecvRes)
					{
						// 応答受信
						this._operationNotifyCtrl.operationMsg = null;
						this._operationNotifyCtrl.notifyStatus = OperationNotifyCtrl.eStatus.Complete;


					}
					break;
				case OperationNotifyCtrl.eStatus.Complete:

					if (this._operationNotifyCtrl.opeType == CellOperationType.eType.Start)
                    {
						//  CELLとの接続が確立しました　と表示されるようにステータスを設定
						Mocs.Utils.CommonUtil.SetLastSocketConnectionStatus(1);
                    } 
					else
                    {
						//  ソケット通信エラー（正常）を設定
						Mocs.Utils.CommonUtil.SetLastSocketError(0);
					}

					// Cell運転操作要求完了
					this._operationNotifyCtrl = null;
					break;
				case OperationNotifyCtrl.eStatus.Error:

					if (this._operationNotifyCtrl.opeType == CellOperationType.eType.Start)
					{
						//  CELLとの接続ができません。　と表示されるようにステータスを設定
						Mocs.Utils.CommonUtil.SetLastSocketConnectionStatus(-1);
					}
					else
					{
						//  ソケット通信エラーを設定
						Mocs.Utils.CommonUtil.SetLastSocketError((int)this._operationNotifyCtrl.notifyStatus);
					}

					// Cell運転操作要求異常
					this._operationNotifyCtrl = null;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// テーブル更新通知処理
		/// </summary>
		/// <param name="opeType"></param>
		/// <returns></returns>
		public void UpdateTableProc()
		{
			switch (this._updateTableNotifyCtrl.notifyStatus)
			{
				case UpdateTableNotifyCtrl.eStatus.Init:
					// 初期処理
					this._updateTableNotifyCtrl.notifyStatus = UpdateTableNotifyCtrl.eStatus.SendUpdateTable;
					break;
				case UpdateTableNotifyCtrl.eStatus.SendUpdateTable:
					// テーブル更新通知
					this._updateTableNotifyCtrl.updateTableMsg = new CellMonTabMsgPutUpdateTable();
					this._updateTableNotifyCtrl.updateTableMsg.tabList = this._updateTableNotifyCtrl.tabList;

					Console.WriteLine("テーブル更新通知送信");
					// テーブル更新通知メッセージ送信
					if (true == this._cellMonTabMsgCtrl.SendMessage(this._updateTableNotifyCtrl.updateTableMsg))
					{
						this._updateTableNotifyCtrl.notifyStatus = UpdateTableNotifyCtrl.eStatus.WaitResponse;
					}
					break;
				case UpdateTableNotifyCtrl.eStatus.WaitResponse:
					// テーブル更新通知応答待ち
					if (this._updateTableNotifyCtrl.updateTableMsg.SendStatus == CellMonTabMessage.eSendStatus.SendNg)
					{
						// 送信失敗
						this._updateTableNotifyCtrl.updateTableMsg = null;
						this._updateTableNotifyCtrl.notifyStatus = UpdateTableNotifyCtrl.eStatus.Error;
					}
					else if (this._updateTableNotifyCtrl.ResError != CellMonTabMsgError.eCmdError.None)
					{
						// 異常応答
						this._updateTableNotifyCtrl.updateTableMsg = null;
						this._updateTableNotifyCtrl.notifyStatus = UpdateTableNotifyCtrl.eStatus.Error;
					}
					else if (this._updateTableNotifyCtrl.updateTableMsg.SendStatus == CellMonTabMessage.eSendStatus.RecvRes)
					{
						// 応答受信
						this._updateTableNotifyCtrl.updateTableMsg = null;
						this._updateTableNotifyCtrl.notifyStatus = UpdateTableNotifyCtrl.eStatus.Complete;
					}
					break;
				case UpdateTableNotifyCtrl.eStatus.Complete:
					// テーブル更新通知完了
					this._updateTableNotifyCtrl = null;
					break;
				case UpdateTableNotifyCtrl.eStatus.Error:
					// テーブル更新通知異常
					this._updateTableNotifyCtrl = null;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// データベース更新要求処理
		/// </summary>
		public void UpdateDbProc()
		{
			switch (this._updateDbNotifyCtrl.notifyStatus)
			{
				case UpdateDbNotifyCtrl.eStatus.Init:
					// 初期処理
					this._updateDbNotifyCtrl.notifyStatus = UpdateDbNotifyCtrl.eStatus.SendUpdateDb;
					break;
				case UpdateDbNotifyCtrl.eStatus.SendUpdateDb:
					// データベース更新要求
					this._updateDbNotifyCtrl.updateDbMsg = new CellMonTabMsgGetUpdateDb();
					this._updateDbNotifyCtrl.updateDbMsg.layDataPath = this._updateDbNotifyCtrl.layDataPath;
					this._updateDbNotifyCtrl.updateDbMsg.masterDataPath = this._updateDbNotifyCtrl.masterDataPath;

					Console.WriteLine("データベース更新要求送信");
					// データベース更新要求メッセージ送信
					if (true == this._cellMonTabMsgCtrl.SendMessage(this._updateDbNotifyCtrl.updateDbMsg))
					{
						this._updateDbNotifyCtrl.notifyStatus = UpdateDbNotifyCtrl.eStatus.WaitResponse;
					}
					break;
				case UpdateDbNotifyCtrl.eStatus.WaitResponse:
					// データベース更新要求答待ち
					if (this._updateDbNotifyCtrl.updateDbMsg.SendStatus == CellMonTabMessage.eSendStatus.SendNg)
					{
						// 送信失敗
						this._updateDbNotifyCtrl.updateDbMsg = null;
						this._updateDbNotifyCtrl.notifyStatus = UpdateDbNotifyCtrl.eStatus.Error;
					}
					else if (this._updateDbNotifyCtrl.ResError != CellMonTabMsgError.eCmdError.None)
					{
						// 異常応答
						this._updateDbNotifyCtrl.updateDbMsg = null;
						this._updateDbNotifyCtrl.notifyStatus = UpdateDbNotifyCtrl.eStatus.Error;
					}
					else if (this._updateDbNotifyCtrl.updateDbMsg.SendStatus == CellMonTabMessage.eSendStatus.RecvRes)
					{
						// 応答受信
						this._updateDbNotifyCtrl.updateDbMsg = null;
						this._updateDbNotifyCtrl.notifyStatus = UpdateDbNotifyCtrl.eStatus.Complete;
					}
					break;
				case UpdateDbNotifyCtrl.eStatus.Complete:
					// データベース更新要求完了
					this._updateDbNotifyCtrl = null;
					break;
				case UpdateDbNotifyCtrl.eStatus.Error:
					// データベース更新要求異常
					this._updateDbNotifyCtrl = null;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// MU挿入要求処理
		/// </summary>
		public void InsertPointProc()
		{
			switch (this._insertPointNotifyCtrl.notifyStatus)
			{
				case InsertPointNotifyCtrl.eStatus.Init:
					// 初期処理
					this._insertPointNotifyCtrl.notifyStatus = InsertPointNotifyCtrl.eStatus.SendInsertPoint;
					break;
				case InsertPointNotifyCtrl.eStatus.SendInsertPoint:
					// MU挿入要求
					this._insertPointNotifyCtrl.insertMsg = new CellMonTabMsgGetInsert();
					this._insertPointNotifyCtrl.insertMsg.MuId = this._insertPointNotifyCtrl.MuId;
					this._insertPointNotifyCtrl.insertMsg.PointId = this._insertPointNotifyCtrl.PointId;

					Console.WriteLine("MU挿入要求送信");
					// MU挿入要求メッセージ送信
					if (true == this._cellMonTabMsgCtrl.SendMessage(this._insertPointNotifyCtrl.insertMsg))
					{
						this._insertPointNotifyCtrl.notifyStatus = InsertPointNotifyCtrl.eStatus.WaitResponse;
					}
					break;
				case InsertPointNotifyCtrl.eStatus.WaitResponse:
					// MU挿入要求応答待ち
					if (this._insertPointNotifyCtrl.insertMsg.SendStatus == CellMonTabMessage.eSendStatus.SendNg)
					{
						// 送信失敗
						this._insertPointNotifyCtrl.insertMsg = null;
						this._insertPointNotifyCtrl.notifyStatus = InsertPointNotifyCtrl.eStatus.Error;
					}
					else if (this._insertPointNotifyCtrl.ResError != CellMonTabMsgError.eCmdError.None)
					{
						// 異常応答
						this._insertPointNotifyCtrl.insertMsg = null;
						this._insertPointNotifyCtrl.notifyStatus = InsertPointNotifyCtrl.eStatus.Error;
					}
					else if (this._insertPointNotifyCtrl.insertMsg.SendStatus == CellMonTabMessage.eSendStatus.RecvRes)
					{
						// 応答受信
						this._insertPointNotifyCtrl.insertMsg = null;
						this._insertPointNotifyCtrl.notifyStatus = InsertPointNotifyCtrl.eStatus.Complete;
					}
					break;
				case InsertPointNotifyCtrl.eStatus.Complete:
					// MU挿入要求完了
					this._insertPointNotifyCtrl = null;
					break;
				case InsertPointNotifyCtrl.eStatus.Error:
					// MU挿入要求異常
					this._insertPointNotifyCtrl = null;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// MU解除要求処理
		/// </summary>
		public void EjectProc()
		{
			switch (this._ejectNotifyCtrl.notifyStatus)
			{
				case EjectNotifyCtrl.eStatus.Init:
					// 初期処理
					this._ejectNotifyCtrl.notifyStatus = EjectNotifyCtrl.eStatus.SendEject;
					break;
				case EjectNotifyCtrl.eStatus.SendEject:
					// MU解除要求
					this._ejectNotifyCtrl.ejectMsg = new CellMonTabMsgGetEject();
					this._ejectNotifyCtrl.ejectMsg.MuId = this._ejectNotifyCtrl.MuId;

					Console.WriteLine("MU解除要求送信");
					// MU解除要求メッセージ送信
					if (true == this._cellMonTabMsgCtrl.SendMessage(this._ejectNotifyCtrl.ejectMsg))
					{
						this._ejectNotifyCtrl.notifyStatus = EjectNotifyCtrl.eStatus.WaitResponse;
					}
					break;
				case EjectNotifyCtrl.eStatus.WaitResponse:
					// MU解除要求応答待ち
					if (this._ejectNotifyCtrl.ejectMsg.SendStatus == CellMonTabMessage.eSendStatus.SendNg)
					{
						// 送信失敗
						this._ejectNotifyCtrl.ejectMsg = null;
						this._ejectNotifyCtrl.notifyStatus = EjectNotifyCtrl.eStatus.Error;
					}
					else if (this._ejectNotifyCtrl.ResError != CellMonTabMsgError.eCmdError.None)
					{
						// 異常応答
						this._ejectNotifyCtrl.ejectMsg = null;
						this._ejectNotifyCtrl.notifyStatus = EjectNotifyCtrl.eStatus.Error;
					}
					else if (this._ejectNotifyCtrl.ejectMsg.SendStatus == CellMonTabMessage.eSendStatus.RecvRes)
					{
						// 応答受信
						this._ejectNotifyCtrl.ejectMsg = null;
						this._ejectNotifyCtrl.notifyStatus = EjectNotifyCtrl.eStatus.Complete;
					}
					break;
				case EjectNotifyCtrl.eStatus.Complete:
					// MU解除要求完了
					this._ejectNotifyCtrl = null;
					break;
				case EjectNotifyCtrl.eStatus.Error:
					// MU解除要求異常
					this._ejectNotifyCtrl = null;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 監視モニタータブレット通信メッセージ送信完了イベント処理
		/// </summary>
		/// <param name="sntMsg">送信メッセージ</param>
		/// <param name="resMsg">受信応答メッセージ</param>
		private void CellMonTabComSentMessageEventProc(CellMonTabMessage sntMsg, CellMonTabMessage resMsg)
		{
			lock (lockObjExecAndCom)
			{
				// Cell運転操作要求
				if (resMsg.GetType() == typeof(CellMonTabMsgGetOperationResponse))
				{
					Console.WriteLine("Cell運転操作要求応答受信");
				}
				// テーブル更新通知
				else if (resMsg.GetType() == typeof(CellMonTabMsgPutUpdateTableResponse))
				{
					Console.WriteLine("テーブル更新通知応答受信");
				}
				// データベース更新要求
				else if (resMsg.GetType() == typeof(CellMonTabMsgGetUpdateDbResponse))
				{
					Console.WriteLine("データベース更新要求応答受信");
				}
				// MU挿入要求
				else if (resMsg.GetType() == typeof(CellMonTabMsgGetInsertResponse))
				{
					Console.WriteLine("MU挿入要求応答受信");
				}
				// MU解除要求
				else if (resMsg.GetType() == typeof(CellMonTabMsgGetEjectResponse))
				{
					Console.WriteLine("MU解除要求応答受信");
				}

				this.IsMsgConnected = true;
			}
		}

		/// <summary>
		/// 監視モニタータブレット通信メッセージ送信失敗イベント処理
		/// </summary>
		/// <param name="message">送信失敗メッセージ</param>
		private void CellMonTabComSendMessageFailEventProc(CellMonTabMessage message)
		{
			lock (lockObjExecAndCom)
			{
				this.IsMsgConnected = false;
			}
		}

		/// <summary>
		/// Cell,Mu通信メッセージ受信イベント処理
		/// </summary>
		/// <param name="message">受信メッセージ</param>
		private void CellMonTabComReceivedMessageEventProc(CellMonTabMessage message)
		{
			lock (lockObjExecAndCom)
			{
				CellMonTabMessage recvdMsg;

				// 受信メッセージ読出し
				if (this._cellMonTabMsgCtrl.RecvMessage(out recvdMsg))
				{
					// データベース更新結果通知メッセージ
					if (recvdMsg.GetType() == typeof(CellMonTabMsgPutUpdateDbResult))
					{
						Console.WriteLine("データベース更新結果通知受信");
						RecvMsgPutUpdateDbResult((CellMonTabMsgPutUpdateDbResult)recvdMsg);
					}

					this.IsMsgConnected = true;
				}
			}
		}

		/// <summary>
		/// データベース更新結果設定
		/// </summary>
		/// <param name="recvdMsg"></param>
		private void RecvMsgPutUpdateDbResult(CellMonTabMsgPutUpdateDbResult recvdMsg)
		{
			CellMonTabMsgPutUpdateDbResultResponse res = new CellMonTabMsgPutUpdateDbResultResponse();

			Console.WriteLine("データベース更新結果:{0}", recvdMsg.result);

			Console.WriteLine("データベース更新結果通知応答送信");
			// 応答のシーケンス番号に受信メッセージのシーケンス番号を返す
			res.SequenceNo = recvdMsg.SequenceNo;
			if (true == this._cellMonTabMsgCtrl.SendMessageRes(res))
			{

			}
		}
	}
}
