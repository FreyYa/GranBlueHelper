using Grandcypher;
using Livet;
using Livet.Messaging;
using GranBlueHelper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace GranBlueHelper.ViewModels
{
	public class SettingsViewModel : ViewModel
	{
		#region Fiddler On/Off 스위치
		private bool _StartFiddler;
		public bool StartFiddler
		{
			get { return this._StartFiddler; }
			set
			{
				if (this._StartFiddler != value)
				{
					this._StartFiddler = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private bool _StopFiddler;
		public bool StopFiddler
		{
			get { return this._StopFiddler; }
			set
			{
				if (this._StopFiddler != value)
				{
					this._StopFiddler = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public SettingsViewModel()
		{
			StartFiddler = false;
			StopFiddler = true;
		}
		public void SetScreenShotFolder()
		{
			string output;
			if (Settings.Current.ScreenShotFolder == "")
				output = "기본 사진 폴더";
			else output = Settings.Current.ScreenShotFolder;
			System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
			dialog.Description = "스크린샷을 저장할 폴더를 선택해주세요.\n현재폴더: " + output;
			dialog.ShowNewFolderButton = false;
			dialog.ShowDialog();
			string selected = dialog.SelectedPath;
			Settings.Current.ScreenShotFolder = selected;
		}
		public void Startup()
		{
			StartFiddler = false;
			StopFiddler = true;
			GrandcypherClient.Current.Proxy.StartUp(Settings.Current.portNum);
			GrandcypherClient.Current.PostMan("FiddlerCore 작동시작. Port: " + Settings.Current.portNum.ToString());
		}
		public void Shutdown()
		{
			StopFiddler = false;
			StartFiddler = true;
			GrandcypherClient.Current.Proxy.Quit();
			GrandcypherClient.Current.PostMan("FiddlerCore 작동중지");
		}
	}
}
