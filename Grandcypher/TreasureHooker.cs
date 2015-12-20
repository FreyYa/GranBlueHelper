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
		public Dictionary<string,int> CurrentTreasureList { get; set; }
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.Contains("/item/article_list/") && oS.oResponse.MIMEType == "application/json")
				TreasureDetail(oS);
		}
		/// <summary>
		/// 유저 아이템 목록을 수집합니다.
		/// </summary>
		/// <param name="oS"></param>
		private void TreasureDetail(Session oS)
		{
			JArray jsonVal = JArray.Parse(oS.GetResponseBodyAsString()) as JArray;
			dynamic Details = jsonVal;

			CurrentTreasureList = new Dictionary<string, int>();

			foreach (dynamic detail in Details)
			{
				string name = detail.name;
				int count = detail.number;

				CurrentTreasureList.Add(name, count);
			}
		}

	}
}
