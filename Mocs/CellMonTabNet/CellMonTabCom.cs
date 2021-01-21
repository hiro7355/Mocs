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
using Mocs.Utilities.Net;
using Mocs.Log;

namespace Mocs.CellMonTabNet
{
	public class CellMonTabCom
	{
		#region 固定値

		/// <summary>
		/// パケットヘッダー
		/// </summary>
		private const string c_PacketHeader = "STX";

		/// <summary>
		/// パケットフッター
		/// </summary>
		private const string c_PacketFooter = "ETX";

		/// <summary>
		/// パケットシーケンス番号サイズ
		/// </summary>
		private const int c_PacketSeqNoSize = 4;

		/// <summary>
		/// パケット長サイズ
		/// </summary>
		private const int c_PacketLenSize = 6;

		/// <summary>
		/// パケット長CRCサイズ
		/// </summary>
		private const int c_PacketLenCrcSize = 4;

		/// <summary>
		/// パケットボディCRCサイズ
		/// </summary>
		private const int c_PacketBodyCrcSize = 4;

		/// <summary>
		/// 送信データ一時保存バッファサイズ
		/// </summary>
		private const int c_SendDataBufSize = 1024;

		/// <summary>
		/// 受信データ一時保存バッファサイズ
		/// </summary>
		private const int c_RecvDataBufSize = 1024;


		#endregion

		#region 型定義

		/// <summary>
		/// 通信パケット構成ステータス
		/// </summary>
		private enum ePacketStatus
		{
			NONE = 0,
			INIT,
			SEARCH_HEADER,
			GET_HEADE_INFO,
			GET_BODY_CRC_FOOTER,
			PACKET_RECEIVED
		}

		#endregion

		#region メンバ変数

		/// <summary>
		/// UDPクライアント(送信)
		/// </summary>
		UdpClient _udpClientSend;

		/// <summary>
		/// UDPクライアント(受信)
		/// </summary>
		UdpClient _udpClientRecv;

		/// <summary>
		/// 接続フラグ
		/// </summary>
		private bool _IsConnected;

		/// <summary>
		/// IPアドレス
		/// </summary>
		private string _IpAddress;

		/// <summary>
		/// ポート番号
		/// </summary>
		private UInt16 _PortNo;

		/// <summary>
		/// 受信データ一時保存バッファ
		/// </summary>
		private Queue<byte> recvDataBuf = new Queue<byte>();

		/// <summary>
		/// 受信パケット長
		/// </summary>
		private int recvPacketLen;

		/// <summary>
		/// 通信パケット構成ステータス
		/// </summary>
		private ePacketStatus rcvPktStatus = ePacketStatus.NONE;

		/// <summary>
		/// 送信シーケンス番号
		/// </summary>
		private UInt16 _SendSeqNo;

		/// <summary>
		/// 受信シーケンス番号
		/// </summary>
		private UInt16 _RecvSeqNo;

		#endregion

		#region プロパティ

		/// <summary>
		/// IPアドレス
		/// </summary>
		public string IpAddress
		{
			get
			{
				return this._IpAddress;
			}
		}

		/// <summary>
		/// ポート番号
		/// </summary>
		public UInt16 PortNo
		{
			get
			{
				return this._PortNo;
			}
		}

		/// <summary>
		/// UDP通信接続確認
		/// </summary>
		public bool IsConnected
		{
			get { return this._IsConnected; }
		}

		/// <summary>
		/// 送信シーケンス番号
		/// </summary>
		public UInt16 SendSeqNo
		{
			get { return this._SendSeqNo; }
			set { this._SendSeqNo = value; }
		}

		/// <summary>
		/// 受信シーケンス番号
		/// </summary>
		public UInt16 RecvSeqNo
		{
			get { return this._RecvSeqNo; }
		}

		#endregion

		#region メソッド

		/// <summary>
		/// コンストラクター
		/// </summary>
		public CellMonTabCom()
		{
			try
			{
				this._udpClientSend = new UdpClient();
				this._udpClientRecv = new UdpClient();
				Initialize();
			}
			catch (Exception ex)
			{
				Console.WriteLine("CellMonTabCom Error:{0}", ex.Message);
			}
		}

		/// <summary>
		/// コンストラクター
		/// </summary>
		public CellMonTabCom(int sendPortNo, int recvPortNo)
		{
			try
			{
				Console.WriteLine("CellMonTabCom sendPortNo:{0}, recvPortNo:{1}", sendPortNo, recvPortNo);

				this._udpClientSend = new UdpClient(sendPortNo);
				this._udpClientRecv = new UdpClient(recvPortNo);

				Initialize();
			}
			catch (Exception ex)
			{
				Console.WriteLine("CellMonTabCom Error:{0}", ex.Message);
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <returns></returns>
		private bool Initialize()
		{
			bool ret = false;

			ret = true;

			return ret;
		}

		/// <summary>
		/// UDP接続
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="portNo"></param>
		/// <returns></returns>
		public bool Connect(string ipAddress, int portNo)
		{
			bool ret = false;

			try
			{
				if (this._IsConnected == false)
				{
					this._udpClientSend.Connect(ipAddress, portNo);

					this._IpAddress = ipAddress;
					this._PortNo = (UInt16)portNo;
					this._IsConnected = true;
					this._SendSeqNo = 0;
					SendLinkMessage();

					ret = true;
				}
			}
			catch (Exception ex)
			{
				MoCsLog.WriteLog(String.Format(@"UDP接続異常({0}/{1})"
												, ipAddress
												, portNo)
									+ ":" + ex.Message);
			}

			return ret;
		}

		/// <summary>
		/// UDP切断
		/// </summary>
		public void Disconnect()
		{
			//ポートクローズ
			try
			{
				this._udpClientSend.Close();
				this._udpClientRecv.Close();
				this._IsConnected = false;
			}
			catch (Exception ex)
			{
				MoCsLog.WriteLog(String.Format(@"UDP切断異常({0}/{1})"
												, this.IpAddress
												, this.PortNo)
									+ ":" + ex.Message);
			}
		}

		/// <summary>
		/// 受信データからパケットを取り出す
		/// </summary>
		/// <param name="recvData">受信データ</param>
		/// <param name="dataLen">受信データ長</param>
		/// <param name="recvPacket">受信パケット出力</param>
		/// <returns>パケット有無</returns>
		private bool GetRecvPacket(byte[] recvData, int dataLen, out string recvPacket)
		{
			bool ret = false;
			ePacketStatus preStatus;

			recvPacket = "";

			// 受信データを一時バッファへ保存
			for (int cnt = 0; cnt < dataLen; cnt++)
			{
				recvDataBuf.Enqueue(recvData[cnt]);
			}

			// パケット受信処理初期化
			if (this.rcvPktStatus == ePacketStatus.NONE
				|| this.rcvPktStatus == ePacketStatus.INIT)
			{
				this.recvPacketLen = 0;
				this.rcvPktStatus = ePacketStatus.SEARCH_HEADER;
			}

			do
			{
				preStatus = this.rcvPktStatus;
				switch (this.rcvPktStatus)
				{
					case ePacketStatus.SEARCH_HEADER:
						if (this.recvDataBuf.Count >= c_PacketHeader.Length)
						{
							string header = Encoding.UTF8.GetString(this.recvDataBuf.Take(c_PacketHeader.Length).ToArray());

							if (header != c_PacketHeader)
							{
								// ヘッダー不一致
								this.recvDataBuf.Dequeue();
							}
							else
							{
								// ヘッダー一致
								this.rcvPktStatus = ePacketStatus.GET_HEADE_INFO;
							}
						}
						break;

					case ePacketStatus.GET_HEADE_INFO:
						if (this.recvDataBuf.Count >= (c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize))
						{
							// シーケンス番号取得
							IEnumerable<byte> seqData = this.recvDataBuf.Skip(c_PacketHeader.Length).Take(c_PacketSeqNoSize);

							// パケット長取得
							IEnumerable<byte> lenData = this.recvDataBuf.Skip(c_PacketHeader.Length + c_PacketSeqNoSize).Take(c_PacketLenSize);

							// CRCチェック(ヘッダー、シーケンス番号、データ長)
							string strCrc = Encoding.UTF8.GetString(this.recvDataBuf.Skip(c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize).Take(c_PacketLenCrcSize).ToArray());
							UInt16 pktCrc = Convert.ToUInt16(strCrc, 16);
							IEnumerable<byte> crcData = this.recvDataBuf.Skip(c_PacketHeader.Length).Take(c_PacketSeqNoSize + c_PacketLenSize);
							UInt16 calCrc = Crc16.GetCrc(crcData.ToArray(), c_PacketSeqNoSize + c_PacketLenSize);
							if (pktCrc != calCrc)
							{
								// データ長CRC不一致
								this.recvDataBuf.Dequeue();
								// ヘッダー探索へ
								this.rcvPktStatus = ePacketStatus.SEARCH_HEADER;
							}
							else
							{
								// データ長CRC一致
								// 受信シーケンス番号更新
								this._RecvSeqNo = Convert.ToUInt16(Encoding.UTF8.GetString(seqData.ToArray()));
								Console.WriteLine("rseqno={0}", this._RecvSeqNo);
								// 受信パケット長更新
								this.recvPacketLen = Convert.ToInt32(Encoding.UTF8.GetString(lenData.ToArray()));
								// ボディ、CRC、フッター受信へ
								this.rcvPktStatus = ePacketStatus.GET_BODY_CRC_FOOTER;
							}
						}
						break;

					case ePacketStatus.GET_BODY_CRC_FOOTER:
						if (this.recvDataBuf.Count >= (c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize + this.recvPacketLen + c_PacketBodyCrcSize + c_PacketFooter.Length))
						{
							string footer = Encoding.UTF8.GetString(this.recvDataBuf.Skip(c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize + this.recvPacketLen + c_PacketBodyCrcSize).Take(c_PacketFooter.Length).ToArray());

							if (footer != c_PacketFooter)
							{
								// フッター不一致
								this.recvDataBuf.Dequeue();
								// ヘッダー探索
								this.rcvPktStatus = ePacketStatus.SEARCH_HEADER;
							}
							else
							{
								// フッター一致
								// CRCチェック
								// ボディ取得
								IEnumerable<byte> bodyData = this.recvDataBuf.Skip(c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize).Take(this.recvPacketLen);
								// CRCチェック
								string strCrc = Encoding.UTF8.GetString(this.recvDataBuf.Skip(c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize + this.recvPacketLen).Take(c_PacketBodyCrcSize).ToArray());
								UInt16 pktCrc = Convert.ToUInt16(strCrc, 16);
								UInt16 calCrc = Crc16.GetCrc(bodyData.ToArray(), this.recvPacketLen);
								if (pktCrc != calCrc)
								{
									// ボディCRC不一致
									this.recvDataBuf.Dequeue();
									// ヘッダー探索
									this.rcvPktStatus = ePacketStatus.SEARCH_HEADER;
								}
								else
								{
									// ボディCRC一致
									this.rcvPktStatus = ePacketStatus.PACKET_RECEIVED;
								}
							}
						}
						break;

					case ePacketStatus.PACKET_RECEIVED:
						// 受信パケット保存
						int pktLen = c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize + this.recvPacketLen + c_PacketBodyCrcSize + c_PacketFooter.Length;
						recvPacket = Encoding.UTF8.GetString(this.recvDataBuf.Take(pktLen).ToArray());

						// パケット分の受信データ取り出し
						for (int cnt = 0; cnt < pktLen; cnt++)
						{
							this.recvDataBuf.Dequeue();
						}
						this.rcvPktStatus = ePacketStatus.SEARCH_HEADER;

						ret = true;
						break;

					default:
						break;
				}
			} while (((preStatus != this.rcvPktStatus)
						|| (this.rcvPktStatus == ePacketStatus.SEARCH_HEADER && this.recvDataBuf.Count >= c_PacketHeader.Length))
					&& ret == false);

			return ret;
		}

		/// <summary>
		/// メッセージ受信
		/// </summary>
		/// <param name="packet">パケット</param>
		/// <returns>ボディ部</returns>
		public bool RecvMessage(out string recvPacket)
		{
			bool ret = false;
			int res = -1;
			recvPacket = "";

			try
			{
				// 受信
				if (this._udpClientRecv != null && this._IsConnected == true)
				{
					string packet;
					byte[] data = null;
					bool bReceived = false;

					do
					{
						// UDPデータ受信
						if (this.recvDataBuf.Count == 0
							|| this.rcvPktStatus != ePacketStatus.SEARCH_HEADER
							|| (this.rcvPktStatus == ePacketStatus.SEARCH_HEADER && this.recvDataBuf.Count < c_PacketHeader.Length))
						{
							IPEndPoint rtIpEndPoint = null;
							data = this._udpClientRecv.Receive(ref rtIpEndPoint);
						}

						// 受信データからパケット取得
						if (data != null && data.Count() > 0
							&& true == GetRecvPacket(data, data.Count(), out packet))
						{
							// 受信パケットからボディ部を抜き出す
							int headLen = c_PacketHeader.Length + c_PacketSeqNoSize + c_PacketLenSize + c_PacketLenCrcSize;
							int bodyLen = packet.Length - headLen - c_PacketBodyCrcSize - c_PacketFooter.Length;

							if (bodyLen == 0)
							{
								UInt32 serNo = Convert.ToUInt32(packet.Substring(c_PacketHeader.Length, c_PacketSeqNoSize));
								if (serNo == 0)
								{
									this._RecvSeqNo = 0;
								}
							}
							else
							{
								recvPacket = packet.Substring(headLen, bodyLen);

								ret = true;

								bReceived = true;
							}
						}
					} while (!bReceived);
				}
			}
			catch (Exception ex)
			{
				MoCsLog.WriteLog(String.Format(@"UDP受信異常({0}/{1})"
												, this.IpAddress
												, this.PortNo)
									+ ":" + ex.Message);
				throw ex;
			}

			return ret;
		}

		/// <summary>
		/// リンクメッセージ送信
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool SendLinkMessage()
		{
			return SendMessage("", 0);
		}

		/// <summary>
		/// メッセージ送信
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool SendMessage(string message)
		{
			return SendMessage(message, this._SendSeqNo);
		}

		/// <summary>
		/// メッセージ応答送信
		/// </summary>
		/// <param name="message"></param>
		/// <param name="seqNo"></param>
		/// <returns></returns>
		public bool SendMessageRes(string message, UInt16 seqNo)
		{
			return SendMessage(message, seqNo);
		}

		/// <summary>
		/// メッセージ送信
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool SendMessage(string message, UInt16 seqNo)
		{
			bool ret = false;
			string packet;

			// シーケンス番号作成
			string seqFormat = String.Format("D{0}", c_PacketSeqNoSize);
			string strSeqData = seqNo.ToString(seqFormat);

			// ボディ作成
			byte[] msgData = Encoding.UTF8.GetBytes(message);

			// パケット長作成
			string lenFormat = String.Format("D{0}", c_PacketLenSize);

			// パケット長CRC作成
			int length = msgData.Length;
			string strLenCrc;
			string strLenData = length.ToString(lenFormat);
			byte[] crcData = Encoding.UTF8.GetBytes(strSeqData + strLenData);
			strLenCrc = Crc16.GetCrc(crcData, crcData.Length).ToString("X4");

			// ボディCRC作成
			string strBodyCrc;
			strBodyCrc = Crc16.GetCrc(msgData, msgData.Length).ToString("X4");

			// パケット作成
			packet = c_PacketHeader + strSeqData + strLenData + strLenCrc + message + strBodyCrc + c_PacketFooter;

			// 送信
			try
			{
				if (this._udpClientSend != null && this._IsConnected == true)
				{
					byte[] data = Encoding.UTF8.GetBytes(packet);
					this._udpClientSend.Send(data, data.Length);

					ret = true;
				}
			}
			catch (Exception ex)
			{
				MoCsLog.WriteLog(String.Format(@"UDP送信異常({0}/{1})"
												, this.IpAddress
												, this.PortNo)
									+ ":" + ex.Message);
				throw ex;
			}

			return ret;
		}

		#endregion
	}
}
