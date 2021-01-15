using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Concurrent;

using Mocs.MoCsSystem;
using Mocs.Log;

namespace Mocs.CellMonTabNet
{
	public class CellMonTabMessageCtrl : IDisposable
	{
		#region 固定値

		/// <summary>
		/// 送信メッセージ保存バッファサイズ
		/// </summary>
		private const int c_SendMessageBufSize = 10;

		/// <summary>
		/// 受信メッセージ保存バッファサイズ
		/// </summary>
		private const int c_RecvMessageBufSize = 10;

		/// <summary>
		/// 送信メッセージ応答保存バッファサイズ
		/// </summary>
		private const int c_SendMessageResBufSize = 10;

#if _SIM_PROC_COM
		/// <summary>
		/// メッセージ再送間隔
		/// </summary>
		private const int c_SendMessageRetryInterval = 20000;

		/// <summary>
		/// メッセージ再送回数
		/// </summary>
		private const int c_SendMessageRetryNum = 1;
#else
		/// <summary>
		/// メッセージ再送間隔
		/// </summary>
		private const int c_SendMessageRetryInterval = 5000;

		/// <summary>
		/// メッセージ再送回数
		/// </summary>
		private const int c_SendMessageRetryNum = 3;
#endif

		#endregion


		#region 型定義

		/// <summary>
		/// メッセージ送信完了イベント型定義
		/// </summary>
		public delegate void SentMessageEvent(CellMonTabMessage sntMsg, CellMonTabMessage resMsg);

		/// <summary>
		/// メッセージ送信失敗イベント型定義
		/// </summary>
		public delegate void SendMessageFailEvent(CellMonTabMessage message);

		/// <summary>
		/// メッセージ受信イベント型定義
		/// </summary>
		public delegate void ReceivedMessageEvent(CellMonTabMessage message);


		// 送信メッセージイベント定義
		private enum eEventSendProc
		{
			ReqSendRes,
			ReqSend,
			ReqSendRetry,
			SendFail,
			Close,
		}

		#endregion

		#region メンバ変数

		/// <summary>
		/// 送受信処理ロックオブジェクト
		/// </summary>
		private object objSendRecvLock = new object();

		/// <summary>
		/// クローズ処理ロックオブジェクト
		/// </summary>
		private object objClosingLock = new object();

		/// <summary>
		/// 監視モニタ、タブレット通信
		/// </summary>
		private CellMonTabCom comCl;

		/// <summary>
		/// 所有者監視モニタ、タブレット管理のID
		/// </summary>
		private UInt32 _ownerMonTabManagerId;

		/// <summary>
		/// 現在送信中メッセージ
		/// </summary>
		private CellMonTabMessage curSendMsg;

		/// <summary>
		/// 現在受信メッセージ
		/// </summary>
		private CellMonTabMessage curRecvMsg;

		/// <summary>
		/// メッセージ送信ループ継続
		/// </summary>
		private bool bSendMsgContinue = false;

		/// <summary>
		/// メッセージ送信ループ
		/// </summary>
		private Thread threadSendMsgLoop;

		/// <summary>
		/// メッセージ受信ループ継続
		/// </summary>
		private bool bRecvMsgContinue = false;

		/// <summary>
		/// メッセージ受信ループ
		/// </summary>
		private Thread threadRecvMsgLoop;

		/// <summary>
		/// メッセージ送信完了イベント定義
		/// </summary>
		public event SentMessageEvent sentMessageEvent;

		/// <summary>
		/// メッセージ送信失敗イベント型定義
		/// </summary>
		public event SendMessageFailEvent sendMessageFailEvent;

		/// <summary>
		/// メッセージ受信イベント定義
		/// </summary>
		public event ReceivedMessageEvent recvdMessageEvent;

		/// <summary>
		/// 送信伝文バッファ
		/// </summary>
		private BlockingCollection<CellMonTabMessage> sendMessageBuf;

		/// <summary>
		/// 受信伝文バッファ
		/// </summary>
		private BlockingCollection<CellMonTabMessage> recvMessageBuf;

		/// <summary>
		/// 応答送信バッファ作成
		/// </summary>
		private BlockingCollection<CellMonTabMessage> sendMessageResBuf;

		/// <summary>
		/// メッセージ再送タイマー
		/// </summary>
		private Timer timerSendRetry;

		/// <summary>
		/// メッセージ送信イベント
		/// </summary>
		private EventWaitHandle[] eventSendProc;

		/// <summary>
		/// メッセージ送信イベント待ち
		/// </summary>
		private WaitHandle[] waitHndlSendProc;

		/// <summary>
		/// メッセージ再送回数
		/// </summary>
		private UInt16 SendRetryNum;

		#endregion

		#region プロパティ

		/// <summary>
		/// 所有者監視モニタ、タブレット管理のID
		/// </summary>
		public UInt32 ownerMonTabManagerId
		{
			get { return this._ownerMonTabManagerId; }
			set { this._ownerMonTabManagerId = value; }
		}

		/// <summary>
		/// メッセージ通信接続確認
		/// </summary>
		public bool IsConnected
		{
			get
			{
				bool ret = false;

				if (this.comCl != null)
				{
					ret = this.comCl.IsConnected;
				}
				return ret;
			}
		}

		#endregion

		#region メソッド

		/// <summary>
		/// コンストラクター
		/// </summary>
		public CellMonTabMessageCtrl()
		{

		}

		/// <summary>
		/// コンストラクター
		/// </summary>
		/// <param name="com">監視モニタ、タブレット通信</param>
		public CellMonTabMessageCtrl(CellMonTabCom com)
		{
			this.comCl = com;

			// メッセージ再送回数初期化
			this.SendRetryNum = 0;

			// メッセージ送信イベント作成
			this.eventSendProc = new EventWaitHandle[5];
			this.eventSendProc[(int)eEventSendProc.ReqSendRes] = new ManualResetEvent(false);
			this.eventSendProc[(int)eEventSendProc.ReqSend] = new ManualResetEvent(false);
			this.eventSendProc[(int)eEventSendProc.ReqSendRetry] = new ManualResetEvent(false);
			this.eventSendProc[(int)eEventSendProc.SendFail] = new ManualResetEvent(false);
			this.eventSendProc[(int)eEventSendProc.Close] = new AutoResetEvent(false);
			this.waitHndlSendProc = new WaitHandle[5];
			this.waitHndlSendProc[(int)eEventSendProc.ReqSendRes] = this.eventSendProc[(int)eEventSendProc.ReqSendRes];
			this.waitHndlSendProc[(int)eEventSendProc.ReqSend] = this.eventSendProc[(int)eEventSendProc.ReqSend];
			this.waitHndlSendProc[(int)eEventSendProc.ReqSendRetry] = this.eventSendProc[(int)eEventSendProc.ReqSendRetry];
			this.waitHndlSendProc[(int)eEventSendProc.SendFail] = this.eventSendProc[(int)eEventSendProc.SendFail];
			this.waitHndlSendProc[(int)eEventSendProc.Close] = this.eventSendProc[(int)eEventSendProc.Close];

			this.eventSendProc[(int)eEventSendProc.ReqSendRes].Reset();
			this.eventSendProc[(int)eEventSendProc.ReqSend].Reset();
			this.eventSendProc[(int)eEventSendProc.ReqSendRetry].Reset();
			this.eventSendProc[(int)eEventSendProc.SendFail].Reset();
			this.eventSendProc[(int)eEventSendProc.Close].Reset();


			// メッセージ送信バッファ作成
			this.sendMessageBuf = new BlockingCollection<CellMonTabMessage>(c_SendMessageBufSize);
			// メッセージ受信バッファ作成
			this.recvMessageBuf = new BlockingCollection<CellMonTabMessage>(c_RecvMessageBufSize);
			// メッセージ応答送信バッファ作成
			this.sendMessageResBuf = new BlockingCollection<CellMonTabMessage>(c_SendMessageResBufSize);

			// メッセージ再送タイマー作成
			TimerCallback timerDelegate = new TimerCallback(SendRetryTimerCallback);
			this.timerSendRetry = new Timer(timerDelegate
											, null
											, Timeout.Infinite
											, Timeout.Infinite);

		}

		/// <summary>
		/// 終了処理
		/// </summary>
		public void Dispose()
		{
			// 通信切断
			if (this.comCl != null)
			{
				this.comCl.Disconnect();
			}

			// メッセージ送信イベントクローズ
			this.eventSendProc[(int)eEventSendProc.ReqSendRes].Close();
			this.eventSendProc[(int)eEventSendProc.ReqSend].Close();
			this.eventSendProc[(int)eEventSendProc.ReqSendRetry].Close();
			this.eventSendProc[(int)eEventSendProc.SendFail].Close();
			this.eventSendProc[(int)eEventSendProc.Close].Close();
		}

		/// <summary>
		/// 監視モニタ、タブレット通信制御開始
		/// </summary>
		public void Start()
		{
			// 送信スレッド作成、開始
			this.bSendMsgContinue = true;
			threadSendMsgLoop = new Thread(threadFuncSendMsgLoop);
			threadSendMsgLoop.IsBackground = true;
			threadSendMsgLoop.Start();

			// 受信スレッド作成、開始
			this.bRecvMsgContinue = true;
			threadRecvMsgLoop = new Thread(threadFuncRecvMsgLoop);
			threadRecvMsgLoop.IsBackground = true;
			threadRecvMsgLoop.Start();
		}

		/// <summary>
		/// 監視モニタ、タブレット通信制御停止
		/// </summary>
		public void Stop()
		{

			// 再送タイマー停止
			StopSendRetryTimer();

			// メッセージ送信イベントリセット
			if (this.eventSendProc[(int)eEventSendProc.ReqSendRes] != null)
			{
				this.eventSendProc[(int)eEventSendProc.ReqSendRes].Reset();
			}
			if (this.eventSendProc[(int)eEventSendProc.ReqSend] != null)
			{
				this.eventSendProc[(int)eEventSendProc.ReqSend].Reset();
			}
			if (this.eventSendProc[(int)eEventSendProc.ReqSendRetry] != null)
			{
				this.eventSendProc[(int)eEventSendProc.ReqSendRetry].Reset();
			}
			if (this.eventSendProc[(int)eEventSendProc.SendFail] != null)
			{
				this.eventSendProc[(int)eEventSendProc.SendFail].Reset();
			}

			// 通信切断
			if (this.comCl != null)
			{
				this.comCl.Disconnect();
			}

			// 送信スレッド停止
			bSendMsgContinue = false;
			if (this.eventSendProc[(int)eEventSendProc.Close] != null)
			{
				this.eventSendProc[(int)eEventSendProc.Close].Set();
			}
			if (threadSendMsgLoop != null)
			{
				threadSendMsgLoop.Join();
			}

			// 受信スレッド停止
			bRecvMsgContinue = false;
			if (threadRecvMsgLoop != null)
			{
				threadRecvMsgLoop.Join();
			}
		}

		/// <summary>
		/// メッセージ送信ループ
		/// </summary>
		private void threadFuncSendMsgLoop()
		{
			int ret = -1;

			while (bSendMsgContinue)
			{
				try
				{
					// 送信処理

					// イベントメッセージ処理
					ret = WaitHandle.WaitAny(this.waitHndlSendProc, -1, false);
					lock (objSendRecvLock)
					{
						if (ret == EventWaitHandle.WaitTimeout)
						{
						}
						if (true == this.eventSendProc[(int)eEventSendProc.ReqSendRes].WaitOne(0))
						{
							this.eventSendProc[(int)eEventSendProc.ReqSendRes].Reset();
							// メッセージ応答送信
							CellMonTabMessage sendMsgRes;
							// 送信メッセージバッファ読出し
							if (true == this.sendMessageResBuf.TryTake(out sendMsgRes))
							{
								// メッセージ作成
								sendMsgRes.MakePacket();
								// メッセージ送信
								this.comCl.SendMessageRes(sendMsgRes.MessageData, sendMsgRes.SequenceNo);
								// メッセージ送信状態更新(送信済み)
								sendMsgRes.SendStatus = CellMonTabMessage.eSendStatus.Sent;
							}
							// 応答送信メッセージバッファに
							// メッセージがあれば送信する
							if (this.sendMessageResBuf.Count > 0)
							{
								this.eventSendProc[(int)eEventSendProc.ReqSendRes].Set();
							}
						}
						if (true == this.eventSendProc[(int)eEventSendProc.ReqSend].WaitOne(0))
						{
							this.eventSendProc[(int)eEventSendProc.ReqSend].Reset();
							// メッセージ送信
							if (this.curSendMsg == null)
							{
								CellMonTabMessage sendMsg;
								// 送信メッセージバッファ読出し
								if (true == this.sendMessageBuf.TryTake(out sendMsg))
								{
									this.curSendMsg = sendMsg;
									// メッセージ作成
									this.curSendMsg.MakePacket();
									// メッセージ送信
									this.comCl.SendMessage(this.curSendMsg.MessageData);
									// メッセージ送信状態更新(送信済み)
									this.curSendMsg.SendStatus = CellMonTabMessage.eSendStatus.Sent;

									// 再送タイマー開始
									StartSendRetryTimer();
								}
							}
						}
						if (true == this.eventSendProc[(int)eEventSendProc.ReqSendRetry].WaitOne(0))
						{
							this.eventSendProc[(int)eEventSendProc.ReqSendRetry].Reset();
							// メッセージ再送
							if (this.curSendMsg != null)
							{
								this.comCl.SendMessage(this.curSendMsg.MessageData);
								// 再送タイマー開始
								StartSendRetryTimer();
							}
						}
						if (true == this.eventSendProc[(int)eEventSendProc.SendFail].WaitOne(0))
						{
							this.eventSendProc[(int)eEventSendProc.SendFail].Reset();
							// メッセージ送信状態更新(送信失敗)
							this.curSendMsg.SendStatus = CellMonTabMessage.eSendStatus.SendNg;

							// 送信失敗コールバック
							NotifySendMessageFialEvent(this.curSendMsg);

							// メッセージ送信失敗
							this.curSendMsg = null;
							this.SendRetryNum = 0;

							// 再送タイマー停止
							StopSendRetryTimer();

							// 次の送信メッセージがあれば送信イベント設定
							if (this.sendMessageBuf.Count != 0)
							{
								this.eventSendProc[(int)eEventSendProc.ReqSend].Set();
							}

							// 送信メッセージのシーケンス番号を更新する
							comCl.SendSeqNo++;
							Console.WriteLine("？？？？？監視モニタ、タブレット送信失敗？？？？？");
						}
					}
				}
				catch (Exception ex)
				{
					StopSendRetryTimer();
					bSendMsgContinue = false;
				}
			}
			this.comCl.Disconnect();
			MoCsLog.WriteLog(String.Format(@"監視モニタ、タブレット送信処理終了"));
		}

		/// <summary>
		/// メッセージ再送タイマー開始
		/// </summary>
		private void StartSendRetryTimer()
		{
			this.timerSendRetry.Change(c_SendMessageRetryInterval, Timeout.Infinite);
		}

		/// <summary>
		/// メッセージ再送タイマー停止
		/// </summary>
		private void StopSendRetryTimer()
		{
			this.timerSendRetry.Change(Timeout.Infinite, Timeout.Infinite);
		}

		/// <summary>
		/// メッセージ再送タイマーコールバック
		/// </summary>
		/// <param name="obj"></param>
		private void SendRetryTimerCallback(object obj)
		{
			// 再送要求イベント設定
			if (this.SendRetryNum < c_SendMessageRetryNum)
			{
				this.eventSendProc[(int)eEventSendProc.ReqSendRetry].Set();
				this.SendRetryNum++;
			}
			else
			{
				// メッセージ送信失敗イベント設定
				this.eventSendProc[(int)eEventSendProc.SendFail].Set();
			}
		}

		/// <summary>
		/// メッセージ受信ループ
		/// </summary>
		private void threadFuncRecvMsgLoop()
		{
			while (bRecvMsgContinue)
			{
				try
				{
					// 受信処理
					// パケット受信待ち(ブロッキング)
					string message;
					if (true == comCl.RecvMessage(out message))
					{
						string[] parseMsg = message.Split(CellMonTabMessage.Delim);
						if (parseMsg[0] == CellMonTabMessage.CmdGet
							|| parseMsg[0] == CellMonTabMessage.CmdPut)
						{
							// 要求メッセージ受信
							if (GetMessageMonTabSim(parseMsg[0], parseMsg[1], out this.curRecvMsg))
							{
								// メッセージクラスに受信メッセージ設定
								this.curRecvMsg.MessageData = message;
								// 受信メッセージ展開
								this.curRecvMsg.ParsePacket();
								// 受信シーケンス番号をメッセージ内に保存
								this.curRecvMsg.SequenceNo = comCl.RecvSeqNo;

								// 受信メッセージバッファ書込み
								if (true != this.recvMessageBuf.TryAdd(this.curRecvMsg))
								{

								}
								else
								{
									// コマンド受信コールバック
									NotifyReceivedMessageEvent(this.curRecvMsg);
								}
								// 受信メッセージ削除
								this.curRecvMsg = null;
							}
						}
						else if (parseMsg[0] == CellMonTabMessage.ResOk
								|| parseMsg[0] == CellMonTabMessage.ResNg)
						{
							// 送信メッセージセージのシーケンス番号化チェックする
							if (comCl.SendSeqNo == comCl.RecvSeqNo)
							{
								lock (objSendRecvLock)
								{
									// 正常応答受信
									if (this.curSendMsg != null)
									{
										if (this.curSendMsg.SubCmdName == parseMsg[1])
										{
											if (GetResMessageMonTabSim(this.curSendMsg, out this.curRecvMsg))
											{
												// メッセージ応答クラスに受信メッセージ応答設定
												this.curRecvMsg.MessageData = message;
												// 受信メッセージ応答展開
												this.curRecvMsg.ParsePacket();

												// メッセージ送信状態更新(応答受信)
												this.curSendMsg.SendStatus = CellMonTabMessage.eSendStatus.RecvRes;

												// 応答受信コールバック
												this.NotifySentMessageEvent(this.curSendMsg, this.curRecvMsg);

												// 送信メッセージ削除
												this.curSendMsg = null;
												this.curRecvMsg = null;

												// 再送タイマー停止
												StopSendRetryTimer();
												// 再送回数リセット
												this.SendRetryNum = 0;

												// 次の送信メッセージがあれば送信イベント設定
												if (this.sendMessageBuf.Count != 0)
												{
													this.eventSendProc[(int)eEventSendProc.ReqSend].Set();
												}

												// 送信メッセージのシーケンス番号を更新する
												comCl.SendSeqNo++;
											}
										}
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					bRecvMsgContinue = false;
				}
			}
			this.comCl.Disconnect();
			MoCsLog.WriteLog(String.Format(@"監視モニタ、タブレット受信処理終了"));
		}

		/// <summary>
		/// コマンド、サブコマンドからメッセージを取得
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="subCmd"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		private bool GetMessage(string cmd, string subCmd, out CellMonTabMessage msg)
		{
			bool ret = false;
			msg = null;

			if (cmd == CellMonTabMessage.CmdGet)
			{
				// GETコマンド
				switch (subCmd)
				{
					// Cell運転操作要求メッセージ
					case CellMonTabMsgGetOperation.subCmdName:
						msg = new CellMonTabMsgGetOperation();
						ret = true;
						break;
					//データベース更新要求メッセージ
					case CellMonTabMsgGetUpdateDb.subCmdName:
						msg = new CellMonTabMsgGetUpdateDb();
						ret = true;
						break;
					//MU挿入要求メッセージ
					case CellMonTabMsgGetInsert.subCmdName:
						msg = new CellMonTabMsgGetInsert();
						ret = true;
						break;
					//MU解除要求メッセージ
					case CellMonTabMsgGetEject.subCmdName:
						msg = new CellMonTabMsgGetEject();
						ret = true;
						break;
					default:
						break;
				}
			}
			else if (cmd == CellMonTabMessage.CmdPut)
			{
				// PUTコマンド
				switch (subCmd)
				{
					//テーブル更新通知メッセージ
					case CellMonTabMsgPutUpdateTable.subCmdName:
						msg = new CellMonTabMsgPutUpdateTable();
						ret = true;
						break;
					default:
						break;
				}
			}

			return ret;
		}

		/// <summary>
		/// コマンドメッセージから応答メッセージを取得
		/// </summary>
		/// <param name="cmdMsg"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		private bool GetResMessage(CellMonTabMessage cmdMsg, out CellMonTabMessage msg)
		{
			bool ret = false;
			msg = null;

			/// データベース更新結果通知メッセージ
			if (cmdMsg.GetType() == typeof(CellMonTabMsgPutUpdateDbResult))
			{
				msg = new CellMonTabMsgPutUpdateDbResultResponse();
				ret = true;
			}

			return ret;
		}

		/// <summary>
		/// コマンド、サブコマンドからメッセージを取得(監視モニタ、タブレットシミュレーション用)
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="subCmd"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		private bool GetMessageMonTabSim(string cmd, string subCmd, out CellMonTabMessage msg)
		{
			bool ret = false;
			msg = null;

			if (cmd == CellMonTabMessage.CmdGet)
			{
				// GETコマンド
				switch (subCmd)
				{
					default:
						break;
				}
			}
			else if (cmd == CellMonTabMessage.CmdPut)
			{
				// PUTコマンド
				switch (subCmd)
				{
					//データベース更新結果通知メッセージ
					case CellMonTabMsgPutUpdateDbResult.subCmdName:
						msg = new CellMonTabMsgPutUpdateDbResult();
						ret = true;
						break;
					default:
						break;
				}
			}

			return ret;
		}

		/// <summary>
		/// コマンドメッセージから応答メッセージを取得(監視モニタ、タブレットシミュレーション用)
		/// </summary>
		/// <param name="cmdMsg"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		private bool GetResMessageMonTabSim(CellMonTabMessage cmdMsg, out CellMonTabMessage msg)
		{
			bool ret = false;
			msg = null;

			/// Cell運転操作要求メッセージ
			if (cmdMsg.GetType() == typeof(CellMonTabMsgGetOperation))
			{
				msg = new CellMonTabMsgGetOperationResponse();
				ret = true;
			}
			/// テーブル更新通知メッセージ
			else if (cmdMsg.GetType() == typeof(CellMonTabMsgPutUpdateTable))
			{
				msg = new CellMonTabMsgPutUpdateTableResponse();
				ret = true;
			}

			/// データベース更新要求メッセージ
			else if (cmdMsg.GetType() == typeof(CellMonTabMsgGetUpdateDb))
			{
				msg = new CellMonTabMsgGetUpdateDbResponse();
				ret = true;
			}
			/// MU挿入要求メッセージ
			else if (cmdMsg.GetType() == typeof(CellMonTabMsgGetInsert))
			{
				msg = new CellMonTabMsgGetInsertResponse();
				ret = true;
			}
			/// MU解除要求メッセージ
			else if (cmdMsg.GetType() == typeof(CellMonTabMsgGetEject))
			{
				msg = new CellMonTabMsgGetEjectResponse();
				ret = true;
			}

			return ret;
		}


		/// <summary>
		/// メッセージ送信
		/// </summary>
		/// <param name="message">送信メッセージ</param>
		/// <returns>送信結果</returns>
		public bool SendMessage(CellMonTabMessage message)
		{
			bool ret = false;

			// 送信メッセージバッファ書込み
			if (true == this.sendMessageBuf.TryAdd(message))
			{
				// 送信要求イベント設定
				if (true == this.eventSendProc[(int)eEventSendProc.ReqSend].Set())
				{
					ret = true;
				}
			}

			return ret;
		}

		/// <summary>
		/// メッセージ応答送信
		/// </summary>
		/// <param name="message">送信メッセージ</param>
		/// <returns>送信結果</returns>
		public bool SendMessageRes(CellMonTabMessage message)
		{
			bool ret = false;

			// 送信メッセージバッファ書込み
			if (true == this.sendMessageResBuf.TryAdd(message))
			{
				// 送信要求イベント設定
				if (true == this.eventSendProc[(int)eEventSendProc.ReqSendRes].Set())
				{
					ret = true;
				}
			}

			return ret;
		}

		/// <summary>
		/// メッセージ受信
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool RecvMessage(out CellMonTabMessage message)
		{
			bool ret = false;
			message = null;

			// 受信メッセージバッファ読出し
			if (true == this.recvMessageBuf.TryTake(out message))
			{
				ret = true;
			}

			return ret;
		}


		/// <summary>
		/// メッセージ送信完了イベント通知
		/// </summary>
		private void NotifySentMessageEvent(CellMonTabMessage sntMsg, CellMonTabMessage resMsg)
		{
			if (this.sentMessageEvent != null)
			{
				this.sentMessageEvent(sntMsg, resMsg);
			}
		}

		/// <summary>
		/// メッセージ送信失敗イベント通知
		/// </summary>
		private void NotifySendMessageFialEvent(CellMonTabMessage message)
		{
			if (this.sentMessageEvent != null)
			{
				this.sendMessageFailEvent(message);
			}
		}

		/// <summary>
		/// メッセージ受信イベント通知
		/// </summary>
		private void NotifyReceivedMessageEvent(CellMonTabMessage message)
		{
			if (this.recvdMessageEvent != null)
			{
				this.recvdMessageEvent(message);

			}
		}
		#endregion



	}
}
