using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grandcypher
{
	public enum BulletKind
	{
		Parabellum,
		Rifle,
		Cartridge,
		Ethereal,
	}
	public class BulletMaster
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler ItemReadStart;
		public EventHandler ItemReadEnd;
		#endregion
		public Dictionary<string, TreasureInfo> CurrentItemList { get; set; }
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.Contains("/item/article_list/") && oS.oResponse.MIMEType == "application/json")
				CountItem(oS);
		}
		private void CountItem(Session session)
		{
			int i = 0;
			//리스트는 십천중 계산기 모듈이 목록을 다 받아오면 그때 이벤트를 발생시킨다
			while (i<1000)
			{
				if (GrandcypherClient.Current.TreasureHooker.LoadingEnd)
					break;
				Debug.WriteLine(i);
				i++;
			}
			//MessageBox.Show("리스트 로드 완료");
			this.ItemReadEnd();
		}
		private void Logger(string str, params object[] args)
		{
			byte[] utf8Bom = { 0xEF, 0xBB, 0xBF };
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

			if (!System.IO.File.Exists(MainFolder + "\\ItemList.csv"))
			{
				var csvPath = Path.Combine(MainFolder, "ItemList.csv");
				using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
				using (var writer = new BinaryWriter(fileStream))
				{
					writer.Write(utf8Bom);
				}
				using (StreamWriter w = File.AppendText(MainFolder + "\\ItemList.csv"))
				{
					w.WriteLine("아이템이름,ID", args);
				}
			}

			using (StreamWriter w = File.AppendText(MainFolder + "\\ItemList.csv"))
			{
				w.WriteLine(str, args);
			}
		}

	}
}
