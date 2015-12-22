using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grandcypher
{
	public class TreasureHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler TreasureReadStart;
		public EventHandler TreasureReadEnd;
		#endregion

		public Dictionary<string, TreasureInfo> CurrentTreasureList { get; set; }
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

			CurrentTreasureList = new Dictionary<string, TreasureInfo>();

			//this.TreasureReadStart();
			foreach (dynamic detail in Details)
			{
				string name = detail.name;
				int count = detail.number;
				int image = detail.image;
				string imgUrl = "http://gbf.game-a.mbga.jp/assets/img/sp/assets/item/article/s/" + image + ".jpg";
				GrandcypherClient.Current.GreetHooker.FileDownloader(imgUrl, image + ".jpg", false);
				TreasureInfo temp = new TreasureInfo
				{
					Name = name,
					count = count,
					ItemID = image
				};

				CurrentTreasureList.Add(name, temp);
			}
			this.TreasureReadEnd();
		}

	}
	public class TreasureInfo
	{
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		public string Name { get; set; }
		public int ItemID { get; set; }
		public int count { get; set; }//current
		public int max { get; set; }//
		public int result { get; set; }//max-current
		public ImageSource LocalImage
		{
			get
			{
				if (this.ItemID > 0)
				{
					if (File.Exists(Path.Combine(MainFolder, "Treasures", this.ItemID + ".jpg")))
						return new BitmapImage(new Uri(Path.Combine(MainFolder, "Treasures", this.ItemID + ".jpg"), UriKind.Absolute));
					else return null;
				}
				else
				{
					if (File.Exists(Path.Combine(MainFolder, "Treasures", "null.jpg")))
						return new BitmapImage(new Uri(Path.Combine(MainFolder, "Treasures", "null.jpg"), UriKind.Absolute));
					else return null;
				}
			}
		}

	}

	public class TenTreasureInfo
	{
		public string Name { get; set; }
		public int Proto { get; set; }
		public int Rust { get; set; }
		public int Element { get; set; }
		public int First { get; set; }
		public int Second { get; set; }
		public int Third { get; set; }
		public int Fourth { get; set; }
		public int Fifth { get; set; }
		public int Sixth { get; set; }
		public int ElementID { get; set; }
		public int Origin { get; set; }
	}

}
