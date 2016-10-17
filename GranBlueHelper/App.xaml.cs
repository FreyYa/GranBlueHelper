using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Livet;
using System.Net;
using GranBlueHelper.ViewModels;
using GranBlueHelper.Views;
using AppSettings = GranBlueHelper.Properties.Settings;
using Settings = GranBlueHelper.Models.Settings;
using Grandcypher;
using GranBlueHelper.Models;
using Grandcypher.Models;
using System.Drawing;
using GranBlueHelper.Models.Notifier;

namespace GranBlueHelper
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		public KeyboardListener KListener { get; set; }
		public static MainWindowViewModel ViewModelRoot { get; private set; }
		public static ProductInfo ProductInfo { get; private set; }
		static App()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception);

			Settings.Load();
			DispatcherHelper.UIDispatcher = this.Dispatcher;
			ProductInfo = new ProductInfo();

			MainNotifier.Current.Initialize();
			GrandcypherClient.Current.Proxy.StartUp(Settings.Current.portNum);
			//GrandcypherClient.Current.WeaponHooker.MasterInfoListLoad();

			EventCore.Current.Start();
			if (GrandcypherClient.Current.Updater.LoadVersion(AppSettings.Default.XMLUpdateUrl.AbsoluteUri))
			{
				if (GrandcypherClient.Current.Updater.IsOnlineVersionGreater(0, ProductInfo.Version.ToString()))
				{
					Version temp = new Version(GrandcypherClient.Current.Updater.GetOnlineVersion(TranslationType.App));
					MainNotifier.Current.Show("업데이트 알림", "상위버전의 어플리케이션이 존재합니다. \n" + temp.Major + "." + temp.Minor + "." + temp.Build + " Rev." + temp.Revision + " Ver", () => App.ViewModelRoot.Activate());
				}
			}

			if (GrandcypherClient.Current.Updater.UpdateTranslations(AppSettings.Default.XMLTransUrl.AbsoluteUri, GrandcypherClient.Current.Translations) > 0)
			{
				MainNotifier.Current.Show("업데이트 알림", "데이터를 성공적으로 업데이트하였습니다.", () => App.ViewModelRoot.Activate());
			}
			ViewModelRoot = new MainWindowViewModel();

			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };

			WindowSizeSetter.Current.WindowSize = new WindowSize();

			this.MainWindow.Show();
			#region .Do(debug)
#if !DEBUG
			KListener = new KeyboardListener();

			KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
#endif
			#endregion
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
			{
				float dpiX = graphics.DpiX;
				float dpiY = graphics.DpiY;
				#region .Do(debug)
#if DEBUG
				Console.WriteLine(dpiX.ToString() + " " + dpiY.ToString());
#endif
				#endregion
				WindowSizeSetter.Current.dpiX = dpiX;
				WindowSizeSetter.Current.dpiY = dpiY;
			}
		}
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			MainNotifier.Current.Dispose();
			#region .Do(debug)
#if !DEBUG
			KListener.Dispose();
#endif
			#endregion
			GrandcypherClient.Current.Proxy.Quit();

			Settings.Current.Save();
		}
		void KListener_KeyDown(object sender, RawKeyEventArgs args)
		{
			if (args.Key.ToString() == "F10")
			{
				WindowControl.Current.TakeScreenShot();
			}
			#region .Do(debug)
#if DEBUG
			Console.WriteLine(args.Key.ToString());
#endif
			#endregion
		}

		private static void ReportException(object sender, Exception exception)
		{
			#region const
			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			const string path = "error.log";
			#endregion

			try
			{
				var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
