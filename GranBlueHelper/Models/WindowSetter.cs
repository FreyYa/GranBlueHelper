using Grandcypher;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace GranBlueHelper.Models
{
	public class WindowSizeSetter : NotificationObject
	{
		public IntPtr procHandler { get; set; }

		#region WindowSize

		private WindowSize _WindowSize;

		public WindowSize WindowSize
		{
			get { return this._WindowSize; }
			set
			{
				if (this._WindowSize != value)
				{
					this._WindowSize = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region singleton

		private static WindowSizeSetter current = new WindowSizeSetter();

		public static WindowSizeSetter Current
		{
			get { return current; }
		}

		#endregion

		#region Win32
		private RECT stRect;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string strClassName, string StrWindowName);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowRect(int hwnd, ref RECT lpRect);
		#endregion
		public float dpiX { get; set; }
		public float dpiY { get; set; }
		private WindowSizeSetter()
		{
			this.WindowSize = new WindowSize();
		}
		public bool SetWindowLocation(bool isExecuting)
		{
			if (isExecuting)
			{
				IntPtr procHandler = FindWindow(null, "グランブルーファンタジー[ChromeApps版]");
				if (procHandler.ToInt32() != 0)
				{
					WindowSizeSetter.Current.procHandler = procHandler;
					WindowControl.Current.WindowForeground();
					stRect = default(RECT);
					GetWindowRect(procHandler.ToInt32(), ref stRect);

					WindowSizeSetter.Current.WindowSize.bottom = stRect.bottom;
					WindowSizeSetter.Current.WindowSize.left = stRect.left;
					WindowSizeSetter.Current.WindowSize.top = stRect.top;
					WindowSizeSetter.Current.WindowSize.right = stRect.right;


					WindowSizeSetter.Current.WindowSize.left = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.left / (dpiX) * (96f));
					WindowSizeSetter.Current.WindowSize.right = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.right / (dpiX) * (96f));
					WindowSizeSetter.Current.WindowSize.top = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.top / (dpiY) * (96f));
					WindowSizeSetter.Current.WindowSize.bottom = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.bottom / (dpiY) * (96f));

					return true;
				}
				else
				{
					GrandcypherClient.Current.PostMan("ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다.");
					return false;
				}
			}
			else if (!isExecuting)
			{
				GrandcypherClient.Current.PostMan("ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다.");
				return false;
			}
			return false;
		}
		public Point Posparser(string PosInfo)
		{
			Point xytemp = new Point();
			var temp = PosInfo.Split('_');
			xytemp.X = Convert.ToInt32(temp[0]);
			xytemp.Y = Convert.ToInt32(temp[1]);
			xytemp.X = Convert.ToInt32(xytemp.X);
			xytemp.Y = Convert.ToInt32(xytemp.Y);

			return xytemp;
		}
	}
}
