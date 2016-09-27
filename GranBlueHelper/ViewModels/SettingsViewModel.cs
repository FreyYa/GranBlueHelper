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
		public void SetScreenShotFolder()
		{
			string output;
			if (Settings.Current.ScreenShotFolder != string.Empty)
				output = Settings.Current.ScreenShotFolder;
			else
				output = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
			dialog.Description = "스크린샷을 저장할 폴더를 선택해주세요.\n현재폴더: " + output;
			dialog.ShowNewFolderButton = true;
			dialog.SelectedPath = Settings.Current.ScreenShotFolder;
			dialog.ShowDialog();
			string selected = dialog.SelectedPath;
			Settings.Current.ScreenShotFolder = selected;
		}
	}
}
