using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher
{
	public class TreasureHooker
	{
		public List<TreasureInfo> TreasureList { get; set; }
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.Contains("/item/article_list/") && oS.oResponse.MIMEType == "application/json")
				TreasureDetail(oS);
		}
		private void TreasureDetail(Session oS)
		{
			JArray jsonVal = JArray.Parse(oS.GetResponseBodyAsString()) as JArray;
			dynamic Details = jsonVal;

			TreasureList = new List<TreasureInfo>();

			foreach (dynamic detail in Details)
			{
				string name = detail.name;
				string comment = detail.comment;
				int count = detail.number;

				TreasureInfo temp = new TreasureInfo
				{
					Name = name,
					Detail = comment,
					Count = count
				};
				TreasureList.Add(temp);

			}
		}

	}
	public class TreasureInfo
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public string Detail { get; set; }
	}
}
