using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GranBlueHelper.Models.Data.Xml;
using Livet;
using System.Windows;
using Grandcypher;

namespace GranBlueHelper.Models
{
	[Serializable]
	public class Settings : NotificationObject
	{
		#region static members

		private static readonly string filePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"KcvKr.tistory.com",
			"GrandcypherGear",
			"Settings.xml");
		private static readonly string CurrentSettingsVersion = "1.0";
		public static Settings Current { get; set; }
		public static void Load()
		{
			try
			{
				Current = filePath.ReadXml<Settings>();
				if (Current.SettingsVersion != CurrentSettingsVersion)
					Current = GetInitialSettings();
			}
			catch (Exception ex)
			{
				Current = GetInitialSettings();
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		public static Settings GetInitialSettings()
		{
			return new Settings
			{
				SettingsVersion = CurrentSettingsVersion,
				portNum = 32433,
				ResetPortConfig = false,
				TranslatorSel = TranslateKind.Google,
				ScreenShotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
			};
		}

		#endregion

		#region SettingsVersion

		private string _SettingsVersion;

		public string SettingsVersion
		{
			get { return this._SettingsVersion; }
			set
			{
				if (this._SettingsVersion != value)
				{
					this._SettingsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 포트 설정

		private int _PortNum;

		public int portNum
		{
			get { return this._PortNum; }
			set
			{
				if (this._PortNum != value)
				{
					this._PortNum = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private bool _ResetPortConfig;

		public bool ResetPortConfig
		{
			get { return this._ResetPortConfig; }
			set
			{
				if (this._ResetPortConfig != value)
				{
					this._ResetPortConfig = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 스크린샷 폴더 저장

		private string _ScreenShotFolder;

		public string ScreenShotFolder
		{
			get { return this._ScreenShotFolder; }
			set
			{
				if (this._ScreenShotFolder != value)
				{
					this._ScreenShotFolder = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 번역기 종류 선택

		private TranslateKind _TranslatorSel;

		public TranslateKind TranslatorSel
		{
			get { return this._TranslatorSel; }
			set
			{
				if (this._TranslatorSel != value)
				{
					this._TranslatorSel = value;
					GrandcypherClient.Current.ScenarioHooker.TranslateSite = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region 바하무트 위스 스킬레벨
		private int _visLv;
		public int visLv
		{
			get { return this._visLv; }
			set
			{
				if (this._visLv == value) return;
				this._visLv = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region 바하무트 콘킬리오 스킬레벨
		private int _concilioLv;
		public int concilioLv
		{
			get { return this._concilioLv; }
			set
			{
				if (this._concilioLv == value) return;
				this._concilioLv = value;

				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region 십천중 계산 설정


		private bool _Proto;
		public bool Proto
		{
			get { return this._Proto; }
			set
			{
				if (this._Proto == value) return;
				this._Proto = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Rust;
		public bool Rust
		{
			get { return this._Rust; }
			set
			{
				if (this._Rust == value) return;
				this._Rust = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Element;
		public bool Element
		{
			get { return this._Element; }
			set
			{
				if (this._Element == value) return;
				this._Element = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _First;
		public bool First
		{
			get { return this._First; }
			set
			{
				if (this._First == value) return;
				this._First = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Second;
		public bool Second
		{
			get { return this._Second; }
			set
			{
				if (this._Second == value) return;
				this._Second = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Third;
		public bool Third
		{
			get { return this._Third; }
			set
			{
				if (this._Third == value) return;
				this._Third = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Fourth;
		public bool Fourth
		{
			get { return this._Fourth; }
			set
			{
				if (this._Fourth == value) return;
				this._Fourth = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Fifth;
		public bool Fifth
		{
			get { return this._Fifth; }
			set
			{
				if (this._Fifth == value) return;
				this._Fifth = value;

				this.RaisePropertyChanged();
			}
		}

		#endregion

		public void Save()
		{
			try
			{
				this.WriteXml(filePath);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
