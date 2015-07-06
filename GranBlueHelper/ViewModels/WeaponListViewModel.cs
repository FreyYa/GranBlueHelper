using Grandcypher;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GranBlueHelper.ViewModels
{
	public class WeaponListViewModel : ViewModel
	{
		#region 무기 리스트

		private List<WeaponInfo> _WeaponList;

		public List<WeaponInfo> WeaponList
		{
			get { return this._WeaponList; }
			set
			{
				if (!Equals(this._WeaponList, value))
				{
					this._WeaponList = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private bool _IsBaha;
		public bool IsBaha
		{
			get { return this._IsBaha; }
			set
			{
				if (!Equals(this._IsBaha, value))
				{
					this._IsBaha = value;
					CalcAttack(this.SkillCounter, value);
					this.RaisePropertyChanged();
				}
			}
		}
		private string _Gap;

		public string Gap
		{
			get { return this._Gap; }
			set
			{
				if (!Equals(this._Gap, value))
				{
					this._Gap = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private WeaponInfo _MainWeapon;

		public WeaponInfo MainWeapon
		{
			get { return this._MainWeapon; }
			set
			{
				if (!Equals(this._MainWeapon, value))
				{
					this._MainWeapon = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private Visibility _ListWeapon;
		public Visibility ListWeapon
		{
			get { return this._ListWeapon; }
			set
			{
				if (!Equals(this._ListWeapon, value))
				{
					this._ListWeapon = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private int _CalcAtt;
		public int CalcAtt
		{
			get { return this._CalcAtt; }
			set
			{
				if (!Equals(this._CalcAtt, value))
				{
					this._CalcAtt = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private string _CalcAttDetail;
		public string CalcAttDetail
		{
			get { return this._CalcAttDetail; }
			set
			{
				if (!Equals(this._CalcAttDetail, value))
				{
					this._CalcAttDetail = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private int _BeforeCalcAtt;
		public int BeforeCalcAtt
		{
			get { return this._BeforeCalcAtt; }
			set
			{
				if (!Equals(this._BeforeCalcAtt, value))
				{
					this._BeforeCalcAtt = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private Visibility _DeckWeapon;
		public Visibility DeckWeapon
		{
			get { return this._DeckWeapon; }
			set
			{
				if (!Equals(this._DeckWeapon, value))
				{
					this._DeckWeapon = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private List<string> _SkillList;
		public List<string> SkillList
		{
			get { return this._SkillList; }
			set
			{
				if (!Equals(this._SkillList, value))
				{
					this._SkillList = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private Skills _SkillCounter;
		public Skills SkillCounter
		{
			get { return this._SkillCounter; }
			set
			{
				if (!Equals(this._SkillCounter, value))
				{
					this._SkillCounter = value;
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

		public WeaponListViewModel()
		{
			this.DeckWeapon = Visibility.Collapsed;
			this.ListWeapon = Visibility.Collapsed;
			this.LoadingScreen = Visibility.Collapsed;
			Read();
		}
		private void Read()
		{
			GrandcypherClient.Current.WeaponHooker.WeaponListLoad += () =>
			{
				this.LoadingScreen = Visibility.Visible;
			};
			GrandcypherClient.Current.WeaponHooker.LoadingEnd += () =>
			{
				this.WeaponList = new List<WeaponInfo>(GrandcypherClient.Current.WeaponHooker.WeaponLists);
				this.LoadingScreen = Visibility.Collapsed;
				this.DeckWeapon = Visibility.Collapsed;
				this.ListWeapon = Visibility.Visible;
			};
			GrandcypherClient.Current.WeaponHooker.DeckLoadingEnd += () =>
			{
				this.WeaponList = new List<WeaponInfo>(GrandcypherClient.Current.WeaponHooker.WeaponLists);
				this.SkillList = new List<string>(GrandcypherClient.Current.WeaponHooker.SkillList);
				this.MainWeapon = GrandcypherClient.Current.WeaponHooker.MainWeapon;
				this.SkillCounter = GrandcypherClient.Current.WeaponHooker.SkillCounter;
				this.CalcAttack(this.SkillCounter);

				this.LoadingScreen = Visibility.Collapsed;
				this.DeckWeapon = Visibility.Visible;
				this.ListWeapon = Visibility.Collapsed;
			};
			GrandcypherClient.Current.WeaponHooker.ProgressBar += () =>
			{
				string temp = GrandcypherClient.Current.WeaponHooker.ProgressStatus.Max.ToString() + "개중 " +
					GrandcypherClient.Current.WeaponHooker.ProgressStatus.Current.ToString() + "개 로드완료";
				GrandcypherClient.Current.PostMan(temp);
				this.Progress = temp;
			};
		}
		private void CalcAttack(Skills skillInfo, bool IsBaha = false)
		{
			int total = skillInfo.TotalAttack;

			double percent = (skillInfo.Large * 6 + skillInfo.Middle * 3 + skillInfo.Small * 1) / (double)100 + 1;
			if (IsBaha) percent = (skillInfo.Large * 6 + skillInfo.Middle * 3 + skillInfo.Small * 1) / (double)100 + 1 + 0.2d;
			double Magnapercent = (skillInfo.MagnaL * 6 + skillInfo.MagnaM * 3 + skillInfo.MagnaS * 1) / (double)100 + 1;
			double Unknownpercent = (skillInfo.UnknownL * 6 + skillInfo.UnknownM * 3 + skillInfo.UnknownS * 1) / (double)100 + 1;
			this.BeforeCalcAtt = this.CalcAtt;
			this.CalcAtt = Convert.ToInt32(Math.Round(total * percent * Magnapercent * Unknownpercent, 0, MidpointRounding.AwayFromZero));


			StringBuilder stbr = new StringBuilder();
			stbr.Append("(" + total);
			if (!IsBaha) stbr.Append("×" + string.Format("{0:F2}", percent));
			else
			{
				stbr.Append("×(" + string.Format("{0:F2}", percent - 0.2d));
				stbr.Append("+0.2)");
			}
			if (Magnapercent > 0) stbr.Append("×" + string.Format("{0:F2}", Magnapercent));
			if (Unknownpercent > 0) stbr.Append("×" + string.Format("{0:F2}", Unknownpercent));
			this.CalcAtt += skillInfo.NovelWeaponCount * 1000;
			if (skillInfo.NovelWeaponCount > 0) stbr.Append("+" + (1000 * skillInfo.NovelWeaponCount));
			stbr.Append(")");
			this.CalcAttDetail = stbr.ToString();
			if (this.CalcAtt - this.BeforeCalcAtt > 0) this.Gap = "+" + (this.CalcAtt - this.BeforeCalcAtt);
			else this.Gap = (this.CalcAtt - this.BeforeCalcAtt).ToString();
		}
	}
}
