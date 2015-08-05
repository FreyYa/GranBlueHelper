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

			GrandcypherClient.Current.Proxy.StartUp(Settings.Current.portNum);
			GrandcypherClient.Current.WeaponHooker.MasterInfoListLoad();
			ViewModelRoot = new MainWindowViewModel();

			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };

			WindowSizeSetter.Current.WindowSize = new WindowSize();
			this.MainWindow.Show();
#if !DEBUG
			KListener = new KeyboardListener();

			KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
#endif
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
			{
				float dpiX = graphics.DpiX;
				float dpiY = graphics.DpiY;
#if DEBUG
				Console.WriteLine(dpiX.ToString() + " " + dpiY.ToString());
#endif
				WindowSizeSetter.Current.dpiX = dpiX;
				WindowSizeSetter.Current.dpiY = dpiY;
			}
		}
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
#if !DEBUG
			KListener.Dispose();
#endif
			GrandcypherClient.Current.Proxy.Quit();

			Settings.Current.Save();
		}
		void KListener_KeyDown(object sender, RawKeyEventArgs args)
		{
			if (args.Key.ToString() == "F10")
			{
				GrandcypherClient.Current.GlobalKeyCore.TakeScreenShot();
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
