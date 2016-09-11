using Grandcypher;
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
using System.Text;

using System.Runtime.InteropServices;

namespace GranBlueHelper.ViewModels
{
	public class MainWindowViewModel : WindowViewModel
	{
		#region isExecuting

		public bool isExecuting
		{
			get { return WindowControl.Current.isExecuting; }
			set
			{
				if (WindowControl.Current.isExecuting != value)
				{
					WindowControl.Current.isExecuting = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Libraries 変更通知プロパティ

		private IEnumerable<BindableTextViewModel> _Libraries;

		public IEnumerable<BindableTextViewModel> Libraries
		{
			get { return this._Libraries; }
			set
			{
				if (!Equals(this._Libraries, value))
				{
					this._Libraries = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region 포트번호

		private int _PortNumber;

		public int PortNumber
		{
			get { return this._PortNumber; }
			set
			{
				if (!Equals(this._PortNumber, value))
				{
					this._PortNumber = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region 프로그램 상태

		private string _AppStatus;

		public string AppStatus
		{
			get { return this._AppStatus; }
			set
			{
				if (!Equals(this._AppStatus, value))
				{
					this._AppStatus = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region 시나리오 리스트

		private List<Scenario> _ScenarioList;

		public List<Scenario> ScenarioList
		{
			get { return this._ScenarioList; }
			set
			{
				if (!Equals(this._ScenarioList, value))
				{
					this._ScenarioList = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region 시나리오 이름

		private string _ScenarioName;

		public string ScenarioName
		{
			get { return this._ScenarioName; }
			set
			{
				if (!Equals(this._ScenarioName, value))
				{
					this._ScenarioName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region 우측 간편 설정 Visibility

		private bool _Default;

		public bool Default
		{
			get { return this._Default; }
			set
			{
				if (!Equals(this._Default, value))
				{
					this._Default = value;
					if (this._HassanSettings || this._Default)
					{
						this.DefaultVisibility = Visibility.Visible;
						this.ScenarioVisibility = Visibility.Collapsed;
					}
					else
					{
						this.DefaultVisibility = Visibility.Collapsed;
						this.ScenarioVisibility = Visibility.Visible;
					}
					this.RaisePropertyChanged();
				}
			}
		}
		private bool _HassanSettings;

		public bool HassanSettings
		{
			get { return this._HassanSettings; }
			set
			{
				if (!Equals(this._HassanSettings, value))
				{
					this._HassanSettings = value;
					if (this._HassanSettings || this._Default)
					{
						this.DefaultVisibility = Visibility.Visible;
						this.ScenarioVisibility = Visibility.Collapsed;
					}
					else
					{
						this.DefaultVisibility = Visibility.Collapsed;
						this.ScenarioVisibility = Visibility.Visible;
					}
					this.RaisePropertyChanged();
				}
			}
		}
		private Visibility _DefaultVisibility;

		public Visibility DefaultVisibility
		{
			get { return this._DefaultVisibility; }
			set
			{
				if (!Equals(this._DefaultVisibility, value))
				{
					this._DefaultVisibility = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private Visibility _ScenarioVisibility;

		public Visibility ScenarioVisibility
		{
			get { return this._ScenarioVisibility; }
			set
			{
				if (!Equals(this._ScenarioVisibility, value))
				{
					this._ScenarioVisibility = value;
					this.RaisePropertyChanged();
				}
			}
		}


		#endregion

		#region 로딩화면

		private Visibility _LoadingScreen;
		public Visibility LoadingScreen
		{
			get { return this._LoadingScreen; }
			set
			{
				if (!Equals(this._LoadingScreen, value))
				{
					this._LoadingScreen = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private string _Progress;
		public string Progress
		{
			get { return this._Progress; }
			set
			{
				if (!Equals(this._Progress, value))
				{
					this._Progress = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 번역기 리스트
		public static Dictionary<string, TranslateKind> TranslatorListsTable = new Dictionary<string, TranslateKind>
		{
			{"구글", TranslateKind.Google}, {"네이버", TranslateKind.Naver}
		};
		public IEnumerable<string> TranslatorLists { get; private set; }

		private string _SelectedSite;

		public string SelectedSite
		{
			get { return this._SelectedSite; }
			set
			{
				if (!Equals(this._SelectedSite, value))
				{
					this._SelectedSite = value;

					if (value == "구글")
						Settings.Current.TranslatorSel = TranslateKind.Google;
					else if (value == "네이버")
						Settings.Current.TranslatorSel = TranslateKind.Naver;
					else Settings.Current.TranslatorSel = TranslateKind.Google;

					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public MainWindowViewModel()
		{
			StatusListener();
			this.TranslatorLists = TranslatorListsTable.Keys.ToList();
			SelectedSite = TranslatorListsTable.Keys.FirstOrDefault();

			this.LoadingScreen = Visibility.Collapsed;
			Read();
			this.AppStatus = "그랑블루 확장 찾기 대기중...";

			this.PortNumber = Settings.Current.portNum;
			this.Libraries = App.ProductInfo.Libraries.Aggregate(
				   new List<BindableTextViewModel>(),
				   (list, lib) =>
				   {
					   list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "" : ", " });
					   list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					   // プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					   return list;
				   });
			this.isExecuting = false;

			this.Title = "그랑블루 도우미 " + App.ProductInfo.VersionString;
		}
		private void Read()
		{
			GrandcypherClient.Current.ScenarioHooker.ScenarioStart += () =>
			{
				this.LoadingScreen = Visibility.Visible;
			};
			GrandcypherClient.Current.ScenarioHooker.TranslatieEnd += () =>
			{
				this.ScenarioList = new List<Scenario>(GrandcypherClient.Current.ScenarioHooker.ScenarioList);
				this.ScenarioName = GrandcypherClient.Current.ScenarioHooker.ScenarioName;
				this.LoadingScreen = Visibility.Collapsed;
				this.AppStatus = "시나리오 번역이 완료되었습니다";
			};
			GrandcypherClient.Current.ScenarioHooker.ProgressBar += () =>
			{
				string temp = GrandcypherClient.Current.ScenarioHooker.ProgressStatus.Max.ToString() + "개중 " +
					GrandcypherClient.Current.ScenarioHooker.ProgressStatus.Current.ToString() + "개 번역완료";
				this.Progress = temp;
				this.AppStatus = "시나리오 로딩중..." + GrandcypherClient.Current.ScenarioHooker.ProgressStatus.Max.ToString() + "개중 " + GrandcypherClient.Current.ScenarioHooker.ProgressStatus.Current.ToString() + "개 번역완료";
			};
		}
		public void ScreenShotButton()
		{
			WindowControl.Current.FindGranblue();
			WindowControl.Current.WindowForeground();
			Thread.Sleep(300);
			if (WindowControl.Current.isExecuting)
			{
				if (Settings.Current.ScreenShotFolder != null)
					WindowControl.Current.ScreenCaptureCore(Path.Combine(Settings.Current.ScreenShotFolder));
				else WindowControl.Current.ScreenCaptureCore(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
			}
			else
			{
				AppStatus = "ERROR : 실행되어있는 그랑블루 크롬 확장을 찾을 수 없습니다.";
			}
		}
		public void Activate()
		{
			//this.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate"));
			WindowControl.Current.FindGranblue();
			WindowControl.Current.WindowForeground();
		}
		private void StatusListener()
		{
			GrandcypherClient.Current.MessageSend += () =>
			{
				this.AppStatus = GrandcypherClient.Current.AppMessage;
			};
		}
	}
}