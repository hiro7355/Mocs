using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace Mocs.CellMonTabNet
{

	/// <summary>
	/// 監視モニタ、タブレット通信メッセージインターフェース
	/// </summary>
	interface ICellMonTabMessage
	{
		/// <summary>
		/// パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		bool MakePacket();

		/// <summary>
		/// パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		bool ParsePacket();

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns></returns>
		bool CheckData();
	}

	/// <summary>
	/// 監視モニタ、タブレット通信メッセージクラス
	/// </summary>
	public abstract class CellMonTabMessage
		: ICellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		const string cmdName = "";

		/// <summary>
		/// サブコマンド名
		/// </summary>
		const string subCmdName = "";

		/// <summary>
		/// 要求メッセージコマンド
		/// </summary>
		public const string CmdGet = "get";

		/// <summary>
		/// 通知メッセージコマンド
		/// </summary>
		public const string CmdPut = "put";

		/// <summary>
		/// メッセージ応答文字列
		/// </summary>
		public const string CmdRes = "res";

		/// <summary>
		/// メッセージ正常応答文字列
		/// </summary>
		public const string ResOk = "ok";

		/// <summary>
		/// メッセージ異常応答文字列
		/// </summary>
		public const string ResNg = "ng";

		/// <summary>
		/// メッセージ区切り文字
		/// </summary>
		public const char Delim = '\t';

		#endregion

		#region 型定義

		/// <summary>
		/// 送信状態定義
		/// </summary>
		public enum eSendStatus
		{
			WaitSend = 0,       // 送信待ち
			Sent,               // 送信済み
			RecvRes,            // 応答受信
			SendNg,             // 送信失敗
		}

		/// <summary>
		/// メッセージ応答定義
		/// </summary>
		public enum eResultResponse
		{
			Ok = 0,         // OK
			Ng,             // NG
		}

		#endregion

		#region メンバ変数

		/// <summary>
		/// 送信状態定義
		/// </summary>
		protected eSendStatus _SendStatus;

		/// <summary>
		/// シーケンス番号
		/// </summary>
		private UInt16 _SequenceNo;

		/// <summary>
		/// メッセージ応答
		/// </summary>
		public eResultResponse MsgRes;

		/// <summary>
		/// メッセージ用データ
		/// </summary>
		protected string _ErrorMessage;

		/// <summary>
		/// メッセージ用データ
		/// </summary>
		public string _MessageData;

		/// <summary>
		/// 送信リトライ回数
		/// </summary>
//		private int sendRetryNum = 3;

		/// <summary>
		/// 送信タイムアウト時間[msec]
		/// </summary>
//		private int sendTimeOut = 5000;

		/// <summary>
		/// メッセージ受信イベント
		/// </summary>
		public delegate void MessageRecvMonTabent();
//		public event MessageRecvMonTabent MsgRecvMonTabent;

		/// <summary>
		/// メッセージ送信完了イベント
		/// </summary>
		public delegate void MessageSentMonTabent();
//		public event MessageSentMonTabent MsgSentMonTabent;

		#endregion

		#region プロパティ

		/// <summary>
		/// コマンド名
		/// </summary>
		public virtual string CmdName
		{
			get { return CellMonTabMessage.cmdName; }
		}

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public virtual string SubCmdName
		{
			get { return CellMonTabMessage.subCmdName; }
		}

		/// <summary>
		/// シーケンス番号
		/// </summary>
		public UInt16 SequenceNo
		{
			get { return this._SequenceNo; }
			set { this._SequenceNo = value; }
		}

		/// <summary>
		/// 送信状態定義
		/// </summary>
		public eSendStatus SendStatus
		{
			get { return this._SendStatus; }
			set { this._SendStatus = value; }
		}

		/// <summary>
		/// エラーメッセージ
		/// </summary>
		public string ErrorMessage
		{
			get { return this._ErrorMessage; }
		}

		/// <summary>
		/// メッセージ用データ
		/// </summary>
		public string MessageData
		{
			get { return this._MessageData; }
			set { this._MessageData = value; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// コンストラクター
		/// </summary>
		public CellMonTabMessage()
		{

		}

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public virtual bool MakePacket()
		{
			return false;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public virtual bool ParsePacket()
		{
			return false;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public virtual bool CheckData()
		{
			return false;
		}

		#endregion

	}

	/// <summary>
	/// 監視モニタ、タブレットメッセージエラークラス
	/// </summary>
	public class CellMonTabMsgError
	{
		#region 固定値

		/// <summary>
		/// エラーレベル接頭語
		/// </summary>
		public const string msgHeaderErrorLevel = "ele";
		/// <summary>
		/// エラーコード接頭語
		/// </summary>
		public const string msgHeaderErrorCode = "err";


		/// <summary>
		/// エラーメッセージ(エラーなし)
		/// </summary>
		private const string ErrorMsgNone = "no error";

		/// <summary>
		/// エラーメッセージ(監視モニタ、タブレット初期化中)
		/// </summary>
		private const string ErrorMsgMonTabOnInit = "monitor(tablet) on init";

		/// <summary>
		/// 監視モニタ、タブレットレディでない
		/// </summary>
		private const string ErrorMsgMonTabNotReady = "monitor(tablet) not ready";

		/// <summary>
		/// 監視モニタ、タブレットモード不正
		/// </summary>
		private const string ErrorMsgMonTabInvalidMode = "invalid mode";

		/// <summary>
		/// エラーメッセージ(コマンドパラメータ不正)
		/// </summary>
		private const string ErrorMsgCmdInvalidParam = "invalid command parameter";

		/// <summary>
		/// エラーメッセージ(不明なエラー)
		/// </summary>
		private const string ErrorMsgUnknownError = "unknown error";

		#endregion

		#region 型定義
		/// <summary>
		/// コマンドエラー要因
		/// </summary>
		public enum eCmdError
		{
			None = 0,           // エラーなし
			MonTabOnInit,           // 監視モニタ、タブレット初期化中
			MonTabNotReady,         // 監視モニタ、タブレットレディでない
			MonTabInvalidMode,      // モード不正
			CmdInvalidParam,    // コマンドパラメータ不正
			UnknownError,       // 不明なエラー
		}
		#endregion

		#region メンバ変数
		/// <summary>
		/// コマンドエラー要因
		/// </summary>
		public eCmdError cmdError;
		#endregion

		#region プロパティ
		#endregion

		#region メソッド

		/// <summary>
		/// コンストラクター
		/// </summary>
		/// <param name="cmdErr">コマンドエラー</param>
		public CellMonTabMsgError(eCmdError cmdErr)
		{
			this.cmdError = cmdErr;
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <returns></returns>
		public string GetErrorMessage()
		{
			string errMsg = "";

			switch (this.cmdError)
			{
				case eCmdError.None:
					errMsg = ErrorMsgNone;
					break;
				case eCmdError.MonTabOnInit:
					errMsg = ErrorMsgMonTabOnInit;
					break;
				case eCmdError.MonTabNotReady:
					errMsg = ErrorMsgMonTabNotReady;
					break;
				case eCmdError.MonTabInvalidMode:
					errMsg = ErrorMsgMonTabInvalidMode;
					break;
				case eCmdError.CmdInvalidParam:
					errMsg = ErrorMsgCmdInvalidParam;
					break;
				default:
					errMsg = ErrorMsgUnknownError;
					break;
			}

			return errMsg;
		}

		/// <summary>
		/// エラー種別取得
		/// </summary>
		/// <returns></returns>
		public static eCmdError GetErrorType(string errMsg)
		{
			eCmdError errType = eCmdError.None;

			switch (errMsg)
			{
				case ErrorMsgNone:
					errType = eCmdError.None;
					break;
				case ErrorMsgMonTabOnInit:
					errType = eCmdError.MonTabOnInit;
					break;
				case ErrorMsgMonTabNotReady:
					errType = eCmdError.MonTabNotReady;
					break;
				case ErrorMsgMonTabInvalidMode:
					errType = eCmdError.MonTabInvalidMode;
					break;
				case ErrorMsgCmdInvalidParam:
					errType = eCmdError.CmdInvalidParam;
					break;
				default:
					errType = eCmdError.UnknownError;
					break;
			}

			return errType;
		}

		/// <summary>
		/// エラーレベルメッセージからエラーレベル取得
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static UInt32 GetErrorLevel(string message)
		{
			UInt32 errLevel = 0;

			// エラーレベルの展開
			if (CellMonTabMsgError.msgHeaderErrorLevel == message.Substring(0, CellMonTabMsgError.msgHeaderErrorLevel.Length))
			{
				errLevel = Convert.ToUInt32(message.Substring(CellMonTabMsgError.msgHeaderErrorLevel.Length));
			}

			return errLevel;
		}

		/// <summary>
		/// エラーレベルからエラーレベルメッセージ取得
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static string GetErrorLevelMessage(UInt32 errLevel)
		{
			string message = CellMonTabMsgError.msgHeaderErrorLevel + errLevel.ToString();

			return message;
		}

		/// <summary>
		/// エラーコードメッセージからエラーコード取得
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static UInt32 GetErrorCode(string message)
		{
			UInt32 errCode = 0;

			// エラーコードの展開
			if (CellMonTabMsgError.msgHeaderErrorCode == message.Substring(0, CellMonTabMsgError.msgHeaderErrorCode.Length))
			{
				errCode = Convert.ToUInt32(message.Substring(CellMonTabMsgError.msgHeaderErrorCode.Length));
			}

			return errCode;
		}

		/// <summary>
		/// エラーコードからエラーコードメッセージ取得
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static string GetErrorCodeMessage(UInt32 errCode)
		{
			string message = CellMonTabMsgError.msgHeaderErrorCode + errCode.ToString();

			return message;
		}

		#endregion
	}

	/// <summary>
	/// 監視モニタ、タブレット処理結果
	/// </summary>
	public class MonTabProcResult
	{
		#region 固定値

		/// <summary>
		/// 監視モニタ、タブレット処理結果のメッセージ内文字列
		/// </summary>
		const string msgMonTabResultOk = "ok";
		const string msgMonTabResultNg = "ng";
		const string msgMonTabResultUnknown = "unknown";

		#endregion

		public enum eResult
		{
			Ok = 0,         // Ok
			Ng,             // Ng
			Unknown,        // 不明
		}

		#region メソッド

		/// <summary>
		/// メッセージから監視モニタ、タブレット処理結果情報取得
		/// </summary>
		/// <param name="message">ステータス情報</param>
		/// <returns></returns>
		public static MonTabProcResult.eResult GetResult(string message)
		{
			MonTabProcResult.eResult result;

			switch (message)
			{
				case msgMonTabResultOk:
					result = MonTabProcResult.eResult.Ok;           // Ok
					break;
				case msgMonTabResultNg:
					result = MonTabProcResult.eResult.Ng;           // Ng
					break;
				case msgMonTabResultUnknown:
					result = MonTabProcResult.eResult.Unknown;    // 不明
					break;
				default:
					result = MonTabProcResult.eResult.Unknown;
					throw new Exception("invalid parameter");
			}

			return result;
		}

		/// <summary>
		/// メッセージから監視モニタ、タブレット処理結果情報取得
		/// </summary>
		/// <param name="result">ステータス情報</param>
		/// <returns></returns>
		public static string GetResultMessage(MonTabProcResult.eResult result)
		{
			string message;

			switch (result)
			{
				case MonTabProcResult.eResult.Ok:
					message = msgMonTabResultOk;            // Ok
					break;
				case MonTabProcResult.eResult.Ng:
					message = msgMonTabResultNg;            // Ng
					break;
				case MonTabProcResult.eResult.Unknown:
					message = msgMonTabResultUnknown;       // 不明
					break;
				default:
					message = msgMonTabResultUnknown;
					throw new Exception("invalid parameter");
			}

			return message;
		}

		#endregion
	}

	/// <summary>
	/// Cell運転操作種別
	/// </summary>
	public class CellOperationType
	{
		#region 固定値

		/// <summary>
		/// Cell運転操作種別のメッセージ内文字列
		/// </summary>
		const string msgTypeNone = "none";              // なし
		const string msgTypeStart = "start";            // 起動
		const string msgTypeCycleStop = "cyclestop";    // サイクル停止
		const string msgTypeStop = "stop";              // 停止
		const string msgTypeRecovery = "recovery";      // 管制運転復帰
		const string msgTypeUnknown = "unknown";        // 不明

		#endregion

		#region 型定義
		public enum eType
		{
			None = 0,       // なし
			Start,          // 起動
			CycleStop,      // サイクル停止
			Stop,           // 停止
			Recovery,       // 管制運転復帰
			Unknown,        // 不明
		}
		#endregion

		#region メンバ変数

		/// <summary>
		/// Cell運転操作種別
		/// </summary>
		public eType type;

		#endregion

		#region メソッド

		/// <summary>
		/// メッセージからCell運転操作種別情報取得
		/// </summary>
		/// <param name="message">Cell運転操作種別</param>
		/// <returns></returns>
		public static CellOperationType.eType GetType(string message)
		{
			CellOperationType.eType type;

			switch (message)
			{
				case msgTypeNone:
					type = CellOperationType.eType.None;            // なし
					break;
				case msgTypeStart:
					type = CellOperationType.eType.Start;           // 起動
					break;
				case msgTypeCycleStop:
					type = CellOperationType.eType.CycleStop;       // サイクル停止
					break;
				case msgTypeStop:
					type = CellOperationType.eType.Stop;            // 停止
					break;
				case msgTypeRecovery:
					type = CellOperationType.eType.Recovery;        // 管制運転復帰
					break;
				default:
					type = CellOperationType.eType.Unknown;
					throw new Exception("invalid parameter");
			}

			return type;
		}

		/// <summary>
		/// Cell運転操作種別情報からメッセージ取得
		/// </summary>
		/// <param name="type">Cell運転操作種別</param>
		/// <returns></returns>
		public static string GetTypeMessage(CellOperationType.eType type)
		{
			string message;

			switch (type)
			{
				case CellOperationType.eType.None:
					message = msgTypeNone;              // なし
					break;
				case CellOperationType.eType.Start:
					message = msgTypeStart;             // 起動
					break;
				case CellOperationType.eType.CycleStop:
					message = msgTypeCycleStop;         // サイクル停止
					break;
				case CellOperationType.eType.Stop:
					message = msgTypeStop;              // 停止
					break;
				case CellOperationType.eType.Recovery:
					message = msgTypeRecovery;          // 管制運転復帰
					break;
				default:
					message = msgTypeUnknown;           // 不明状態
					throw new Exception("invalid parameter");
			}

			return message;
		}

		#endregion
	}

	/// <summary>
	/// Cell運転操作要求メッセージ
	/// </summary>
	public class CellMonTabMsgGetOperation
		: CellMonTabMessage
	{
		#region 固定値

		#region メッセージ文字列
		#endregion

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdGet;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "ope";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#region メッセージデータ定義

		/// <summary>
		/// メッセージデータ
		/// </summary>
		public string Message = "";

		/// <summary>
		/// Cell運転操作種別
		/// </summary>
		public CellOperationType.eType opeType;

		#endregion

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetOperation.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetOperation.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				this.Message = CellOperationType.GetTypeMessage(this.opeType);
				this.MessageData = cmdName + Delim + subCmdName + Delim + this.Message;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					while (true)
					{
						// コマンドチェック
						if (parseMsg[0] != cmdName)
						{
							break;
						}
						// サブコマンドチェック
						if (parseMsg[1] != subCmdName)
						{
							break;
						}

						// Cell運転操作種別展開
						this.opeType = CellOperationType.GetType(parseMsg[2]);

						ret = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// Cell運転操作要求メッセージ応答
	/// </summary>
	public class CellMonTabMsgGetOperationResponse
		: CellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdRes;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "ope";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetOperationResponse.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetOperationResponse.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				// 応答結果に従い
				// 結果、サブコマンド作成
				if (this.cmdErrInfo.cmdError == CellMonTabMsgError.eCmdError.None)
				{
					// 正常応答
					this.MessageData = ResOk;
				}
				else if (this.MsgRes == eResultResponse.Ng)
				{
					// 異常応答
					this.MessageData = ResNg;
				}
				this.MessageData += Delim + subCmdName;
				this.MessageData += Delim + this.cmdErrInfo.GetErrorMessage();

				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					// コマンドチェック
					if (parseMsg[0] != ResOk && parseMsg[0] != ResNg)
					{
						throw new Exception("Invalid response message.");
					}
					// サブコマンドチェック
					if (parseMsg[1] != subCmdName)
					{
						throw new Exception("mismatch sub command name.");
					}

					if (parseMsg[0] != ResOk)
					{
						// エラー応答
						this.MsgRes = eResultResponse.Ng;
						this.cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.GetErrorType(parseMsg[2]));
					}
					else
					{
						// 正常応答
					}

					ret = true;
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// テーブル更新通知メッセージ
	/// </summary>
	public class CellMonTabMsgPutUpdateTable
		: CellMonTabMessage
	{
		#region 固定値

		#region メッセージ文字列
		#endregion

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdPut;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "updatetab";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#region メッセージデータ定義

		/// <summary>
		/// メッセージデータ
		/// </summary>
		public string Message = "";

		/// <summary>
		/// 更新テーブルリスト
		/// </summary>
		public List<string> tabList = new List<string>();

		#endregion

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgPutUpdateTable.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgPutUpdateTable.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				this.Message = "";
				foreach (var tab in this.tabList)
				{
					this.Message = tab + Delim;
				}

				this.MessageData = cmdName + Delim + subCmdName + Delim + this.Message;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					while (true)
					{
						// コマンドチェック
						if (parseMsg[0] != cmdName)
						{
							break;
						}
						// サブコマンドチェック
						if (parseMsg[1] != subCmdName)
						{
							break;
						}

						// 更新テーブル展開
						int tabCount = parseMsg.Count() - 2;
						if (tabCount > 0)
						{
							for (int cnt = 0; cnt < tabCount; cnt++)
							{
								this.tabList.Add(parseMsg[2 + cnt]);
							}
						}

						ret = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// テーブル更新通知メッセージ応答
	/// </summary>
	public class CellMonTabMsgPutUpdateTableResponse
		: CellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdRes;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "updatetab";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgPutUpdateTableResponse.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgPutUpdateTableResponse.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				// 応答結果に従い
				// 結果、サブコマンド作成
				if (this.cmdErrInfo.cmdError == CellMonTabMsgError.eCmdError.None)
				{
					// 正常応答
					this.MessageData = ResOk;
				}
				else if (this.MsgRes == eResultResponse.Ng)
				{
					// 異常応答
					this.MessageData = ResNg;
				}
				this.MessageData += Delim + subCmdName;
				this.MessageData += Delim + this.cmdErrInfo.GetErrorMessage();

				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					// コマンドチェック
					if (parseMsg[0] != ResOk && parseMsg[0] != ResNg)
					{
						throw new Exception("Invalid response message.");
					}
					// サブコマンドチェック
					if (parseMsg[1] != subCmdName)
					{
						throw new Exception("mismatch sub command name.");
					}

					if (parseMsg[0] != ResOk)
					{
						// エラー応答
						this.MsgRes = eResultResponse.Ng;
						this.cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.GetErrorType(parseMsg[2]));
					}
					else
					{
						// 正常応答
					}

					ret = true;
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// データベース更新要求メッセージ
	/// </summary>
	public class CellMonTabMsgGetUpdateDb
		: CellMonTabMessage
	{
		#region 固定値

		#region メッセージ文字列
		#endregion

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdGet;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "update_db";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#region メッセージデータ定義

		/// <summary>
		/// メッセージデータ
		/// </summary>
		public string Message = "";

		/// <summary>
		/// レイアウトデータ設定ファイルパス
		/// </summary>
		public string layDataPath = "";

		/// <summary>
		/// マスタデータ設定ファイルパス
		/// </summary>
		public string masterDataPath = "";

		#endregion

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetUpdateDb.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetUpdateDb.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				this.Message = this.layDataPath + Delim + this.masterDataPath;
				this.MessageData = cmdName + Delim + subCmdName + Delim + this.Message;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					while (true)
					{
						// コマンドチェック
						if (parseMsg[0] != cmdName)
						{
							break;
						}
						// サブコマンドチェック
						if (parseMsg[1] != subCmdName)
						{
							break;
						}

						// 設定ファイルパス展開
						this.layDataPath = parseMsg[2];
						this.masterDataPath = parseMsg[3];

						ret = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// データベース更新要求メッセージ応答
	/// </summary>
	public class CellMonTabMsgGetUpdateDbResponse
		: CellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdRes;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "update_db";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetUpdateDbResponse.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetUpdateDbResponse.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				// 応答結果に従い
				// 結果、サブコマンド作成
				if (this.cmdErrInfo.cmdError == CellMonTabMsgError.eCmdError.None)
				{
					// 正常応答
					this.MessageData = ResOk;
				}
				else if (this.MsgRes == eResultResponse.Ng)
				{
					// 異常応答
					this.MessageData = ResNg;
				}
				this.MessageData += Delim + subCmdName;
				this.MessageData += Delim + this.cmdErrInfo.GetErrorMessage();

				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					// コマンドチェック
					if (parseMsg[0] != ResOk && parseMsg[0] != ResNg)
					{
						throw new Exception("Invalid response message.");
					}
					// サブコマンドチェック
					if (parseMsg[1] != subCmdName)
					{
						throw new Exception("mismatch sub command name.");
					}

					if (parseMsg[0] != ResOk)
					{
						// エラー応答
						this.MsgRes = eResultResponse.Ng;
						this.cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.GetErrorType(parseMsg[2]));
					}
					else
					{
						// 正常応答
					}

					ret = true;
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// データベース更新結果通知メッセージ
	/// </summary>
	public class CellMonTabMsgPutUpdateDbResult
		: CellMonTabMessage
	{
		#region 固定値

		#region メッセージ文字列
		#endregion

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdPut;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "update_db_res";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#region メッセージデータ定義

		/// <summary>
		/// メッセージデータ
		/// </summary>
		public string Message = "";

		/// <summary>
		/// データベース更新結果
		/// </summary>
		public MonTabProcResult.eResult result;

		#endregion

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgPutUpdateDbResult.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgPutUpdateDbResult.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				this.Message = MonTabProcResult.GetResultMessage(this.result);
				this.MessageData = cmdName + Delim + subCmdName + Delim + this.Message;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					while (true)
					{
						// コマンドチェック
						if (parseMsg[0] != cmdName)
						{
							break;
						}
						// サブコマンドチェック
						if (parseMsg[1] != subCmdName)
						{
							break;
						}

						// データベース更新結果の展開
						this.result = MonTabProcResult.GetResult(parseMsg[2]);

						ret = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// データベース更新結果通知メッセージ応答
	/// </summary>
	public class CellMonTabMsgPutUpdateDbResultResponse
		: CellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdRes;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "update_db_res";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgPutUpdateDbResultResponse.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgPutUpdateDbResultResponse.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				// 応答結果に従い
				// 結果、サブコマンド作成
				if (this.cmdErrInfo.cmdError == CellMonTabMsgError.eCmdError.None)
				{
					// 正常応答
					this.MessageData = ResOk;
				}
				else if (this.MsgRes == eResultResponse.Ng)
				{
					// 異常応答
					this.MessageData = ResNg;
				}
				this.MessageData += Delim + subCmdName;
				this.MessageData += Delim + this.cmdErrInfo.GetErrorMessage();

				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					// コマンドチェック
					if (parseMsg[0] != ResOk && parseMsg[0] != ResNg)
					{
						throw new Exception("Invalid response message.");
					}
					// サブコマンドチェック
					if (parseMsg[1] != subCmdName)
					{
						throw new Exception("mismatch sub command name.");
					}

					if (parseMsg[0] != ResOk)
					{
						// エラー応答
						this.MsgRes = eResultResponse.Ng;
						this.cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.GetErrorType(parseMsg[2]));
					}
					else
					{
						// 正常応答
					}

					ret = true;
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// MU挿入要求メッセージ
	/// </summary>
	public class CellMonTabMsgGetInsert
		: CellMonTabMessage
	{
		#region 固定値

		#region メッセージ文字列
		#endregion

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdGet;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "insert";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#region メッセージデータ定義

		/// <summary>
		/// メッセージデータ
		/// </summary>
		public string Message = "";

		/// <summary>
		/// MU識別子
		/// </summary>
		public UInt32 MuId;

		/// <summary>
		/// 挿入ポイントID
		/// </summary>
		public UInt32 PointId;

		#endregion

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetInsert.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetInsert.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				this.Message = this.MuId.ToString() + Delim + this.PointId.ToString();
				this.MessageData = cmdName + Delim + subCmdName + Delim + this.Message;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					while (true)
					{
						// コマンドチェック
						if (parseMsg[0] != cmdName)
						{
							break;
						}
						// サブコマンドチェック
						if (parseMsg[1] != subCmdName)
						{
							break;
						}

						// メッセージ展開
						this.MuId = Convert.ToUInt32(parseMsg[2]);
						this.PointId = Convert.ToUInt32(parseMsg[3]);

						ret = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// MU挿入要求メッセージ応答
	/// </summary>
	public class CellMonTabMsgGetInsertResponse
		: CellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdRes;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "insert";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetInsertResponse.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetInsertResponse.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				// 応答結果に従い
				// 結果、サブコマンド作成
				if (this.cmdErrInfo.cmdError == CellMonTabMsgError.eCmdError.None)
				{
					// 正常応答
					this.MessageData = ResOk;
				}
				else if (this.MsgRes == eResultResponse.Ng)
				{
					// 異常応答
					this.MessageData = ResNg;
				}
				this.MessageData += Delim + subCmdName;
				this.MessageData += Delim + this.cmdErrInfo.GetErrorMessage();

				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					// コマンドチェック
					if (parseMsg[0] != ResOk && parseMsg[0] != ResNg)
					{
						throw new Exception("Invalid response message.");
					}
					// サブコマンドチェック
					if (parseMsg[1] != subCmdName)
					{
						throw new Exception("mismatch sub command name.");
					}

					if (parseMsg[0] != ResOk)
					{
						// エラー応答
						this.MsgRes = eResultResponse.Ng;
						this.cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.GetErrorType(parseMsg[2]));
					}
					else
					{
						// 正常応答
					}

					ret = true;
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// MU解除要求メッセージ
	/// </summary>
	public class CellMonTabMsgGetEject
		: CellMonTabMessage
	{
		#region 固定値

		#region メッセージ文字列
		#endregion

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdGet;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "eject";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#region メッセージデータ定義

		/// <summary>
		/// メッセージデータ
		/// </summary>
		public string Message = "";

		/// <summary>
		/// MU識別子
		/// </summary>
		public UInt32 MuId;

		#endregion

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetEject.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetEject.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				this.Message = this.MuId.ToString();
				this.MessageData = cmdName + Delim + subCmdName + Delim + this.Message;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					while (true)
					{
						// コマンドチェック
						if (parseMsg[0] != cmdName)
						{
							break;
						}
						// サブコマンドチェック
						if (parseMsg[1] != subCmdName)
						{
							break;
						}

						// メッセージ展開
						this.MuId = Convert.ToUInt32(parseMsg[2]);

						ret = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

	/// <summary>
	/// MU解除要求メッセージ応答
	/// </summary>
	public class CellMonTabMsgGetEjectResponse
		: CellMonTabMessage
	{
		#region 固定値

		/// <summary>
		/// コマンド名
		/// </summary>
		public const string cmdName = CmdRes;

		/// <summary>
		/// サブコマンド名
		/// </summary>
		public const string subCmdName = "eject";

		#endregion

		#region 型定義
		#endregion

		#region メンバ変数

		/// <summary>
		/// コマンドエラー処理
		/// </summary>
		public CellMonTabMsgError cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.eCmdError.None);

		#endregion

		#region プロパティ
		/// <summary>
		/// コマンド名
		/// </summary>
		public override string CmdName
		{
			get { return CellMonTabMsgGetEjectResponse.cmdName; }
		}
		/// <summary>
		/// サブコマンド名
		/// </summary>
		public override string SubCmdName
		{
			get { return CellMonTabMsgGetEjectResponse.subCmdName; }
		}
		#endregion

		#region メソッド

		/// <summary>
		/// 送信パケット作成
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool MakePacket()
		{
			bool ret = false;

			try
			{
				// 応答結果に従い
				// 結果、サブコマンド作成
				if (this.cmdErrInfo.cmdError == CellMonTabMsgError.eCmdError.None)
				{
					// 正常応答
					this.MessageData = ResOk;
				}
				else if (this.MsgRes == eResultResponse.Ng)
				{
					// 異常応答
					this.MessageData = ResNg;
				}
				this.MessageData += Delim + subCmdName;
				this.MessageData += Delim + this.cmdErrInfo.GetErrorMessage();

				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Make Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// 受信パケット展開
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool ParsePacket()
		{
			bool ret = false;
			string[] parseMsg;

			try
			{
				if (this.MessageData != null && this.MessageData.Length != 0)
				{
					parseMsg = this.MessageData.Split(Delim);
					// コマンドチェック
					if (parseMsg[0] != ResOk && parseMsg[0] != ResNg)
					{
						throw new Exception("Invalid response message.");
					}
					// サブコマンドチェック
					if (parseMsg[1] != subCmdName)
					{
						throw new Exception("mismatch sub command name.");
					}

					if (parseMsg[0] != ResOk)
					{
						// エラー応答
						this.MsgRes = eResultResponse.Ng;
						this.cmdErrInfo = new CellMonTabMsgError(CellMonTabMsgError.GetErrorType(parseMsg[2]));
					}
					else
					{
						// 正常応答
					}

					ret = true;
				}
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Parse Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		/// <summary>
		/// データチェック
		/// </summary>
		/// <returns>処理結果</returns>
		public override bool CheckData()
		{
			bool ret = false;

			try
			{
				ret = true;
			}
			catch (Exception ex)
			{
				this._ErrorMessage = "Check Error:" + this.GetType().FullName + ":" + ex.Message;
			}

			return ret;
		}

		#endregion
	}

}
