using Fiddler;
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
		#endregion

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
	}
	public class ExpInfo
	{
		public int exp { get; set; }
		public int next_exp { get; set; }
		public int level { get; set; }
		public int max_level { get; set; }
		public int remain_next_exp { get; set; }
	}

}
