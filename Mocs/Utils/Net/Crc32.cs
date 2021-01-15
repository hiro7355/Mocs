using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocs.Utilities.Net
{
	static class Crc32
	{
		/// <summary>
		/// 原始多項式 (x32+x26+x23+x22+x16+x12+x11+x10+x8+x7+x5+x4+x2+x+ 1)
		/// </summary>
		const UInt32 polynomial = 0xEDB88320;

		/// <summary>
		/// CRCを求める
		/// </summary>
		/// <param name="bytes">データ</param>
		/// <param name="length">データ長</param>
		/// <returns>CRC</returns>
		public static UInt32 GetCrc(UInt32 initValue, byte[] bytes, int length)
		{
			UInt32 crc_in = initValue;       // 初期値
			UInt32 crc;

			for (int i = 0; i < length; ++i)
			{
				crc = (crc_in ^ ((UInt32)bytes[i])) & 0xFF;
				for (byte j = 0; j < 8; ++j)
				{
					if ((crc & 0x01) != 0)
					{
						crc = (UInt32)((crc >> 1) ^ polynomial);
					}
					else
					{
						crc >>= 1;
					}
				}
				crc_in = ((crc_in >> 8) & 0x00FFFFFF) ^ crc;
			}

			return (UInt32)(crc_in ^ 0xFFFFFFFF);
		}

	}
}
