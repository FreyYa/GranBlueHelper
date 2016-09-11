using Grandcypher;
using Livet;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace GranBlueHelper.Models
{
	public class WindowSizeSetter : NotificationObject
	{
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

		#region public 변수
		public float dpiX { get; set; }
		public float dpiY { get; set; }
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

		#region SystemWindowSize

		private WindowSize _SystemWindowSize;

		public WindowSize SystemWindowSize
		{
			get { return this._SystemWindowSize; }
			set
			{
				if (this._SystemWindowSize != value)
				{
					this._SystemWindowSize = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		#endregion
		private WindowSizeSetter()
		{
			this.WindowSize = new WindowSize();
			this.SystemWindowSize = new WindowSize();
		}
		/// <summary>
		/// 윈도우의 위치를 찾아낸다. DPI에 따른 계산도 추가로 적용
		/// </summary>
		/// <param name="isExecuting">그랑블루 확장을 찾았는가에 대한 변수</param>
		/// <returns></returns>
		public void SetWindowLocation(IntPtr handler)
		{
			//IntPtr procHandler = FindWindow(null, "グランブルーファンタジー[ChromeApps版]");
			//if(procHandler.ToInt32()==0) procHandler = FindWindow(null, "グランブル?ファンタジ?[ChromeApps版]");

			if (handler.ToInt32() != 0)
			{
				this.procHandler = handler;
				WindowControl.Current.WindowForeground();
				stRect = default(RECT);
				GetWindowRect(procHandler.ToInt32(), ref stRect);

				this.SystemWindowSize.bottom = stRect.bottom;
				this.SystemWindowSize.left = stRect.left;
				this.SystemWindowSize.top = stRect.top;
				this.SystemWindowSize.right = stRect.right;


				this.WindowSize.left = Convert.ToInt32(this.SystemWindowSize.left / (dpiX) * (96f));
				this.WindowSize.right = Convert.ToInt32(this.SystemWindowSize.right / (dpiX) * (96f));
				this.WindowSize.top = Convert.ToInt32(this.SystemWindowSize.top / (dpiY) * (96f));
				this.WindowSize.bottom = Convert.ToInt32(this.SystemWindowSize.bottom / (dpiY) * (96f));
#if DEBUG
				Debug.WriteLine("left: " + this.WindowSize.left + "right: " + this.WindowSize.right + "top: " + this.WindowSize.top + "bottom: " + this.WindowSize.bottom);
#endif
			}
			else
			{
				GrandcypherClient.Current.PostMan("실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다. 필독 문서 참조");
			}
		}

	}
}
