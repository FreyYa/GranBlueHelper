using Fiddler;
using Grandcypher.Raw;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Grandcypher
{
	public class NWeaponHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler SkillError;
		public EventHandler AtkError;
		public EventHandler HPError;
		public EventHandler FinishRead;
		#endregion
		public int MaxPage { get; private set; }
		public int CurrentPage { get; private set; }
		public int FirstPage { get; private set; }
		public bool EnableListEdit { get; set; }
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		#region CurrentMode
		private ReadMode _CurrentMode;
		public ReadMode CurrentMode
		{
			get { return this._CurrentMode; }
			set
			{
				if (this._CurrentMode == value) return;
				this._CurrentMode = value;
				GrandcypherClient.Current.PostMan("읽기 모드를 전환합니다: " + value.ToString());
				_WeaponList = new Dictionary<int, List<Weapon>>();
			}
		}
		#endregion

		private Dictionary<int, List<Weapon>> _WeaponList { get; set; }
		public List<Weapon> WeaponList { get; private set; }
		public NWeaponHooker()
		{
			this._WeaponList = new Dictionary<int, List<Weapon>>();
			this.WeaponList = new List<Weapon>();
			EnableListEdit = false;
			this.LocalListLoad();
		}
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/weapon/list") && oS.oResponse.MIMEType == "application/json")
				ListHook(oS);
		}
		private void ListHook(Session oS)
		{
			if (!EnableListEdit) return;

			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			string options = jsonFull["options"]["status_short_name"].ToString();
			FirstPage = Convert.ToInt32(jsonFull["first"]);
			MaxPage = Convert.ToInt32(jsonFull["last"]);
			CurrentPage = Convert.ToInt32(jsonFull["current"]);
			JArray currentlist = jsonFull["list"] as JArray;
			switch (this.CurrentMode)
			{
				case ReadMode.스킬레벨:
					if (!options.Contains("SLv"))
					//리스트 정렬상태가 스킬레벨이 아닌경우 리턴
					{
						this.SkillError();
						GrandcypherClient.Current.PostMan("무기 목록 정렬을 스킬레벨 기준으로 변경해주시기 바랍니다");
						return;
					}
					break;
				case ReadMode.공격력:
					if (!options.Contains("攻"))
					//리스트 정렬상태가 공격력이 아닌경우 리턴
					{
						this.AtkError();
						GrandcypherClient.Current.PostMan("무기 목록 정렬을 공격력 기준으로 변경해주시기 바랍니다");
						return;
					}
					break;
				case ReadMode.HP:
					if (!options.Contains("H"))
					//리스트 정렬상태가 HP가 아닌경우 리턴
					{
						this.HPError();
						GrandcypherClient.Current.PostMan("무기 목록 정렬을 HP 기준으로 변경해주시기 바랍니다");
						return;
					}
					break;
			}

			if (!_WeaponList.ContainsKey(CurrentPage))
			{
				List<Weapon> temp_weapon_list = new List<Weapon>();
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
					Weapon temp = new Weapon();
					Weapon nulltest = new Weapon();
					temp.master.id = Convert.ToInt32(item["master"]["id"]);
					temp.param.id = Convert.ToInt32(item["param"]["id"]);

					switch (this.CurrentMode)
					{
						case ReadMode.스킬레벨:
							int skill = -1;
							int.TryParse(item["param"]["status"].ToString(), out skill);
							temp.param.SkillLv = skill;
							temp.master.skill1_image = item["master"]["skill1_image"].ToString();
							temp.master.skill2_image = item["master"]["skill2_image"].ToString();
							break;
						case ReadMode.공격력:
							int atk = -1;
							int.TryParse(item["param"]["status"].ToString(), out atk);
							temp.param.AttStatus = atk;
							nulltest = this.WeaponList.Find(x => x.param.id == temp.param.id);
							if (nulltest != null) nulltest.param.AttStatus = atk;
							break;
						case ReadMode.HP:
							int hp = -1;
							int.TryParse(item["param"]["status"].ToString(), out hp);
							temp.param.HPStatus = hp;
							nulltest = this.WeaponList.Find(x => x.param.id == temp.param.id);
							if (nulltest != null) nulltest.param.HPStatus = hp;
							break;
					}

					temp_weapon_list.Add(temp);
				}
				_WeaponList.Add(CurrentPage, temp_weapon_list);
			}
			if (this._WeaponList.Count == MaxPage)
			{
				if (this.CurrentMode == ReadMode.스킬레벨)
				{
					foreach (var item in this._WeaponList)
					{
						this.WeaponList = this.WeaponList.Concat(item.Value).Where(x => x.master.skill1_image != string.Empty || x.master.skill2_image != string.Empty).ToList();
					}
				}
				this.FinishRead();
				GrandcypherClient.Current.PostMan("스킬보유 무기 목록 로드 완료: 총 " + this.WeaponList.Count + "개");
				switch (this.CurrentMode)
				{
					case ReadMode.스킬레벨:
						this.CurrentMode = ReadMode.공격력;
						break;
					case ReadMode.공격력:
						this.CurrentMode = ReadMode.HP;
						break;
					case ReadMode.HP:
						this.EnableListEdit = false;
						this.LocalListSave();
						break;
				}
			}
		}
		private void LocalListSave()
		{
			XmlDocument NewXmlDoc = new XmlDocument();
			NewXmlDoc.AppendChild(NewXmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
			XmlNode Source = NewXmlDoc.CreateElement("", "Weapons", "");
			NewXmlDoc.AppendChild(Source);

			if (!Directory.Exists(Path.Combine(MainFolder, "Data")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Data"));

			NewXmlDoc.Save(Path.Combine(MainFolder, "Data", "WeaponData.xml"));

			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.Load(Path.Combine(MainFolder, "Data", "WeaponData.xml"));
			foreach (var item in WeaponList)
			{
				XmlNode FristNode = XmlDoc.DocumentElement;

				XmlElement root = XmlDoc.CreateElement("Weapon");
				root.SetAttribute("MasterID", item.master.id.ToString());
				root.SetAttribute("ParamID", item.param.id.ToString());
				root.SetAttribute("SkillLv", item.param.SkillLv.ToString());
				root.SetAttribute("SkillRaw1", item.master.Skill1.RawData);
				root.SetAttribute("SkillRaw2", item.master.Skill2.RawData);
				root.SetAttribute("AttackStatus", item.param.AttStatus.ToString());
				root.SetAttribute("HPStatus", item.param.HPStatus.ToString());

				FristNode.AppendChild(root);
			}
			XmlDoc.Save(Path.Combine(MainFolder, "Data", "WeaponData.xml"));
		}
		private void LocalListLoad()
		{

		}
	}
	public enum ReadMode
	{
		스킬레벨,
		공격력,
		HP,
	}
}
