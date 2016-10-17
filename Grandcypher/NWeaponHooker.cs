using Fiddler;
using Grandcypher.Raw;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grandcypher
{
	public class NWeaponHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler FilterError;
		public EventHandler FinishRead;
		#endregion
		private Dictionary<int, List<Weapon>> _WeaponList { get; set; }
		public List<Weapon> WeaponList { get; private set; }
		public NWeaponHooker()
		{
			this._WeaponList = new Dictionary<int, List<Weapon>>();
			this.WeaponList = new List<Weapon>();
		}
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/weapon/list") && oS.oResponse.MIMEType == "application/json")
				ListHook(oS);
		}
		private void ListHook(Session oS)
		{
			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			dynamic options = jsonFull["options"]["status_short_name"];
			if (options != "SLv")
			//리스트 정렬상태가 스킬레벨이 아닌경우 리턴
			{
				this.FilterError();
				GrandcypherClient.Current.PostMan("무기 목록 정렬을 스킬레벨 기준으로 변경해주시기 바랍니다");
				return;
			}
			int firstPage = Convert.ToInt32(jsonFull["first"]);
			int lastPage = Convert.ToInt32(jsonFull["last"]);
			int currentPage = Convert.ToInt32(jsonFull["current"]);

			JArray currentlist = jsonFull["list"] as JArray;
			if (!_WeaponList.ContainsKey(currentPage))
			{
				List<Weapon> temp_weapon_list = new List<Weapon>();
				foreach (var item in currentlist)
				{
					#region DEBUG
#if DEBUG
					Debug.WriteLine(item["master"]["id"]);
					Debug.WriteLine(item["param"]["id"]);
					Debug.WriteLine(item["param"]["status"]);
					Debug.WriteLine("------------------------------");
#endif
					#endregion
					Weapon temp = new Weapon();
					temp.master.id = Convert.ToInt32(item["master"]["id"]);
					temp.param.id = Convert.ToInt32(item["param"]["id"]);
					temp.param.SetStatus(item["param"]["status"].ToString());
					temp.master.skill1_image = item["master"]["skill1_image"].ToString();
					temp.master.skill2_image = item["master"]["skill2_image"].ToString();

					temp_weapon_list.Add(temp);
				}
				_WeaponList.Add(currentPage, temp_weapon_list);
			}
			if (this._WeaponList.Count == lastPage)
			{
				foreach (var item in this._WeaponList)
				{
					this.WeaponList = this.WeaponList.Concat(item.Value).Where(x => x.master.skill1_image != string.Empty || x.master.skill2_image != string.Empty).ToList();
				}
				this.FinishRead();
			}
		}
	}
}
