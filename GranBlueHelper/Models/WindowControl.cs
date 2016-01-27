using Grandcypher;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

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
		public static WindowControl Current { get; } = new WindowControl();
		#endregion

		public bool isExecuting { get; set; }
		public bool IsFind { get; set; }

		public void WindowForeground()
		{
			ShowWindow(WindowSizeSetter.Current.procHandler, SW_SHOWNORMAL);
			SetForegroundWindow(WindowSizeSetter.Current.procHandler);
		}
		public void ScreenCapture(string screenDirectory, int UserId = -1, string guid = "")
		{
			try
			{
				var savepoint = screenDirectory;
				var date = DateTime.Today.ToString("yyyyMMdd");
				var time = DateTime.Now.ToString("HHmmss");
				if (!Directory.Exists(savepoint)) savepoint = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				var filepath = Path.Combine(savepoint, date + "_" + time + ".png");

				if (UserId > 0)
				{
					string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
					savepoint = Path.Combine(MainFolder);
					filepath = Path.Combine(savepoint, UserId + "_" + guid + ".png");
				}

				int leftf = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.left * (WindowSizeSetter.Current.dpiX) / (96f));
				int topf = Convert.ToInt32(WindowSizeSetter.Current.WindowSize.top * (WindowSizeSetter.Current.dpiY) / (96f));
				int widthf = Convert.ToInt32((WindowSizeSetter.Current.WindowSize.right - WindowSizeSetter.Current.WindowSize.left) * (WindowSizeSetter.Current.dpiX) / (96f));
				int heightf = Convert.ToInt32((WindowSizeSetter.Current.WindowSize.bottom - WindowSizeSetter.Current.WindowSize.top) * (WindowSizeSetter.Current.dpiX) / (96f));

				Bitmap bitmap = new Bitmap(widthf, heightf);

				Graphics g = Graphics.FromImage(bitmap);

				g.CopyFromScreen(new System.Drawing.Point(leftf, topf), new System.Drawing.Point(0, 0), new System.Drawing.Size(widthf, heightf));

				bitmap.Save(filepath, ImageFormat.Png);
				if (UserId == -1) GrandcypherClient.Current.PostMan("저장성공: " + date + "_" + time + ".png");
			}
			catch (Exception ex)
			{
				GrandcypherClient.Current.PostMan("스크린샷 저장에 실패하였습니다.");
				Debug.WriteLine(ex);
			}

		}
		public void FindGranblue()
		{
			isExecuting = false;
			Process[] process = Process.GetProcesses();
			foreach (Process proc in process)
			{

				if (proc.ProcessName.Equals("chrome"))
				//  Pgm_FileName 프로그램의 실행 파일[.exe]를 제외한 파일명
				{
					if (proc.MainWindowTitle.Equals("グランブルーファンタジー[ChromeApps版]")
						|| proc.MainWindowTitle.Equals("グランブル?ファンタジ?[ChromeApps版]"))
					{
						isExecuting = true;
						IsFind = true;

						WindowSizeSetter.Current.SetWindowLocation(isExecuting);

						GrandcypherClient.Current.PostMan("그랑블루 확장 어플리케이션을 찾았습니다. 이제 그랑블루 확장을 종료할때까지 해당 프로세스를 기억합니다.");
						break;
					}
				}
				else
					isExecuting = false;
			}
			if (!isExecuting)
				GrandcypherClient.Current.PostMan("ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다.");
		}
		public void TakeScreenShot()
		{
			FindGranblue();
			WindowControl.Current.WindowForeground();
			Thread.Sleep(1000);
			if (WindowSizeSetter.Current.SetWindowLocation(isExecuting))
			{
				if (Settings.Current.ScreenShotFolder != null)
					WindowControl.Current.ScreenCapture(Path.Combine(Settings.Current.ScreenShotFolder));
				else WindowControl.Current.ScreenCapture(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
			}
			else
			{
				GrandcypherClient.Current.PostMan("ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다.");
			}
		}

	}
}
