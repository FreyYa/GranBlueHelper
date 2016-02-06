using Grandcypher;
using Livet;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private decimal _CalcAtt;
		public decimal CalcAtt
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
		private decimal _BeforeCalcAtt;
		public decimal BeforeCalcAtt
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
		private decimal _Total;
		public decimal Total
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

		#region TargetMagna
		private string _TargetMagna;
		public string TargetMagna
		{
			get { return this._TargetMagna; }
			set
			{
				if (this._TargetMagna == value) return;
				this._TargetMagna = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region TargetMagna Visible
		private Visibility _TargetMagnaVisible;
		public Visibility TargetMagnaVisible
		{
			get { return this._TargetMagnaVisible; }
			set
			{
				if (!Equals(this._TargetMagnaVisible, value))
				{
					this._TargetMagnaVisible = value;
					this.RaisePropertyChanged();
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

		#region SelectedSynastry

		private string _SelectedSynastry;

		public string SelectedSynastry
		{
			get { return this._SelectedSynastry; }
			set
			{
				if (_SelectedSynastry != value)
				{
					this._SelectedSynastry = value;
					this.RaisePropertyChanged();
					CalcAttack(this.SkillCounter);
				}
			}
		}

		#endregion

		public IEnumerable<string> BlessingList { get; private set; }
		public IEnumerable<string> SynastryList { get; private set; }

		/// <summary>
		/// 통상 0  언노운 1 마그나 2 캐릭터 3
		/// </summary>
		public static Dictionary<string, int> BlessingTable = new Dictionary<string, int>
		{
			{"속성", 0}, {"언노운", 1}, {"마그나", 2},{"캐릭터",3},{"일반공인강화",4}
		};

		/// <summary>
		/// 유리 1.5 통상 1 불리 0.75
		/// </summary>
		public static Dictionary<string, decimal> SynastryTable = new Dictionary<string, decimal>
		{
			{"유리", 1.5m}, {"통상", 1}, {"불리", 0.75m}
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
			this.SynastryList = SynastryTable.Keys.ToList();

			this.DeckWeapon = Visibility.Collapsed;
			this.ListWeapon = Visibility.Collapsed;
			this.LoadingScreen = Visibility.Collapsed;
			this.IsManualExist = false;

			Read();

			SelectedBlessing = BlessingTable.Keys.FirstOrDefault();
			SelectedFriendBlessing = BlessingTable.Keys.FirstOrDefault();
			SelectedSynastry = SynastryTable.Keys.FirstOrDefault();

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
			//스킬 배율을 저장
			decimal total = skillInfo.BasicAttack;
			this.Total = total;

			decimal baha = skillInfo.Baha / 100m;

			decimal percent = skillInfo.Noramal / 100m;

			decimal Magnapercent = skillInfo.Magna / 100m;

			decimal Unknownpercent = skillInfo.Unknown / 100m;

			decimal Strangthpercent = skillInfo.Str / 100m;

			decimal Savingpercent = skillInfo.Saving / 100m;

			decimal Attributepercent = SynastryTable[this.SelectedSynastry];

			//퍼센트 저장 변수
			decimal pNormal = 1, pMagna = 1, pUnknown = 1, pElement = 0, pChar = 0;

			//자신의 가호
			if (BlessingTable[this.SelectedBlessing] == 3) pChar += this.BlessingPercent / 100m;//캐릭터
			else if (BlessingTable[this.SelectedBlessing] == 2) pMagna += this.BlessingPercent / 100m;//마그나
			else if (BlessingTable[this.SelectedBlessing] == 1) pUnknown += this.BlessingPercent / 100m;//언노운
			else if (BlessingTable[this.SelectedBlessing] == 4) pNormal += this.BlessingPercent / 100m;//제우스 티탄 등 일반공인 부스트
			else if (BlessingTable[this.SelectedBlessing] == 0) pElement += (decimal)this.BlessingPercent / 100m;//속성 

			//친구의 가호
			if (BlessingTable[this.SelectedFriendBlessing] == 3) pChar += this.FriendBlessingPercent / 100m;//캐릭터
			else if (BlessingTable[this.SelectedFriendBlessing] == 2) pMagna += this.FriendBlessingPercent / 100m;//마그나
			else if (BlessingTable[this.SelectedFriendBlessing] == 1) pUnknown += this.FriendBlessingPercent / 100m;//언노운
			else if (BlessingTable[this.SelectedFriendBlessing] == 4) pNormal += this.FriendBlessingPercent / 100m;//제우스 티탄 등 일반공인 부스트
			else if (BlessingTable[this.SelectedFriendBlessing] == 0) pElement += (decimal)this.FriendBlessingPercent / 100m;//속성 



			this.CalcAtt = Convert.ToInt32(Math.Round(total * (1 + baha + (percent * pNormal) + pChar) * (1 + Magnapercent * pMagna) * (1 + (Unknownpercent * pUnknown) + Strangthpercent + Savingpercent) * (Attributepercent + pElement), 0, MidpointRounding.AwayFromZero));

			StringBuilder stbr = new StringBuilder();

			if (!this.IsvisBaha && !this.IsconcilioBaha) stbr.Append("×" + string.Format("{0:0.##}", 1 + percent * pNormal));
			else
			{
				stbr.Append("×(" + string.Format("{0:0.##}", 1 + percent * pNormal));
				stbr.Append("+" + baha + ")");
			}
			this.NormalWeapon = stbr.ToString();
			stbr.Clear();

			if (Magnapercent > 0) this.MagnaWeapon = "×" + string.Format("{0:0.##}", 1 + Magnapercent * pMagna);

			if (Unknownpercent > 0) this.UnknownWeapon = "×" + string.Format("{0:0.##}", 1 + Unknownpercent * pUnknown);
			this.UnknownWeaponTooltip = "언노운 무기 배율";
			if (Strangthpercent > 0 && Savingpercent == 0)
			{
				this.UnknownWeapon = "×(" + string.Format("{0:F2}", 1 + Unknownpercent * pUnknown) + "+" + string.Format("{0:0.##}", Strangthpercent) + ")";
				this.UnknownWeaponTooltip = "언노운 & 스트렝스 무기 배율";
			}
			else if (Strangthpercent == 0 && Savingpercent > 0)
			{
				this.UnknownWeapon = "×(" + string.Format("{0:F2}", 1 + Unknownpercent * pUnknown) + "+" + string.Format("{0:0.##}", Savingpercent) + ")";
				this.UnknownWeaponTooltip = "언노운 & 세이빙 무기 배율";
			}
			else if (Strangthpercent > 0 && Savingpercent > 0)
			{
				this.UnknownWeapon = "×(" + string.Format("{0:F2}", 1 + Unknownpercent * pUnknown) + "+" + string.Format("{0:0.##}", Strangthpercent) + "+" + string.Format("{0:0.##}", Savingpercent) + ")";
				this.UnknownWeaponTooltip = "언노운 & 스트렝스 & 세이빙 무기 배율";
			}

			if (Attributepercent > 0) this.Attribute = "×" + string.Format("{0:0.##}", Attributepercent + pElement);


			this.CalcAtt += skillInfo.staticAtt;
			if (skillInfo.staticAtt > 0) this.NovelWeapon = "+" + (skillInfo.staticAtt);


			var tempNPC = new List<NpcInfo>(GrandcypherClient.Current.WeaponHooker.NPCList);

			for (int i = 0; i < tempNPC.Count; i++)
			{
				tempNPC[i].CalcAtt = Convert.ToInt32(Math.Round(tempNPC[i].attack * (1 + baha + (percent * pNormal) + pChar) * (1 + Magnapercent * pMagna) * (1 + (Unknownpercent * pUnknown) + Strangthpercent + Savingpercent) * (Attributepercent + pElement), 0, MidpointRounding.AwayFromZero));
			}
			this.NPCList = new List<NpcInfo>(tempNPC);
			if (this.CalcTargetSkillLv(skillInfo) > 0) this.TargetMagna = this.CalcTargetSkillLv(skillInfo).ToString("##0.##%");
			else this.TargetMagna = "조건 달성";

			#region DEBUG
#if DEBUG
			Debug.WriteLine(this.CalcTargetSkillLv(skillInfo));
#endif
			#endregion

		}
		private decimal CalcTargetSkillLv(Skills skillInfo)
		{
			if (BlessingTable[this.SelectedFriendBlessing] == 0 && BlessingTable[this.SelectedBlessing] == 0)
			{
				var magna = skillInfo.Magna / 100m;

				var Ans = (magna + 1) * (SynastryTable[this.SelectedSynastry] + (this.BlessingPercent / 100m) + (this.FriendBlessingPercent / 100m));

				var targetup = (SynastryTable[this.SelectedSynastry] + (this.BlessingPercent / 100m) + (this.FriendBlessingPercent / 100m)) * magna + (this.BlessingPercent / 100m);
				var targetdown = 2 * (SynastryTable[this.SelectedSynastry] + this.FriendBlessingPercent / 100m);
				var target = targetup / targetdown;

				this.TargetMagnaVisible = Visibility.Visible;
				return target - (magna);
			}
			else
			{
				this.TargetMagnaVisible = Visibility.Collapsed;
				return -1;
			}
		}
	}
}
