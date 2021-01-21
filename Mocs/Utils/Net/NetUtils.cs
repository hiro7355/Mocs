using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Mocs.Utilities.Net
{
	class NetUtils
	{

		[
			System.Runtime.InteropServices.StructLayout
			(
				System.Runtime.InteropServices.LayoutKind.Explicit
			)
		]
		unsafe struct TcpKeepAlive
		{
			[System.Runtime.InteropServices.FieldOffset(0)]
			[
				System.Runtime.InteropServices.MarshalAs
				(
					System.Runtime.InteropServices.UnmanagedType.ByValArray,
					SizeConst = 12
				)
			]
			public fixed byte Bytes[12];

			[System.Runtime.InteropServices.FieldOffset(0)]
			public uint On_Off;

			[System.Runtime.InteropServices.FieldOffset(4)]
			public uint KeepaLiveTime;

			[System.Runtime.InteropServices.FieldOffset(8)]
			public uint KeepaLiveInterval;
		}

		public static int SetKeepAliveValues(
					System.Net.Sockets.Socket Socket,
					bool On_Off,
					uint KeepaLiveTime,
					uint KeepaLiveInterval)
		{
			int Result = -1;

			unsafe
			{
				TcpKeepAlive KeepAliveValues = new TcpKeepAlive();

				KeepAliveValues.On_Off = Convert.ToUInt32(On_Off);
				KeepAliveValues.KeepaLiveTime = KeepaLiveTime;
				KeepAliveValues.KeepaLiveInterval = KeepaLiveInterval;

				byte[] InValue = new byte[12];

				for (int I = 0; I < 12; I++)
					InValue[I] = KeepAliveValues.Bytes[I];

				Result = Socket.IOControl(IOControlCode.KeepAliveValues, InValue, null);
			}

			return Result;
		}

	}
}
