using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher
{
	public class BookmakerHooker
	{
		private List<AreaInfo> AreaList { get; set; }
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.Contains("/bookmaker/expect/") && oS.oResponse.MIMEType == "application/json")
				ExpectHook(oS);
		}
		private void ExpectHook(Session oS)
		{
			this.AreaList = new List<AreaInfo>();
			var today = DateTime.Now;

			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			dynamic full = jsonFull;
			dynamic areainfo = full.area_info;

			int MaxCount = 0;
			foreach (var item in areainfo)
			{
				MaxCount++;
			}

			foreach (var item in areainfo)
			{
				AreaInfo Info = new AreaInfo();

				Info.id = item.id;
				Info.name = item.name;
				Info.point = item.point;
				Info.ranking = item.ranking;

#if DEBUG
				string raw = Info.id + "," + Info.name + "," + Info.point + "," + Info.ranking;
				Console.WriteLine(raw);
#endif
				this.AreaList.Add(Info);
			}
			StringBuilder stbr = new StringBuilder();
			stbr.Append(today.ToLongDateString()+",");

			for (int i = 0; i < AreaList.Count; i++)
			{
				stbr.Append(AreaList[i].point + ",");
			}
			for (int i = 0; i < AreaList.Count; i++)
			{
				stbr.Append(AreaList[i].ranking);
			}
#if DEBUG
			Console.WriteLine(stbr.ToString());
#endif
		}
	}
	public class AreaInfo
	{
		public int id { get; set; }
		public string name { get; set; }
		public decimal point { get; set; }
		public int ranking { get; set; }
	}
}
