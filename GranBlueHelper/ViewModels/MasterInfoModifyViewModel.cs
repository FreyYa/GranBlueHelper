using Grandcypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GranBlueHelper.ViewModels
{
	public class MasterInfoModifyViewModel : WindowViewModel
	{
		#region SkillDetails
		private List<string> _SkillDetails;
		public List<string> SkillDetails
		{
			get { return this._SkillDetails; }
			set
			{
				if (this._SkillDetails == value) return;
				this._SkillDetails = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Skill I
		private string _SelectedDetail1;
		public string SelectedDetail1
		{
			get { return this._SelectedDetail1; }
			set
			{
				if (this._SelectedDetail1 == value) return;
				this._SelectedDetail1 = value;
				if (value != null)
				{
					NameUpdate(this._SelectedDetail1, 1);
					this.RaisePropertyChanged();
				}
			}
		}
		private string _SkillName1;
		public string SkillName1
		{
			get { return this._SkillName1; }
			set
			{
				if (this._SkillName1 == value) return;
				this._SkillName1 = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Skill II
		private string _SelectedDetail2;
		public string SelectedDetail2
		{
			get { return this._SelectedDetail2; }
			set
			{
				if (this._SelectedDetail2 == value) return;
				this._SelectedDetail2 = value;
				if (value != null)
				{
					NameUpdate(this._SelectedDetail2, 2);
					this.RaisePropertyChanged();
				}
			}
		}

		private string _SkillName2;
		public string SkillName2
		{
			get { return this._SkillName2; }
			set
			{
				if (this._SkillName2 == value) return;
				this._SkillName2 = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Attribute
		public static Dictionary<string, int> AttributeTable = new Dictionary<string, int>
		{
			{"화", 1}, {"수", 2}, {"토", 3}, {"풍", 4}, {"빛", 5}, {"암", 6}
		};
		public IEnumerable<string> AttributeLists { get; private set; }

		private string _SelectedAttribute;

		public string SelectedAttribute
		{
			get { return this._SelectedAttribute; }
			set
			{
				if (!Equals(this._SelectedAttribute, value))
				{
					this._SelectedAttribute = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region WeaponKind
		public static Dictionary<string, int> WeaponKindTable = new Dictionary<string, int>
		{
			{"검", 1}, {"단검", 2}, {"창", 3}, {"도끼", 4}, {"지팡이", 5}, {"총", 6}, {"권갑", 7}, {"활", 8}, {"악기", 9}, {"도", 10}, {"소재",11 },
		};
		public IEnumerable<string> WeaponKindLists { get; private set; }

		private string _SelectedWeaponKind;

		public string SelectedWeaponKind
		{
			get { return this._SelectedWeaponKind; }
			set
			{
				if (!Equals(this._SelectedWeaponKind, value))
				{
					this._SelectedWeaponKind = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ItemName
		private string _ItemName;
		public string ItemName
		{
			get { return this._ItemName; }
			set
			{
				if (this._ItemName == value) return;
				this._ItemName = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region MasterId
		private List<int> _MasterIdList;
		public List<int> MasterIdList
		{
			get { return this._MasterIdList; }
			set
			{
				if (this._MasterIdList == value) return;
				this._MasterIdList = value;
				this.RaisePropertyChanged();
			}
		}
		private List<WeaponInfo> _ManualList;
		public List<WeaponInfo> ManualList
		{
			get { return this._ManualList; }
			set
			{
				if (this._ManualList == value) return;
				this._ManualList = value;
				this.RaisePropertyChanged();
			}
		}
		private int _SelectedMasterId;
		public int SelectedMasterId
		{
			get { return this._SelectedMasterId; }
			set
			{
				if (this._SelectedMasterId == value) return;
				this._SelectedMasterId = value;
				this.LoadMasterInfo(value);
				this.RaisePropertyChanged();
			}
		}
		#endregion

		public MasterInfoModifyViewModel()
		{
			this.Title = "무기 정보 수정";
			this.SkillDetails = new List<string>(GrandcypherClient.Current.Translations.GetSkillList());
			this.AttributeLists = AttributeTable.Keys.ToList();
			this.WeaponKindLists = WeaponKindTable.Keys.ToList();
			this.MasterIdList = new List<int>(MakeMasterIdList());
		}
		private void LoadMasterInfo(int ID)
		{
			if (this.SelectedMasterId == 0) return;
			foreach (var item in ManualList)
			{
				if (item.MasterId == ID)
				{
					this.SelectedAttribute = item.Element;
					this.SelectedWeaponKind = item.Kind;
					this.ItemName = item.ItemName;
					this.SelectedDetail1 = item.SkillDetail1;
					this.SelectedDetail2 = item.SkillDetail2;
					return;
				}
			}
		}
		private List<WeaponInfo> RemoveDuplicateValue(List<WeaponInfo> array)
		{
			List<WeaponInfo> list = new List<WeaponInfo>();
			for (int i = 0; i < array.Count; i++)
			{
				if (list.Any(x => x.MasterId == array[i].MasterId))
				{
					continue;
				}
				list.Add(array[i]);
			}
			return list;
		}

		private List<int> MakeMasterIdList()
		{
			List<int> temp = new List<int>();

			ManualList = GrandcypherClient.Current.WeaponHooker.WeaponLists.Where(x => x.IsManual).ToList();
			if (GrandcypherClient.Current.WeaponHooker.MainWeapon.IsManual)
				ManualList.Add(GrandcypherClient.Current.WeaponHooker.MainWeapon);
			var t = RemoveDuplicateValue(ManualList);

			foreach (var item in t)
			{
				temp.Add(item.MasterId);
			}

			return temp;
		}
		public void SaveData()
		{
			if (this.SelectedMasterId < 1) return;
			WeaponInfo temp = new WeaponInfo
			{
				MasterId = this.SelectedMasterId,
				ItemName = this.ItemName,
				attribute = AttributeTable[this.SelectedAttribute],
				WeaponType = WeaponKindTable[this.SelectedWeaponKind],
			};
			if (this.SkillName1 != null) temp.SkillName1 = this.SkillName1;
			else temp.SkillName1 = string.Empty;
			if (this.SkillName2 != null) temp.SkillName2 = this.SkillName2;
			else temp.SkillName2 = string.Empty;

			if (this.SelectedDetail1 != null) temp.SkillDetail1 = this.SelectedDetail1;
			if (this.SelectedDetail2 != null) temp.SkillDetail2 = this.SelectedDetail2;

			GrandcypherClient.Current.WeaponHooker.MasterInfoSave(temp);
			GrandcypherClient.Current.WeaponHooker.Reload();
		}
		private void NameUpdate(string Detail, int Num)
		{
			if (Num == 1) this.SkillName1 = GrandcypherClient.Current.Translations.GetSkillInfo(this.SelectedDetail1);
			else this.SkillName2 = GrandcypherClient.Current.Translations.GetSkillInfo(this.SelectedDetail2);
		}
	}
}
