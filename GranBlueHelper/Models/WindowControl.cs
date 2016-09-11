using Grandcypher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using System.Windows;

namespace GranBlueHelper.Models
{
	public class WindowControl : NotificationObject
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
		public static WindowControl Current { get; } = new WindowControl();
		#endregion

		#region isExecuting
		private bool _isExecuting;
		/// <summary>
		/// 그랑블루 확장 상태
		/// </summary>
		public bool isExecuting
		{
			get { return this._isExecuting; }
			set
			{
				if (this._isExecuting != value)
				{
					this._isExecuting = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		void WIndowControl()
		{
			this.isExecuting = false;
		}

		#region 윈도우관련
		/// <summary>
		/// 그랑블루 판타지 어플리케이션을 화면 맨 위로 올린후 활성화한다.
		/// </summary>
		public void WindowForeground()
		{
			ShowWindow(WindowSizeSetter.Current.procHandler, SW_SHOWNORMAL);
			SetForegroundWindow(WindowSizeSetter.Current.procHandler);
		}
		/// <summary>
		/// 그랑블루 판타지의 프로세스를 찾아낸다. 없으면 오류 메시지 송출
		/// </summary>
		public void FindGranblue()
		{
			isExecuting = false;
			while (true)
			{
				Process[] process = Process.GetProcesses();
				List<Process> list = new List<Process>();
				foreach (Process proc in process)
				{

					if (proc.MainWindowTitle.Equals("グランブルーファンタジー[ChromeApps版]")
						|| proc.MainWindowTitle.Equals("グランブル?ファンタジ?[ChromeApps版]"))
					{
						isExecuting = true;

						WindowSizeSetter.Current.SetWindowLocation(proc.MainWindowHandle);

						GrandcypherClient.Current.PostMan("그랑블루 확장 어플리케이션을 찾았습니다. 이제 그랑블루 확장을 종료할때까지 해당 프로세스를 기억합니다.");

						return;
					}
					else if (proc.MainWindowTitle.Contains("グランブル"))
					{
						if (proc.MainWindowTitle.Contains("ファンタジ"))
							list.Add(proc);
					}
					else if (proc.MainWindowTitle.Contains("Granblue Fantasy"))
					{
						list.Add(proc);
					}
					else
					{
						isExecuting = false;
					}
				}

				if (list.Count > 1)
				{
					var result = MessageBox.Show("[グランブル]와 [ファンタジ]가 모두 포함된 창, 브라우저 탭, 혹은 프로세스가 두개 이상 존재합니다. \n대상이 될 하나의 프로세스를 제외한 나머지 프로세스를 종료하신후 확인을 눌러주시기 바랍니다.", "프로세스 선별", MessageBoxButton.OKCancel);
					if (result == MessageBoxResult.Cancel)
					{
						isExecuting = false;
					}
				}
				else if (list.Count == 1)
				{
					isExecuting = true;

					WindowSizeSetter.Current.SetWindowLocation(list[0].MainWindowHandle);

					GrandcypherClient.Current.PostMan("그랑블루 확장 어플리케이션을 찾았습니다. 이제 그랑블루 확장을 종료할때까지 해당 프로세스를 기억합니다.");
					break;
				}
				else
				{
					isExecuting = false;
					break;
				}
			}

			if (!isExecuting)
				GrandcypherClient.Current.PostMan("ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다. 필독 문서 참조");
		}
		#endregion

		#region 스크린샷
		/// <summary>
		/// 스크린샷을 촬영한다
		/// </summary>
		public void TakeScreenShot()
		{
			FindGranblue();
			WindowForeground();
			Thread.Sleep(500);
			if (WindowSizeSetter.Current.procHandler.ToInt32() != 0)
			{
				if (Settings.Current.ScreenShotFolder != null)
					ScreenCaptureCore(Path.Combine(Settings.Current.ScreenShotFolder));
				else ScreenCaptureCore(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
			}
			else
			{
				GrandcypherClient.Current.PostMan("ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다. 필독 문서 참조");
			}
		}
		/// <summary>
		/// 스크린샷 촬영 코어
		/// </summary>
		/// <param name="screenDirectory"></param>
		/// <param name="UserId"></param>
		/// <param name="guid"></param>
		public void ScreenCaptureCore(string screenDirectory)
		{
			try
			{
				var savepoint = screenDirectory;
				var date = DateTime.Today.ToString("yyyyMMdd");
				var time = DateTime.Now.ToString("HHmmss");
				if (!Directory.Exists(savepoint)) savepoint = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				var filepath = Path.Combine(savepoint, date + "_" + time + ".png");

				int leftf = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.left * (WindowSizeSetter.Current.dpiX) / (96f));
				int topf = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.top * (WindowSizeSetter.Current.dpiY) / (96f));
				int widthf = Convert.ToInt32((WindowSizeSetter.Current.WindowSize.right - WindowSizeSetter.Current.WindowSize.left) * (WindowSizeSetter.Current.dpiX) / (96f));
				int heightf = Convert.ToInt32((WindowSizeSetter.Current.WindowSize.bottom - WindowSizeSetter.Current.WindowSize.top) * (WindowSizeSetter.Current.dpiX) / (96f));

				Bitmap bitmap = new Bitmap(widthf, heightf);

				Graphics g = Graphics.FromImage(bitmap);

				g.CopyFromScreen(new System.Drawing.Point(leftf, topf), new System.Drawing.Point(0, 0), new System.Drawing.Size(widthf, heightf));

				bitmap.Save(filepath, ImageFormat.Png);
				bitmap.Dispose();
				g.Dispose();
			}
			catch (Exception ex)
			{
				GrandcypherClient.Current.PostMan("스크린샷 저장에 실패하였습니다.");
				Debug.WriteLine(ex);
			}

		}

		/// <summary>
		/// 스크린샷 촬영(하드에 저장하지 않음)
		/// </summary>
		/// <param name="screenDirectory"></param>
		/// <param name="UserId"></param>
		/// <param name="guid"></param>
		public Bitmap ScreenCaptureOut()
		{
			try
			{
				int leftf = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.left * (WindowSizeSetter.Current.dpiX) / (96f));
				int topf = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.top * (WindowSizeSetter.Current.dpiY) / (96f));
				int widthf = Convert.ToInt32((WindowSizeSetter.Current.WindowSize.right - WindowSizeSetter.Current.WindowSize.left) * (WindowSizeSetter.Current.dpiX) / (96f));
				int heightf = Convert.ToInt32((WindowSizeSetter.Current.WindowSize.bottom - WindowSizeSetter.Current.WindowSize.top) * (WindowSizeSetter.Current.dpiX) / (96f));

				Bitmap bitmap = new Bitmap(widthf, heightf);

				Graphics g = Graphics.FromImage(bitmap);

				g.CopyFromScreen(new System.Drawing.Point(leftf, topf), new System.Drawing.Point(0, 0), new System.Drawing.Size(widthf, heightf));
				return bitmap;
			}
			catch (Exception ex)
			{
				GrandcypherClient.Current.PostMan("스크린샷 촬영에 실패하였습니다.");
				Debug.WriteLine(ex);
				return null;
			}

		}

		#endregion
	}
}
