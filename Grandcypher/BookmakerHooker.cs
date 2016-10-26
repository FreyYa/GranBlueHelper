using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
			string[] ranking = new string[5];
			for (int i = 0; i < AreaList.Count; i++)
			{
				switch (i)
				{
					case 0://북
						ranking[AreaList[i].ranking] = "N";
						break;
					case 1://서
						ranking[AreaList[i].ranking] = "W";
						break;
					case 2://동
						ranking[AreaList[i].ranking] = "E";
						break;
					case 3://남
						ranking[AreaList[i].ranking] = "S";
						break;
				}
			}
			foreach (var item in ranking)//북서동남
			{
				stbr.Append(item);
			}
			this.Logger("{0},{1},{2},{3},{4},{5}", today.ToString(), AreaList[0].point, AreaList[1].point, AreaList[2].point, AreaList[3].point, stbr.ToString());
		}
		private void Logger(string str, params object[] args)
		{
			byte[] utf8Bom = { 0xEF, 0xBB, 0xBF };
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

			if (!System.IO.File.Exists(MainFolder + "\\BookMaker.csv"))
			{
				var csvPath = Path.Combine(MainFolder, "BookMaker.csv");
				using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
				using (var writer = new BinaryWriter(fileStream))
				{
					writer.Write(utf8Bom);
				}
				using (StreamWriter w = File.AppendText(MainFolder + "\\BookMaker.csv"))
				{
					w.WriteLine("날짜,북,서,동,남,랭킹", args);
				}
			}

			using (StreamWriter w = File.AppendText(MainFolder + "\\BookMaker.csv"))
			{
				w.WriteLine(str, args);
			}
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
