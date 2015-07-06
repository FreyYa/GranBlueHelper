using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Grandcypher
{
	//http://weblog.west-wind.com/posts/2012/Aug/30/Using-JSONNET-for-dynamic-JSON-parsing
	public class ScenarioHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler ScenarioStart;
		public EventHandler TranslatieEnd;
		public EventHandler ProgressBar;
		#endregion
		public List<Scenario> ScenarioList { get; private set; }
		public LimitedValue ProgressStatus { get; set; }
		public TranslateKind TranslateSite { get; set; }
		public string PathName { get; private set; }
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		public ScenarioHooker()
		{

		}
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/quest/scenario") && oS.oResponse.MIMEType == "application/json")
				QuestDetail(oS);
			if (oS.PathAndQuery.StartsWith("/coopraid/scenario") && oS.oResponse.MIMEType == "application/json")
				QuestDetail(oS);
		}
		private void QuestDetail(Session oS)
		{
			this.ScenarioStart();

			var path = oS.PathAndQuery.Split('?');
			PathName = path[0];
			PathName = PathName.Substring(1, path[0].Count() - 1);
			PathName = PathName.Replace("/", "_");

			if (this.ScenarioList == null)
				this.ScenarioList = new List<Scenario>();
			else
			{
				this.ScenarioList.Clear();
				this.ScenarioList.TrimExcess();

			}

			JArray jsonVal = JArray.Parse(oS.GetResponseBodyAsString()) as JArray;
			dynamic Details = jsonVal;
			this.ProgressStatus = new LimitedValue();
			this.ProgressStatus.Max = Details.Count;
			this.ProgressStatus.Min = 0;
			this.ProgressStatus.Current = 0;

			this.ProgressBar();
			string TranslateDir = "Google";
			if (TranslateSite == TranslateKind.Naver) TranslateDir = "Naver";

			if (File.Exists(Path.Combine(MainFolder, "Translations", "Scenarios", TranslateDir, PathName + ".xml")))
			{
				foreach (var detail in Details)
				{
					string dtl = detail.detail;
					//string sel1 = detail.sel1_txt;
					//string sel2 = detail.sel2_txt;

					Scenario temp = new Scenario
					{
						context = RemoveWebTag(dtl),
						index = detail.id,
					};
					if (detail.charcter1_name != "null") temp.Name = detail.charcter1_name;
					else temp.Name = "";

					temp.TrName = GrandcypherClient.Current.Translations.ReplaceTranslation(temp.Name);


					temp.TrContext = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.ScenarioDetail, temp.context);
					//if (sel1 != null)
					//{
					//	temp.sel1_txt = RemoveWebTag(sel1);
					//	temp.Trsel1 = Translator(temp.sel1_txt, TranslateSite);
					//}
					//if (sel2 != null)
					//{
					//	temp.sel2_txt = RemoveWebTag(sel2);
					//	temp.Trsel2 = Translator(temp.sel2_txt, TranslateSite);
					//}

					ScenarioList.Add(temp);

					this.ProgressStatus.Current++;
					this.ProgressBar();
				}
				this.TranslatieEnd();
			}
			else
			{
				foreach (dynamic detail in Details)
				{
					string dtl = detail.detail;
					string sel1 = detail.sel1_txt;
					string sel2 = detail.sel2_txt;
					Scenario temp = new Scenario
					{
						context = RemoveWebTag(dtl),
						index = detail.id,
					};
					if (ScenarioList.Count == 0) temp.PathName = PathName;

					if (detail.charcter1_name != "null") temp.Name = detail.charcter1_name;
					else temp.Name = "";


					temp.TrName = GrandcypherClient.Current.Translations.ReplaceTranslation(temp.Name);

					temp.TrContext = GrandcypherClient.Current.Translations.ReplaceTranslation(temp.context);
					temp.TrContext = Translator(temp.TrContext, TranslateSite);
					if (temp.TrContext != null) temp.TrContext = temp.TrContext.Replace("정액", "");

					if (sel1 != null)
					{
						temp.sel1_txt = RemoveWebTag(sel1);
						temp.Trsel1 = Translator(temp.sel1_txt, TranslateSite);
					}
					if (sel2 != null)
					{
						temp.sel2_txt = RemoveWebTag(sel2);
						temp.Trsel2 = Translator(temp.sel2_txt, TranslateSite);
					}

					#region Debug
#if DEBUG
				Console.WriteLine("-----------------------------------------------------------");

				Console.WriteLine(temp.Name + " : " + RemoveWebTag(temp.context));
				Console.WriteLine(temp.TrName + " : " + temp.TrContext);
				if (temp.sel1_txt != null)
				{
					Console.WriteLine(temp.sel1_txt);
					Console.WriteLine(temp.Trsel1);

				}
				if (temp.sel2_txt != null)
				{
					Console.WriteLine(temp.sel2_txt);
					Console.WriteLine(temp.Trsel2);
				}
#endif
					#endregion

					ScenarioList.Add(temp);

					this.ProgressStatus.Current++;
					this.ProgressBar();
				}
				this.TranslatieEnd();
				GrandcypherClient.Current.Translations.WriteFile(ScenarioList, TranslateSite);
			}
		}
		public string Translator(string input, TranslateKind kind)
		{

			switch (kind)
			{
				case TranslateKind.Naver:
					return NaverTranslator(input);
				case TranslateKind.Google:
					return GoogleTranslator(input);
				case TranslateKind.InfoSeek:
					break;
				default:
					break;
			}
			return string.Empty;
		}
		private string GoogleTranslator(string input)
		{
			if (input.Count() > 0)
			{
				try
				{
					bool SpanRemain = true;

					string url = String.Format("http://www.google.com/translate_t?hl=ko&ie=UTF8&text={0}&langpair=ja|ko", input);

					WebClient webClient = new WebClient();

					string result = webClient.DownloadString(url);

					int temp1 = result.IndexOf("id=result_box") - 6;//span시작

					result = result.Substring(result.IndexOf("id=result_box") - 6, result.Count() - temp1);//span시작부터 문서끝까지
					result = result.Substring(0, result.IndexOf("</div"));//첫번째 </div가 발견되는 부분 이후를 모두 제거

					result = result.Substring(result.IndexOf(">") + 1, result.Count() - result.IndexOf(">") - 1);
					result = result.Substring(result.IndexOf(">") + 1, result.Count() - result.IndexOf(">") - 1);
					result = result.Replace("</span>", "");
					result = result.Replace("</div>", "");

					while (SpanRemain)
					{
						if (result.Contains("<") || result.Contains(">"))
						{
							int inx = result.IndexOf("<");
							int endinx = result.IndexOf(">") + 1;
							string removetext;

							if (inx != endinx)
							{
								removetext = result.Substring(inx, endinx - inx);
								result = result.Replace(removetext, "");
							}

							if (!result.Contains("<span")) SpanRemain = false;
						}
						else SpanRemain = false;
					}
					return result;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return "구글과의 통신이 원활하지않아 번역이 취소되었습니다.";
				}
			}
			else return null;

		}
		private string NaverTranslator(string input)
		{
			if (input.Count() > 0)
			{
				string url = String.Format("http://jpdic.naver.com/search.nhn?range=all&q={0}&sm=jpd_hty", HttpUtility.UrlEncode(input));

				WebClient webClient = new WebClient();

				webClient.Encoding = Encoding.UTF8;

				string result = webClient.DownloadString(url);

				try
				{
					int temp1 = result.IndexOf("jap_ico") - 14;//span시작

					result = result.Substring(temp1, result.Count() - temp1);//span시작부터 문서끝까지
					result = result.Substring(0, result.IndexOf("</div"));//첫번째 </div가 발견되는 부분 이후를 모두 제거
					result = result.Substring(result.IndexOf("</strong") + 24, result.Count() - result.IndexOf("</strong>") - 24);
					result = result.Substring(0, result.IndexOf("</span"));//첫번째 </div가 발견되는 부분 이후를 모두 제거

					result = result.Replace("\n", "");

					return result;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return "네이버와의 통신이 원활하지않아 번역이 취소되었습니다.";
				}
			}
			else return null;
		}
		public string RemoveWebTag(string rawdata, bool RemoveEnter = true)
		{
			var tempstr = rawdata;
			bool IsEnd = true;

			while (IsEnd)
			{
				int removePointStart = tempstr.IndexOf('<');
				int removePointEnd = tempstr.IndexOf('>');

				if (removePointStart != removePointEnd)
				{
					tempstr = tempstr.Remove(removePointStart, removePointEnd - removePointStart + 1);
					removePointStart = 0;
					removePointEnd = 0;
				}
				if (!tempstr.Contains("<") && !tempstr.Contains(">")) IsEnd = false; ;
			}
			if (RemoveEnter) IsEnd = true;
			while (IsEnd)
			{
				tempstr = tempstr.Replace("\n", "");
				if (!tempstr.Contains("\n")) IsEnd = false;
			}

			return tempstr;
		}
	}
	public class Scenario
	{
		public int index { get; set; }
		public string Name { get; set; }
		public string TrName { get; set; }
		public string context { get; set; }
		public string TrContext { get; set; }
		public string sel1_txt { get; set; }
		public string sel2_txt { get; set; }
		public string Trsel1 { get; set; }
		public string Trsel2 { get; set; }
		public string PathName { get; set; }
		public string synopsis { get; set; }
	}
	public enum TranslateKind
	{
		Naver,
		Google,
		InfoSeek,
	}
	public class LimitedValue
	{
		public int Current { get; set; }
		public int Max { get; set; }
		public int Min { get; set; }
	}
}
