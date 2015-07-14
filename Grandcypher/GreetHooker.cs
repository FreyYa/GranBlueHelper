using Fiddler;
using Grandcypher.Raw;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Grandcypher
{
	public class GreetHooker
	{
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler ReadStart;
		public EventHandler TranslatieEnd;
		public EventHandler ProgressBar;
		#endregion
		public LimitedValue ProgressStatus { get; set; }
		public List<UserIf> GreetList { get; set; }
		public GreetHooker()
		{

		}
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/profile/greet_list") && oS.oResponse.MIMEType == "application/json")
				GreetHook(oS);
			if (oS.PathAndQuery.StartsWith("/guild_main/comment_list") && oS.oResponse.MIMEType == "application/json")
				GreetHook(oS, true);
			if (oS.PathAndQuery.StartsWith("/guild/main/comment_list") && oS.oResponse.MIMEType == "application/json")
				GreetHook(oS, true);
			else if (oS.PathAndQuery.StartsWith("/guild_main/comment") && oS.oResponse.MIMEType == "application/json")
				GreetHook(oS, true);

		}
		private void GreetHook(Session oS, bool IsGuildMessage = false)
		{
			this.ReadStart();
			GreetList = new List<UserIf>();
			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;

			dynamic test = jsonFull;
			dynamic list = test.list;
			int MaxCount = 0;
			foreach (var item in list)
			{
				MaxCount++;
			}
			this.ProgressStatus = new LimitedValue();
			this.ProgressStatus.Max = MaxCount;
			this.ProgressStatus.Min = 0;
			this.ProgressStatus.Current = 0;

			foreach (var item in list)
			{
				UserIf temp = new UserIf();
				temp.UserComment = new UserComment();
				string userName, userComment, userLevel, userTime;

				JProperty SkillProperty = item;
				var Firsts = SkillProperty.First;
				if (IsGuildMessage)
				{
					userName = "user_name";
					userComment = "user_comment";
					userLevel = "user_level";
					userTime = "chat_time";
				}
				else
				{
					userName = "from_user_name";
					userComment = "from_user_comment";
					userLevel = "from_user_level";
					userTime = "from_greet_time";
				}
				temp.from_user_name = (string)Firsts[userName];
				int userlevelint = (int)Firsts[userLevel];
				temp.from_greet_time = (string)Firsts[userTime];
				temp.from_user_level = "Rank " + userlevelint.ToString();
				temp.UserComment.text = (string)Firsts[userComment]["text"];
				temp.UserComment.is_stamp = (bool)Firsts[userComment]["is_stamp"];
				if (temp.UserComment.is_stamp)
				{
					temp.UserComment = ReturnStampImage(temp.UserComment);
				}
				temp.UserComment.text = GrandcypherClient.Current.ScenarioHooker.RemoveWebTag(temp.UserComment.text);
				temp.UserComment.TrText = GrandcypherClient.Current.ScenarioHooker.Translator(temp.UserComment.text, GrandcypherClient.Current.ScenarioHooker.TranslateSite);

				GreetList.Add(temp);
#if DEBUG
				Console.WriteLine("=======================================================");
				Console.WriteLine(temp.from_user_name + " : " + temp.UserComment.text);
				Console.WriteLine(temp.from_user_name + " : " + temp.UserComment.TrText);
				Console.WriteLine("Stamp Url: " + temp.UserComment.StampUrl);
#endif
				this.ProgressStatus.Current++;
				this.ProgressBar();
			}
			this.TranslatieEnd();
		}
		/// <summary>
		/// 이미지파일을 인터넷에서 읽어옵니다
		/// http://jmsoft.tistory.com/47
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FileDownloader(string uri, string StampFileName)
		{
			WebRequest req = WebRequest.Create(new Uri(uri));

			WebResponse result = req.GetResponse();


			Stream ReceiveStream = result.GetResponseStream();

			Byte[] read = new Byte[512];

			int bytes = ReceiveStream.Read(read, 0, 512);

			Encoding encode;
			encode = System.Text.Encoding.Default;
			if (!Directory.Exists(Path.Combine(MainFolder, "Stamps"))) Directory.CreateDirectory(Path.Combine(MainFolder, "Stamps"));
			if (!File.Exists(Path.Combine(MainFolder, "Stamps", StampFileName)))
			{
				FileStream FileStr = new FileStream(Path.Combine(MainFolder, "Stamps", StampFileName), FileMode.OpenOrCreate, FileAccess.Write);

				while (bytes > 0)
				{
					// 버퍼의 데이터를 사용하여 이 스트림에 바이트 블록을 씀
					FileStr.Write(read, 0, bytes);
					bytes = ReceiveStream.Read(read, 0, 512);

				}
				BinaryWriter Savefile = new BinaryWriter(FileStr, encode);
				Savefile.Close();
			}
		}
		private UserComment ReturnStampImage(UserComment rawData)
		{
			UserComment UserCommentTemp = new UserComment();
			UserCommentTemp = rawData;

			int urlPointStart = UserCommentTemp.text.IndexOf("src=\"") + 5;
			int urlPointEnd = UserCommentTemp.text.IndexOf(".png\"") + 4;
			UserCommentTemp.StampUrl = UserCommentTemp.text.Substring(urlPointStart, urlPointEnd - urlPointStart);

			try
			{
				int urlFIlenameStart = UserCommentTemp.StampUrl.IndexOf("full") + 5;
				int urlFIlenameEnd = UserCommentTemp.StampUrl.IndexOf(".png");

				UserCommentTemp.StampFileName = UserCommentTemp.StampUrl.Substring(urlFIlenameStart, urlFIlenameEnd - urlFIlenameStart) + ".png";
			}
			catch
			{
				UserCommentTemp.StampFileName = string.Empty;
			}

			try
			{
				FileDownloader(UserCommentTemp.StampUrl, UserCommentTemp.StampFileName);
			}
			catch (Exception ex)
			{
				GrandcypherClient.Current.PostMan("스탬프 이미지를 받아오는데 문제가 발생하였습니다.");
				Debug.WriteLine(ex);
			}

			return UserCommentTemp;
		}
	}
}
