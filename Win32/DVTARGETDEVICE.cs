using System.Runtime.InteropServices;

namespace PokerPoker.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	// ReSharper disable once InconsistentNaming
	internal class DVTARGETDEVICE
	{
		public ushort tdSize;
		public uint tdDeviceNameOffset;
		public ushort tdDriverNameOffset;
		public ushort tdExtDevmodeOffset;
		public ushort tdPortNameOffset;
		public byte tdData;
	}
}
