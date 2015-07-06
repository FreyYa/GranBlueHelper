using System.Runtime.InteropServices;

namespace PokerPoker.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	internal class RECT
	{
		public int left;
		public int top;
		public int width;
		public int height;
	}
}