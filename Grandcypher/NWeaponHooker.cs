using Fiddler;
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
		#endregion
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
			dynamic firstPage = jsonFull["first"];
			dynamic lastPage = jsonFull["last"];
			dynamic currentPage = jsonFull["current"];

			JArray currentlist = JArray.Parse(jsonFull["list"].ToString()) as JArray;
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

			}
		}
	}
}
