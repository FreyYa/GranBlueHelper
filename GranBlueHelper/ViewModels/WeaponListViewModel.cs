using Grandcypher;
using Livet;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Settings = GranBlueHelper.Models.Settings;

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
		private bool _IsvisBaha;
		public bool IsvisBaha
		{
			get { return this._IsvisBaha; }
			set
			{
				if (!Equals(this._IsvisBaha, value))
				{
					this._IsvisBaha = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
				}
			}
		}
		private bool _IsconcilioBaha;
		public bool IsconcilioBaha
		{
			get { return this._IsconcilioBaha; }
			set
			{
				if (!Equals(this._IsconcilioBaha, value))
				{
					this._IsconcilioBaha = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
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

		#region NPC List
		private List<NpcInfo> _NPCList;
		public List<NpcInfo> NPCList
		{
			get { return this._NPCList; }
			set
			{
				if (this._NPCList == value) return;
				this._NPCList = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region WeaponString List
		private string _NormalWeapon;
		public string NormalWeapon
		{
			get { return this._NormalWeapon; }
			set
			{
				if (this._NormalWeapon == value) return;
				this._NormalWeapon = value;
				this.RaisePropertyChanged();
			}
		}
		private string _MagnaWeapon;
		public string MagnaWeapon
		{
			get { return this._MagnaWeapon; }
			set
			{
				if (this._MagnaWeapon == value) return;
				this._MagnaWeapon = value;
				this.RaisePropertyChanged();
			}
		}
		private string _UnknownWeapon;
		public string UnknownWeapon
		{
			get { return this._UnknownWeapon; }
			set
			{
				if (this._UnknownWeapon == value) return;
				this._UnknownWeapon = value;
				this.RaisePropertyChanged();
			}
		}
		private string _UnknownWeaponTooltip;
		public string UnknownWeaponTooltip
		{
			get { return this._UnknownWeaponTooltip; }
			set
			{
				if (this._UnknownWeaponTooltip == value) return;
				this._UnknownWeaponTooltip = value;
				this.RaisePropertyChanged();
			}
		}
		private string _NovelWeapon;
		public string NovelWeapon
		{
			get { return this._NovelWeapon; }
			set
			{
				if (this._NovelWeapon == value) return;
				this._NovelWeapon = value;
				this.RaisePropertyChanged();
			}
		}
		private string _Attribute;
		public string Attribute
		{
			get { return this._Attribute; }
			set
			{
				if (this._Attribute == value) return;
				this._Attribute = value;
				this.RaisePropertyChanged();
			}
		}
		private int _Total;
		public int Total
		{
			get { return this._Total; }
			set
			{
				if (this._Total == value) return;
				this._Total = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region SelectedBlessing

		private string _SelectedBlessing;

		public string SelectedBlessing
		{
			get { return this._SelectedBlessing; }
			set
			{
				if (_SelectedBlessing != value)
				{
					this._SelectedBlessing = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
				}
			}
		}

		#endregion

		#region SelectedFriendBlessing

		private string _SelectedFriendBlessing;

		public string SelectedFriendBlessing
		{
			get { return this._SelectedFriendBlessing; }
			set
			{
				if (_SelectedFriendBlessing != value)
				{
					this._SelectedFriendBlessing = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
				}
			}
		}

		#endregion

		#region BlessingPercent

		private int _BlessingPercent;

		public int BlessingPercent
		{
			get { return this._BlessingPercent; }
			set
			{
				if (_BlessingPercent != value)
				{
					this._BlessingPercent = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
				}
			}
		}

		#endregion

		#region FriendBlessingPercent

		private int _FriendBlessingPercent;

		public int FriendBlessingPercent
		{
			get { return this._FriendBlessingPercent; }
			set
			{
				if (_FriendBlessingPercent != value)
				{
					this._FriendBlessingPercent = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
				}
			}
		}

		#endregion

		#region IsManualExist
		private bool _IsManualExist;
		public bool IsManualExist
		{
			get { return this._IsManualExist; }
			set
			{
				if (this._IsManualExist == value) return;
				this._IsManualExist = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		public IEnumerable<string> BlessingList { get; private set; }

		/// <summary>
		/// 통상 0  언노운 1 마그나 2 캐릭터 3
		/// </summary>
		public static Dictionary<string, int> BlessingTable = new Dictionary<string, int>
		{
			{"속성", 0}, {"언노운", 1}, {"마그나", 2},{"캐릭터",3},{"일반공인강화",4}
		};
		public void ShowMasterInfoModify()
		{
			var window = new MasterInfoModifyViewModel();
			var message = new TransitionMessage(window, "Show/MasterInfoModify");
			this.Messenger.RaiseAsync(message);
		}
		public WeaponListViewModel()
		{
			this.BlessingList = BlessingTable.Keys.ToList();

			this.DeckWeapon = Visibility.Collapsed;
			this.ListWeapon = Visibility.Collapsed;
			this.LoadingScreen = Visibility.Collapsed;
			this.IsManualExist = false;

			Read();

			SelectedBlessing = BlessingTable.Keys.FirstOrDefault();
			SelectedFriendBlessing = BlessingTable.Keys.FirstOrDefault();
			this.BlessingPercent = 40;
			this.FriendBlessingPercent = 40;
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

				if (this.WeaponList.Where(x => x.IsManual).Count() > 0 || this.MainWeapon.IsManual) this.IsManualExist = true;
				else this.IsManualExist = false;

				this.IsconcilioBaha = GrandcypherClient.Current.WeaponHooker.IsConcilioExist;
				this.IsvisBaha = GrandcypherClient.Current.WeaponHooker.IsVisExist;

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
		public void CompareAttackStatus()
		{
			if (this.CalcAtt - this.BeforeCalcAtt > 0) this.Gap = "+" + (this.CalcAtt - this.BeforeCalcAtt);
			else this.Gap = (this.CalcAtt - this.BeforeCalcAtt).ToString();

			this.BeforeCalcAtt = this.CalcAtt;
		}
		private void CalcAttack(Skills skillInfo)
		{
			if (skillInfo == null) return;
			int total = skillInfo.TotalAttack;
			decimal baha = 0;
			decimal percent = (skillInfo.Large * 6 + skillInfo.Middle * 3 + skillInfo.Small * 1 + skillInfo.NormalSkillLvCount) / 100m + 1;

			decimal Magnapercent = (skillInfo.MagnaL * 6 + skillInfo.MagnaM * 3 + skillInfo.MagnaS * 1 + skillInfo.MagnaSkillLvCount) / 100m + 1;

			decimal Unknownpercent = (skillInfo.UnknownL * 6 + skillInfo.UnknownM * 3 + skillInfo.UnknownS * 1 + skillInfo.UnknownSkillLvCount) / 100m + 1;

			decimal Strangthpercent = (skillInfo.StrL * 6 + skillInfo.StrM * 3 + skillInfo.StrS * 1 + skillInfo.StrangthSkillCount) / 100m;


			decimal Attributepercent = 1;

			//자신의 가호
			if (BlessingTable[this.SelectedBlessing] == 3)//캐릭터
			{
				percent += (decimal)this.BlessingPercent / 100m;
			}
			else if (BlessingTable[this.SelectedBlessing] == 2)//마그나
			{
				if (Magnapercent > 1)
				{
					Magnapercent -= 1;
					Magnapercent = Magnapercent * (1 + (decimal)this.BlessingPercent / 100m);
					Magnapercent += 1;
				}
			}
			else if (BlessingTable[this.SelectedBlessing] == 1)//언노운
			{
				if (Unknownpercent > 1)
				{
					Unknownpercent -= 1;
					Unknownpercent = Unknownpercent * (1 + (decimal)this.BlessingPercent / 100m);
					Unknownpercent += 1;
				}
			}
			else if (BlessingTable[this.SelectedBlessing] == 4)//제우스 티탄 등 일반공인 부스트
			{
				if (percent > 1)
				{
					percent -= 1;
					percent = percent * (1 + (decimal)this.BlessingPercent / 100m);
					percent += 1;
				}
			}
			else//속성
			{
				Attributepercent += (decimal)this.BlessingPercent / 100m;
			}

			//친구의 가호
			if (BlessingTable[this.SelectedFriendBlessing] == 3)//캐릭터
			{
				percent += (decimal)this.FriendBlessingPercent / 100m;
			}
			else if (BlessingTable[this.SelectedFriendBlessing] == 2)//마그나
			{
				if (Magnapercent > 1)
				{
					Magnapercent -= 1;
					Magnapercent = Magnapercent * (1 + (decimal)this.FriendBlessingPercent / 100m);
					Magnapercent += 1;
				}
			}
			else if (BlessingTable[this.SelectedFriendBlessing] == 1)//언노운
			{
				if (Unknownpercent > 1)
				{
					Unknownpercent -= 1;
					Unknownpercent = Unknownpercent * (1 + (decimal)this.FriendBlessingPercent / 100m);
					Unknownpercent += 1;
				}
			}
			else if (BlessingTable[this.SelectedFriendBlessing] == 4)//제우스 티탄 등 일반공인 부스트
			{
				percent -= 1;
				percent = percent * (1 + (decimal)this.BlessingPercent / 100m);
				percent += 1;
			}
			else//속성
			{
				Attributepercent += (decimal)this.FriendBlessingPercent / 100m;
			}

			if (this.IsvisBaha)
			{
				if (Settings.Current.visLv == 10)
				{
					percent += 0.3m;
					baha += 0.3m;
				}
				else
				{
					percent += 0.2m + Convert.ToDecimal((decimal)(Settings.Current.visLv - 1) / 100m);
					baha += 0.2m + Convert.ToDecimal((decimal)(Settings.Current.visLv - 1) / 100m);
				}
			}
			if (this.IsconcilioBaha)
			{
				if (Settings.Current.concilioLv == 10)
				{
					percent += 0.15m;
					baha += 0.15m;
				}
				else
				{
					percent += 0.1m + Convert.ToDecimal((decimal)(Settings.Current.concilioLv - 1) / 200m);
					baha += 0.1m + Convert.ToDecimal((decimal)(Settings.Current.concilioLv - 1) / 200m);
				}
			}

			this.CalcAtt = Convert.ToInt32(Math.Round(total * percent * Magnapercent * (Unknownpercent + Strangthpercent) * Attributepercent, 0, MidpointRounding.AwayFromZero));

			StringBuilder stbr = new StringBuilder();

			this.Total = total;
			if (!this.IsvisBaha && !this.IsconcilioBaha) stbr.Append("×" + string.Format("{0:F2}", percent));
			else
			{
				stbr.Append("×(" + string.Format("{0:F2}", percent - baha));
				stbr.Append("+" + baha + ")");
			}
			this.NormalWeapon = stbr.ToString();
			stbr.Clear();

			if (Magnapercent > 1) this.MagnaWeapon = "×" + string.Format("{0:F2}", Magnapercent);

			if (Unknownpercent > 1) this.UnknownWeapon = "×" + string.Format("{0:F2}", Unknownpercent);
			this.UnknownWeaponTooltip = "언노운 무기 배율";
			if (Strangthpercent > 0)
			{
				this.UnknownWeapon = "×(" + string.Format("{0:F2}", Unknownpercent) + "+" + string.Format("{0:F2}", Strangthpercent) + ")";
				this.UnknownWeaponTooltip = "언노운 & 스트렝스 무기 배율";
			}

			if (Attributepercent > 1) this.Attribute = "×" + string.Format("{0:F2}", Attributepercent);


			this.CalcAtt += skillInfo.NovelWeaponCount * 1000;
			if (skillInfo.NovelWeaponCount > 0) this.NovelWeapon = "+" + (1000 * skillInfo.NovelWeaponCount);


			var tempNPC = new List<NpcInfo>(GrandcypherClient.Current.WeaponHooker.NPCList);

			for (int i = 0; i < tempNPC.Count; i++)
			{
				tempNPC[i].CalcAtt = Convert.ToInt32(Math.Round(tempNPC[i].attack * percent * Magnapercent * (Unknownpercent + Strangthpercent) * Attributepercent, 0, MidpointRounding.AwayFromZero));
			}
			this.NPCList = new List<NpcInfo>(tempNPC);
		}
	}
}
