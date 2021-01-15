using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Utilities.Net
{
	class Crc16
	{
		/// <summary>
		/// 原始多項式 (x16+x15+x2+1)
		/// </summary>
		const ushort polynomial = 0xA001;

		/// <summary>
		/// CRCを求める
		/// </summary>
		/// <param name="bytes">データ</param>
		/// <param name="length">データ長</param>
		/// <returns>CRC</returns>
		public static UInt16 GetCrc(byte[] bytes, int length)
		{
			int crc = 0xffff;		// 初期値

			crc = ~crc;
			for (ushort i = 0; i < length; ++i)
			{
				crc ^= bytes[i];
				for (byte j = 0; j < 8; ++j)
				{
					if ((crc & 0x0001) != 0)
					{
						crc = (ushort)((crc >> 1) ^ polynomial);
					}
					else
					{
						crc >>= 1;
					}
					crc >>= 1;
				}
			}

			return (UInt16)(~crc);
		}
	}
}
