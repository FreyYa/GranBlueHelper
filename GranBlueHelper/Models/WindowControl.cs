using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.Models
{
	public class WindowControl
	{
		#region Win32
		[DllImportAttribute("user32.dll")]
		static extern bool SetCursorPos(int x, int y);

		[DllImportAttribute("user32.dll")]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern void SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_SHOWNORMAL = 1;

		public const int MOUSEEVENTF_LEFTDOWN = 0x02;
		public const int MOUSEEVENTF_LEFTUP = 0x04;
		#endregion

		#region singleton

		private static WindowControl current = new WindowControl();

		public static WindowControl Current
		{
			get { return current; }
		}

		#endregion
		public WindowControl()
		{

		}
		public void WindowForeground()
		{
			ShowWindow(WindowSizeSetter.Current.procHandler, SW_SHOWNORMAL);
			SetForegroundWindow(WindowSizeSetter.Current.procHandler);

		}

	}
}
