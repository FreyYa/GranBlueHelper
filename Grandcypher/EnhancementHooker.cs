using Fiddler;
using Livet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher
{
	public class EnhancementHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler WeaponReadStart;
		public EventHandler WeaponReadEnd;
		public EventHandler CharReadStart;
		public EventHandler CharReadEnd;
		public EventHandler CalcEnd;
		public EventHandler ChangeEnd;
		#endregion

		public List<EnhanceInfo> EnhanceList { get; set; }
		public EnhanceInfo MasterInfo { get; set; }
		public ExpInfo expInfo { get; set; }
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/weapon/weapon_base") && oS.oResponse.MIMEType == "application/json")
				EnhancementHook(oS, true);
			if (oS.PathAndQuery.StartsWith("/npc/npc_base") && oS.oResponse.MIMEType == "application/json")
				EnhancementHook(oS, false);
			if (oS.PathAndQuery.StartsWith("/summon/summon_base") && oS.oResponse.MIMEType == "application/json")
				EnhancementHook(oS, true);
		}
		public EnhancementHooker()
		{
			this.EnhanceList = new List<EnhanceInfo>();
			for (int i = 0; i < 20; i++)
			{
				EnhanceInfo temp = new EnhanceInfo();
				temp.ID = i;
				this.EnhanceList.Add(temp);
			}
			this.MasterInfo = new EnhanceInfo();
			this.MasterInfo.ID = -1;
		}
		public void Clear()
		{
			List<EnhanceInfo> temps = new List<EnhanceInfo>();

			for (int i = 0; i < 20; i++)
			{
				EnhanceInfo temp = new EnhanceInfo();
				temp.ID = i;
				temps.Add(temp);
			}
			this.EnhanceList = new List<EnhanceInfo>(temps);
			this.ChangeEnd();
		}
		private void EnhancementHook(Session oS, bool IsWeapon)
		{
			if (IsWeapon) this.WeaponReadStart();
			else this.CharReadStart();
			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;

			dynamic test = jsonFull;
			var list = test.param as JObject;
			expInfo = new ExpInfo
			{
				exp = (int)list["exp"],
				next_exp = (int)list["next_exp"],
				level = (int)list["level"],
				max_level = (int)list["max_level"],
				remain_next_exp = (int)list["remain_next_exp"],
			};
			if (IsWeapon) this.WeaponReadEnd();
			else this.CharReadEnd();
		}
		public void ChangeListData(EnhanceInfo data)
		{
			if (data.ID == -1)
			{
				this.MasterInfo = data;
			}
			else
			{
				for (int i = 0; i < EnhanceList.Count; i++)
				{
					if (EnhanceList[i].ID == data.ID)
					{
						data.ID = i;
						EnhanceList[i] = data;
						break;
					}
				}
			}
			this.ChangeEnd();
		}
		public void EnhancementCalc()
		{
			var bottom = this.MasterInfo.Result;
			if (bottom > 0)
			{
				decimal total = 0;
				for (int i = 0; i < EnhanceList.Count; i++)
				{
					if (EnhanceList[i].Result == 0) continue;

					var temp = EnhanceList[i].Result / bottom;
					temp = Math.Truncate(temp * 1000);
					temp = temp / 1000;
					total += temp;
				}
				this.MasterInfo.TotalResult = total;
			}
			this.CalcEnd();
		}
	}
	public class ExpInfo
	{
		public int exp { get; set; }
		public int next_exp { get; set; }
		public int level { get; set; }
		public int max_level { get; set; }
		public int remain_next_exp { get; set; }
	}
	public class EnhanceInfo : NotificationObject
	{

		#region Rank List
		//게임 내 표기 1 노멀 2 레어 3 SR 4 SSR
		private static Dictionary<string, int> RankTable = new Dictionary<string, int>
		{
			{"R",2}, {"SR",4 }, {"SSR",8 }, {"바하무트",40}
		};
		public IEnumerable<string> RankList { get { return RankTable.Keys.ToList(); } }

		private string _SelectedRank;
		public string SelectedRank
		{
			get { return this._SelectedRank; }
			set
			{
				if (this._SelectedRank == value) return;
				this._SelectedRank = value;
				this.CalcResult();
				GrandcypherClient.Current.EnhancementHooker.ChangeListData(this);
				GrandcypherClient.Current.EnhancementHooker.EnhancementCalc();
			}
		}

		private static Dictionary<string, decimal> ElementTable = new Dictionary<string, decimal>
		{
			{"없음",0 },{"R",1}, {"SR",4 }, {"SSR",40 }, {"쁘띠",2}, {"데빌" ,4.8m}
		};
		public IEnumerable<string> ElementList { get { return ElementTable.Keys.ToList(); } }
		private string _SelectedElement;
		public string SelectedElement
		{
			get { return this._SelectedElement; }
			set
			{
				if (this._SelectedElement == value) return;
				this._SelectedElement = value;
				this.CalcResult();
				GrandcypherClient.Current.EnhancementHooker.ChangeListData(this);
				GrandcypherClient.Current.EnhancementHooker.EnhancementCalc();
			}
		}
		#endregion
		public int ID { get; set; }
		private int _SkillLv;
		public int SkillLv
		{
			get { return this._SkillLv; }
			set
			{
				if (this._SkillLv == value) return;
				if (value == 0) return;
				this._SkillLv = value;
				this.CalcResult();
				GrandcypherClient.Current.EnhancementHooker.ChangeListData(this);
				GrandcypherClient.Current.EnhancementHooker.EnhancementCalc();
			}
		}
		public decimal Result { get; set; }
		public decimal TotalResult { get; set; }
		private void CalcResult()
		{
			if (this.SelectedElement != null) this.Result = SkillLv * ElementTable[SelectedElement];
			if (this.SelectedRank != null) this.Result = SkillLv * RankTable[SelectedRank];
		}
	}
}
