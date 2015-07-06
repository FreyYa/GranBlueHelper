using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher
{
	public class NoticeHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler NoticeListLoad;
		public EventHandler LoadingEnd;
		public EventHandler ProgressBar;
		public EventHandler ContentProgress;
		#endregion
		public List<NoticeInfo> NoticeLists { get; set; }
		public LimitedValue ProgressStatus { get; set; }
		public LimitedValue ContentProgressStatus { get; set; }
		public NoticeHooker()
		{

		}
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/news/news_data") && !oS.PathAndQuery.StartsWith("/news/news_data/?") && oS.oResponse.MIMEType == "application/json")
				NoticeDetail(oS);
		}
		private void NoticeDetail(Session oS)
		{
			this.NoticeListLoad();

			if (this.NoticeLists == null)
				this.NoticeLists = new List<NoticeInfo>();
			else
			{
				this.NoticeLists.Clear();
				this.NoticeLists.TrimExcess();
			}
			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			dynamic test = jsonFull;
			dynamic list = test.list;

			List<dynamic> NoticeList = new List<dynamic>();

			for (int i = 0; i < list.Count; i++)
			{
				NoticeList.Add(list[i]);
			}

			this.ProgressStatus = new LimitedValue();
			this.ProgressStatus.Max = NoticeList.Count;
			this.ProgressStatus.Min = 0;
			this.ProgressStatus.Current = 0;

			this.ProgressBar();

			for (int i = 0; i < NoticeList.Count; i++)
			{
				NoticeInfo temp = new NoticeInfo
				{
					contents = NoticeList[i].contents,
					title = NoticeList[i].title,
					date = NoticeList[i].date,
					created_at = NoticeList[i].created_at,
					updated_at = NoticeList[i].updated_at,
				};
				temp.contents = GrandcypherClient.Current.ScenarioHooker.RemoveWebTag(temp.contents, false);
				temp.contents = temp.contents.Replace("「", "[");
				temp.contents = temp.contents.Replace("」", "]");

				temp.title = temp.title.Replace("「", "[");
				temp.title = temp.title.Replace("」", "]");

				temp.TrContents = SplitContents(temp.contents);
				temp.TrTitle = GrandcypherClient.Current.ScenarioHooker.Translator(GrandcypherClient.Current.ScenarioHooker.RemoveWebTag(temp.title), GrandcypherClient.Current.ScenarioHooker.TranslateSite);

				NoticeLists.Add(temp);
				this.ProgressStatus.Current++;
				this.ProgressBar();
			}
			this.LoadingEnd();
		}
		public List<string> SplitContents(string rawString)
		{
			var temp = rawString.Split('\n');

			this.ContentProgressStatus = new LimitedValue();
			this.ContentProgressStatus.Max = temp.Count();
			this.ContentProgressStatus.Min = 0;
			this.ContentProgressStatus.Current = 0;

			List<string> translated = new List<string>();
			foreach (var item in temp.ToList())
			{
				translated.Add(GrandcypherClient.Current.ScenarioHooker.Translator(item, GrandcypherClient.Current.ScenarioHooker.TranslateSite));
				this.ContentProgressStatus.Current++;
				this.ContentProgress();
			}
			return translated;
		}
	}
	
	public class NoticeInfo
	{
		public string contents { get; set; }
		public List<string> TrContents { get; set; }
		public string title { get; set; }
		public string TrTitle { get; set; }
		public int created_at { get; set; }
		public int updated_at { get; set; }
		public string date { get; set; }
	}
}
