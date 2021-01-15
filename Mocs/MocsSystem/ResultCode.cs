using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.MoCsSystem
{
	static class ResultCode
	{
		//エラーコード定義
		/// <summary>
		/// 0:戻り値正常
		/// </summary>
		public const int OK = 0;

#if false
		/// <summary>
		/// 初期化不整合発生
		/// </summary>
		public const int WARNNING_INITIALIZE = 0;

		/// <summary>
		/// ファイル排他失敗
		/// </summary>
		public const int WARNNING_FILELOCKFAILED = 6;

		/// <summary>
		/// 処理中の例外発生
		/// </summary>
		public const int ERROR_EXCEPTION = 7;

		/// <summary>
		/// パラメータ説明
		/// </summary>
		public const int PARAM_COMMENT = -99999;

		/// <summary>
		/// 入力ファイルが存在しない
		/// </summary>
		public const int ERROR_FILENOTFOUND = 5;

		/// <summary>レコードが存在しない
		/// </summary>
		public const int ERROR_RECORDNOTFOUND = 100;

		/// <summary>ヘッダーレコードが存在しない
		/// </summary>
		public const int ERROR_HEADER_RECORD_NOTFOUND = 101;

		/// <summary>ヘッダーレコード項目数不足
		/// </summary>
		public const int ERROR_HEADER_RECORD_ITEM_SHORTNESS = 102;

		/// <summary>ヘッダーレコード長エラー
		/// </summary>
		public const int ERROR_HEADER_RECORD_LENGTH_MISMATCH = 103;

		/// <summary>レコード長エラー
		/// </summary>
		public const int ERROR_RECORD_LENGTH_MISMATCH = 104;

		/// <summary>フッターレコードなし
		/// </summary>
		public const int ERROR_FOOTER_RECORD_NOTFOUND = 105;

		/// <summary>データ件数不整合
		/// </summary>
		public const int ERROR_DATA_RECORD_COUNT_UNMATCH = 106;

		/// <summary>レコード不整合（フッターレコード以降にレコードあり等
		/// </summary>
		public const int ERROR_RECORD_MISMATCH = 107;

		/// <summary>レコード項目数不整合
		/// </summary>
		public const int ERROR_RECORD_ITEM_COUNT_MISMATCH = 109;

		/// <summary>該当フォルダなし
		/// </summary>
		public const int ERROR_NOT_FOLDER = 110;

		/// <summary>項目桁数不整合
		/// </summary>
		public const int ERROR_ITEM_LENGTH_MISMATCH = 111;

		/// <summary>重複エラー
		/// </summary>
		public const int ERROR_DUPLICATION = 112;

		/// <summary>ファイルオープンエラー
		/// </summary>
		public const int ERROR_FILE_OPEN = 113;

		/// <summary>ファイルオープンエラー
		/// </summary>
		public const int ERROR_FILE_EXIST = 114;
#endif
		/// <summary>
		/// その他エラー
		/// </summary>
		public const int ERROR_OTHER = -1;

		/// <summary>
		/// データレコードが０件
		/// </summary>
		public const int WARNNING_NODATARECORD = 4;

		/// <summary>
		/// 初期化エラー
		/// </summary>
		public const int ERROR_INITIALIZE = 8;

		/// <summary>DB例外発生
		/// </summary>
		public const int ERROR_DB_EXCEPTION = 108;


		/// <summary> レイアウト登録エラー
		/// </summary>
		public const int ERROR_REGISTER_LAYOUT = 1000;
		/// <summary> レイアウトアクセスエラー
		/// </summary>
		public const int ERROR_ACCESS_LAYOUT = 1001;

		/// <summary> ブロッキング登録エラー
		/// </summary>
		public const int ERROR_REGISTER_BLOCKING = 1010;
		/// <summary> ブロッキングアクセスエラー
		/// </summary>
		public const int ERROR_ACCESS_BLOCKING = 1011;

		/// <summary>TCPリッスンエラー
		/// </summary>
		public const int ERROR_TCP_LISTEN = 2000;

		/// <summary>UDPリッスンエラー
		/// </summary>
		public const int ERROR_UDP_LISTEN = 2100;

		/// <summary>WiFi通信送信処理異常
		/// </summary>
		public const int ERROR_WIFI_SEND_PROC = 10010;

		/// <summary>WiFi通信受信処理異常
		/// </summary>
		public const int ERROR_WIFI_RECV_PROC = 10011;


		/// <summary>
		/// Cell,MU通信メッセージ送信完了イベント処理異常
		/// </summary>
		public const int ERROR_MUCOM_SENT_EVENT = 10100;
		/// <summary>
		/// Cell,MU通信メッセージ送信失敗イベント処理異常
		/// </summary>
		public const int ERROR_MUCOM_SENDFAILE_EVENT = 10101;
		/// <summary>
		/// Cell,MU通信メッセージ受信イベント処理異常
		/// </summary>
		public const int ERROR_MUCOM_RECV_EVENT = 10102;


		/// <summary>ロボット定期処理実行異常
		/// </summary>
		public const int ERROR_MU_EXEC = 20010;


		/// <summary>オーダー情報作成異常
		/// </summary>
		public const int ERROR_ORDER_MAKE_ORDERINFO = 30000;

		/// <summary>オーダー状態設定異常
		/// </summary>
		public const int ERROR_ORDER_SET_STATUS = 30001;

		/// <summary>オーダー実行異常
		/// </summary>
		public const int ERROR_ORDER_EXEC = 30002;

		/// <summary>走行情報作成異常
		/// </summary>
		public const int ERROR_ORDER_MAKE_ROUTEINFO = 30003;





		/// <summary>警告　ファイルなし
		/// </summary>
		public const int WARNNING_NOFILE = 302;
	}
}
