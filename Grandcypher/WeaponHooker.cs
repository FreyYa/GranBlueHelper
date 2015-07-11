using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Grandcypher
{
	public class WeaponHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler WeaponListLoad;
		public EventHandler LoadingEnd;
		public EventHandler DeckLoadingEnd;
		public EventHandler ProgressBar;
		#endregion
		public LimitedValue ProgressStatus { get; set; }
		public List<WeaponInfo> WeaponLists { get; set; }
		public List<NpcInfo> NPCList { get; set; }
		public WeaponInfo MainWeapon { get; set; }
		public Skills SkillCounter { get; set; }
		public List<string> SkillList { get; set; }
		private bool DeckIsEnd { get; set; }
		private bool ListIsEnd { get; set; }
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.StartsWith("/weapon/list") && oS.oResponse.MIMEType == "application/json")
				ListDetail(oS);
			if (oS.PathAndQuery.StartsWith("/weapon/enhancement_materials") && oS.oResponse.MIMEType == "application/json")
				ListDetail(oS);
			if (oS.PathAndQuery.StartsWith("/party/weapon/pc") && oS.oResponse.MIMEType == "application/json")
				ListDetail(oS);
			if (!oS.PathAndQuery.Contains("decks") && oS.PathAndQuery.StartsWith("/party/deck") && oS.oResponse.MIMEType == "application/json")
				DeckDetail(oS);
		}
		/// <summary>
		/// 기본 무기 리스트. 강화/리스트/창고가 포함됨
		/// </summary>
		/// <param name="oS"></param>
		#region List
		private void ListDetail(Session oS)
		{
			this.WeaponListLoad();
			this.ListIsEnd = false;
			this.DeckIsEnd = false;
			if (this.WeaponLists == null)
				this.WeaponLists = new List<WeaponInfo>();
			else
			{
				this.WeaponLists.Clear();
				this.WeaponLists.TrimExcess();
			}
			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			dynamic test = jsonFull;
			dynamic list = test.list;


			List<dynamic> weaponList = new List<dynamic>();

			for (int i = 0; i < list.Count; i++)
			{
				weaponList.Add(list[i]);
			}

			this.ProgressStatus = new LimitedValue();
			this.ProgressStatus.Max = weaponList.Count;
			this.ProgressStatus.Min = 0;
			this.ProgressStatus.Current = 0;

			this.ProgressBar();

			for (int i = 0; i < weaponList.Count; i++)
			{
				dynamic tempIndex = weaponList[i].master;
				dynamic tempparam = weaponList[i].param;
				WeaponInfo temp = new WeaponInfo();

				temp.MasterId = tempIndex.id;
				temp.ParamId = tempparam.id;//무기 스킬레벨등을 저장하고 구별하기 위한 부분
				temp.is_used = weaponList[i].is_used;

				//if (tempIndex.attribute != null)
				//	temp.Attribute = tempIndex.attribute;

				temp.ItemName = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponList, "", TranslateKind.Google, temp.MasterId);
				temp.SkillName1 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.FirstSkillName, "", TranslateKind.Google, temp.MasterId);
				temp.SkillName2 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.LastSkillName, "", TranslateKind.Google, temp.MasterId);
				temp.SkillDetail1 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.FirstSkillDetail, "", TranslateKind.Google, temp.MasterId);
				temp.SkillDetail2 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.LastSkillDetail, "", TranslateKind.Google, temp.MasterId);
				temp.Element = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.Element, "", TranslateKind.Google, temp.MasterId);
				temp.Kind = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponType, "", TranslateKind.Google, temp.MasterId);

				temp.SkillLv1 = WeaponLvLoad(temp.ParamId, 1);
				temp.SkillLv2 = WeaponLvLoad(temp.ParamId, 2);

				if (temp.SkillName1 == string.Empty || temp.SkillName1 == null) temp.vSkillLv1 = Visibility.Collapsed;
				else temp.vSkillLv1 = Visibility.Visible;
				if (temp.SkillName2 == string.Empty || temp.SkillName2 == null) temp.vSkillLv2 = Visibility.Collapsed;
				else temp.vSkillLv2 = Visibility.Visible;

				//1=공격 2=체력Up
				if (temp.SkillDetail1.Contains("공격력") && !temp.SkillDetail1.Contains("HP상승"))
					temp.SkillType1 = 1;
				else if (temp.SkillDetail1.Contains("HP상승") && !temp.SkillDetail1.Contains("공격력"))
					temp.SkillType1 = 2;

				if (temp.SkillDetail2.Contains("공격력") && !temp.SkillDetail2.Contains("HP상승"))
					temp.SkillType2 = 1;
				else if (temp.SkillDetail2.Contains("HP상승") && !temp.SkillDetail2.Contains("공격력"))
					temp.SkillType2 = 2;

				WeaponLists.Add(temp);
				this.ProgressStatus.Current++;
				this.ProgressBar();
			}
			this.ListIsEnd = true;
			this.LoadingEnd();
		}
		#endregion

		/// <summary>
		/// 덱 편성 화면. 공인계산기 포함
		/// </summary>
		/// <param name="oS"></param>
		#region 덱 편성
		private void DeckDetail(Session oS)
		{
			this.ListIsEnd = false;
			this.DeckIsEnd = false;

			this.WeaponListLoad();
			int MasterAttribute = 0;
			if (this.WeaponLists == null)
				this.WeaponLists = new List<WeaponInfo>();
			else
			{
				this.WeaponLists.Clear();
				this.WeaponLists.TrimExcess();
			}

			if (this.NPCList == null)
				this.NPCList = new List<NpcInfo>();
			else
			{
				this.NPCList.Clear();
				this.NPCList.TrimExcess();
			}

			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			dynamic test = jsonFull;
			dynamic list = test.deck.pc.weapons;
			dynamic npcList = test.deck.npc;

			if (test.deck.pc.skill.count > 0)
			{
				this.SkillList = new List<string>();
				dynamic skilllist = test.deck.pc.skill.description;
				foreach (var item in skilllist)
				{
					JProperty SkillProperty = item;
					var Firsts = SkillProperty.First;
					string SkillComment = (string)Firsts["comment"];
					int over = (int)Firsts["over"];

					if (over > 1) SkillList.Add("◎ " + GrandcypherClient.Current.ScenarioHooker.Translator(SkillComment, GrandcypherClient.Current.ScenarioHooker.TranslateSite) + " + " + over.ToString());
					else SkillList.Add("◎ " + GrandcypherClient.Current.ScenarioHooker.Translator(SkillComment, GrandcypherClient.Current.ScenarioHooker.TranslateSite));
				}
			}
			if (test.deck.pc.param.attribute != null)
			{
				MasterAttribute = Convert.ToInt32(test.deck.pc.param.attribute);
			}
			this.ProgressStatus = new LimitedValue();
			this.ProgressStatus.Max = 15;
			this.ProgressStatus.Min = 0;
			this.ProgressStatus.Current = 0;

			//캐릭터의 속성을 구별할 방법이 외부 DB를 추가로 만드는 방법밖에 없기때문에 일단 모두 같은 속성으로 간주. 
			//즉 디 오더 그랑데나 속성 불일치 종족파티를 쓰는 유저는 이 계산기를 사용할수없음
			for (int i = 1; i < 6; i++)
			{
				NpcInfo npc = new NpcInfo();
				string temp = i.ToString();
				JObject jobject = (JObject)npcList[temp];
				JObject param;
				JObject master;
				try
				{
					param = (JObject)jobject["param"];
					master = (JObject)jobject["master"];
				}
				catch
				{
					this.NPCList.Add(npc);
					continue;
				}
				npc.name = (string)master["name"];
				npc.attack = (int)param["attack"];
				this.NPCList.Add(npc);
				this.ProgressStatus.Current++;
				this.ProgressBar();
			}

			for (int i = 1; i < 11; i++)
			{
				WeaponInfo deck = new WeaponInfo();
				string temp = i.ToString();
				JObject jobject = (JObject)list[temp];
				JObject master;
				JObject param;
				try
				{
					master = (JObject)jobject["master"];
					param = (JObject)jobject["param"];
				}
				catch
				{
					deck.vSkillLv1 = Visibility.Collapsed;
					deck.vSkillLv2 = Visibility.Collapsed;
					WeaponLists.Add(deck);
					continue;
				}
				deck.MasterId = (int)master["id"];
				deck.ParamId = (int)param["id"];//무기 스킬레벨등을 저장하고 구별하기 위한 부분
				//if (master["attribute"].ToString() != "")
				//	deck.Attribute = (int)master["attribute"];

				deck.ItemName = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponList, "", TranslateKind.Google, deck.MasterId);

				deck.SkillName1 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.FirstSkillName, "", TranslateKind.Google, deck.MasterId);
				//if (jobject["skill1"].HasValues)
				//	deck.SkillDetail1 = (string)jobject["skill1"]["description"];
				deck.attribute = (int)master["attribute"];
				deck.SkillName2 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.LastSkillName, "", TranslateKind.Google, deck.MasterId);
				//if (jobject["skill2"].HasValues)
				//	deck.SkillDetail2 = (string)jobject["skill2"]["description"];

				deck.SkillDetail1 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.FirstSkillDetail, "", TranslateKind.Google, deck.MasterId);
				deck.SkillDetail2 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.LastSkillDetail, "", TranslateKind.Google, deck.MasterId);
				deck.Element = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.Element, "", TranslateKind.Google, deck.MasterId);
				deck.Kind = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponType, "", TranslateKind.Google, deck.MasterId);
				//1=공격 2=체력Up 
				if (deck.SkillDetail1.Contains("공격력") && !deck.SkillDetail1.Contains("HP상승"))
					deck.SkillType1 = 1;
				else if (deck.SkillDetail1.Contains("HP상승") && !deck.SkillDetail1.Contains("공격력"))
					deck.SkillType1 = 2;


				if (deck.SkillDetail2.Contains("공격력") && !deck.SkillDetail2.Contains("HP상승"))
					deck.SkillType2 = 1;
				else if (deck.SkillDetail2.Contains("HP상승") && !deck.SkillDetail2.Contains("공격력"))
					deck.SkillType2 = 2;
				deck.SkillLv1 = WeaponLvLoad(deck.ParamId, 1);
				deck.SkillLv2 = WeaponLvLoad(deck.ParamId, 2);

				if (deck.SkillName1 != string.Empty && deck.SkillName1 != null) deck.vSkillLv1 = Visibility.Visible;
				else deck.vSkillLv1 = Visibility.Collapsed;
				if (deck.SkillName2 != string.Empty && deck.SkillName2 != null) deck.vSkillLv2 = Visibility.Visible;
				else deck.vSkillLv2 = Visibility.Collapsed;
				if (i == 1)
				{
					MainWeapon = deck;
				}
				else WeaponLists.Add(deck);
				this.ProgressStatus.Current++;
				this.ProgressBar();
			}

			List<WeaponInfo> CollectedWeapon = new List<WeaponInfo>(WeaponLists);
			CollectedWeapon.Add(MainWeapon);


			CollectedWeapon = CollectedWeapon.Where(x =>
					(x.SkillDetail1 != null || x.SkillDetail2 != null) && ((x.SkillDetail1.Contains("공격력 상승") || x.SkillDetail2.Contains("공격력 상승"))))
					.ToList();//공격력 상승이라는 키워드를 가진 모든 무기를 로드(마그나 제외)
			CollectedWeapon = CollectedWeapon.Where(x => !x.SkillName1.Contains("방진") && !x.SkillName2.Contains("방진"))
				.ToList();

			List<WeaponInfo> MagnaWeapon = WeaponLists.Where(x =>
					(x.SkillDetail1 != null || x.SkillDetail2 != null) && ((x.SkillDetail1.Contains("공격력 상승") || x.SkillDetail2.Contains("공격력 상승"))))
					.ToList();//마그나 공인 무기를 별도로 분류
			MagnaWeapon = MagnaWeapon.Where(x => x.SkillName1.Contains("방진") || x.SkillName2.Contains("방진"))
				.ToList();

			List<WeaponInfo> UnknownWeapon = CollectedWeapon.Where
				(x => x.SkillName1.Contains("ATK") || x.SkillName2.Contains("ATK")).ToList();
			UnknownWeapon = UnknownWeapon.Where(x => x.attribute == MasterAttribute).ToList();//언노운 분류

			List<WeaponInfo> StrangthWeapon = CollectedWeapon.Where
				(x => x.SkillName1.Contains("스트렝스") || x.SkillName2.Contains("스트렝스")).ToList();
			StrangthWeapon = StrangthWeapon.Where(x => x.attribute == MasterAttribute).ToList();//스트렝스 분류


			switch (MasterAttribute)//일반 공인 분류
			{
				case 1://화
					CollectedWeapon = CollectedWeapon.Where(x =>
						x.SkillDetail1.Contains("화속성") ||
						x.SkillDetail2.Contains("화속성"))
						.ToList();
					break;
				case 2://수
					CollectedWeapon = CollectedWeapon.Where(x =>
						x.SkillDetail1.Contains("수속성") ||
						x.SkillDetail2.Contains("수속성"))
						.ToList();
					break;
				case 3://토
					CollectedWeapon = CollectedWeapon.Where(x =>
						x.SkillDetail1.Contains("토속성") ||
						x.SkillDetail2.Contains("토속성"))
						.ToList();
					break;
				case 4://풍
					CollectedWeapon = CollectedWeapon.Where(x =>
						x.SkillDetail1.Contains("풍속성") ||
						x.SkillDetail2.Contains("풍속성"))
						.ToList();
					break;
				case 5://광
					CollectedWeapon = CollectedWeapon.Where(x =>
						x.SkillDetail1.Contains("빛속성") ||
						x.SkillDetail2.Contains("빛속성"))
						.ToList();
					break;
				case 6://암
					CollectedWeapon = CollectedWeapon.Where(x =>
						x.SkillDetail1.Contains("암속성") ||
						x.SkillDetail2.Contains("암속성"))
						.ToList();
					break;
			}
			switch (MasterAttribute)//일반 공인 분류
			{
				case 1://화
					MagnaWeapon = MagnaWeapon.Where(x =>
						x.SkillDetail1.Contains("화속성") ||
						x.SkillDetail2.Contains("화속성"))
						.ToList();
					break;
				case 2://수
					MagnaWeapon = MagnaWeapon.Where(x =>
						x.SkillDetail1.Contains("수속성") ||
						x.SkillDetail2.Contains("수속성"))
						.ToList();
					break;
				case 3://토
					MagnaWeapon = MagnaWeapon.Where(x =>
						x.SkillDetail1.Contains("토속성") ||
						x.SkillDetail2.Contains("토속성"))
						.ToList();
					break;
				case 4://풍
					MagnaWeapon = MagnaWeapon.Where(x =>
						x.SkillDetail1.Contains("풍속성") ||
						x.SkillDetail2.Contains("풍속성"))
						.ToList();
					break;
				case 5://광
					MagnaWeapon = MagnaWeapon.Where(x =>
						x.SkillDetail1.Contains("빛속성") ||
						x.SkillDetail2.Contains("빛속성"))
						.ToList();
					break;
				case 6://암
					MagnaWeapon = MagnaWeapon.Where(x =>
						x.SkillDetail1.Contains("암속성") ||
						x.SkillDetail2.Contains("암속성"))
						.ToList();
					break;
			}


			var little = CollectedWeapon.Where(x =>//1퍼센트
						x.SkillDetail1.Contains("小") ||
						x.SkillDetail2.Contains("小"))
						.ToList();
			var middle = CollectedWeapon.Where(x =>//3퍼센트
						x.SkillDetail1.Contains("中") ||
						x.SkillDetail2.Contains("中"))
						.ToList();
			var large = CollectedWeapon.Where(x =>//6퍼센트
						x.SkillDetail1.Contains("大") ||
						x.SkillDetail2.Contains("大"))
						.ToList();

			var Mlittle = MagnaWeapon.Where(x =>//1퍼센트
						x.SkillDetail1.Contains("小") ||
						x.SkillDetail2.Contains("小"))
						.ToList();
			var Mmiddle = MagnaWeapon.Where(x =>//3퍼센트
						x.SkillDetail1.Contains("中") ||
						x.SkillDetail2.Contains("中"))
						.ToList();
			var Mlarge = MagnaWeapon.Where(x =>//6퍼센트
						x.SkillDetail1.Contains("大") ||
						x.SkillDetail2.Contains("大"))
						.ToList();

			var Ulittle = UnknownWeapon.Where(x =>//1퍼센트
						x.SkillDetail1.Contains("小") ||
						x.SkillDetail2.Contains("小"))
						.ToList();
			var Umiddle = UnknownWeapon.Where(x =>//3퍼센트
						x.SkillDetail1.Contains("中") ||
						x.SkillDetail2.Contains("中"))
						.ToList();
			var Ularge = UnknownWeapon.Where(x =>//6퍼센트
						x.SkillDetail1.Contains("大") ||
						x.SkillDetail2.Contains("大"))
						.ToList();


			var Slittle = StrangthWeapon.Where(x =>//1퍼센트
						x.SkillDetail1.Contains("小") ||
						x.SkillDetail2.Contains("小"))
						.ToList();
			var Smiddle = StrangthWeapon.Where(x =>//3퍼센트
						x.SkillDetail1.Contains("中") ||
						x.SkillDetail2.Contains("中"))
						.ToList();
			var Slarge = StrangthWeapon.Where(x =>//6퍼센트
						x.SkillDetail1.Contains("大") ||
						x.SkillDetail2.Contains("大"))
						.ToList();


			var normalSum = little.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			normalSum += little.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			normalSum += middle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			normalSum += middle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			normalSum += large.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			normalSum += large.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			var MagnaSum = Mlittle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			MagnaSum += Mlittle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			MagnaSum += Mmiddle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			MagnaSum += Mmiddle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			MagnaSum += Mlarge.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			MagnaSum += Mlarge.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			var UnknownSum = Ulittle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			UnknownSum += Ulittle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			UnknownSum += Umiddle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			UnknownSum += Umiddle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			UnknownSum += Ularge.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			UnknownSum += Ularge.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			var StrangthSum = Slittle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			StrangthSum += Slittle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			StrangthSum += Smiddle.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			StrangthSum += Smiddle.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			StrangthSum += Slarge.Where(x => x.SkillName1 != string.Empty).Sum(y => y.SkillLv1 - 1);
			StrangthSum += Slarge.Where(x => x.SkillName2 != string.Empty).Sum(y => y.SkillLv2 - 1);

			//소/중/대 공인으로 분류
			SkillCounter = new Skills
			{
				Small = little.Count,
				UnknownS = Ulittle.Count,
				MagnaS = Mlittle.Count,
				Middle = middle.Count,
				UnknownM = Umiddle.Count,
				MagnaM = Mmiddle.Count,
				Large = large.Count,
				UnknownL = Ularge.Count,
				MagnaL = Mlarge.Count,
				StrS = Slittle.Count,
				StrM = Smiddle.Count,
				StrL = Slarge.Count,
				NormalSkillLvCount = normalSum,
				MagnaSkillLvCount = MagnaSum,
				UnknownSkillLvCount = UnknownSum,
				StrangthSkillCount = StrangthSum,
			};

			int TotalAtt = 0;
			
			if (test.deck.pc.param != null) TotalAtt = test.deck.pc.param.attack;
			if (MainWeapon.SkillName1.Contains("단련의 공인"))
			{
				SkillCounter.NovelWeaponCount++;
				MainWeapon.vSkillLv1 = Visibility.Collapsed;
				MainWeapon.vSkillLv2 = Visibility.Collapsed;
			}
			foreach (var item in WeaponLists)
			{
				if (item.SkillName1 != null && item.SkillName1.Contains("단련의 공인"))
				{
					SkillCounter.NovelWeaponCount++;
					item.vSkillLv1 = Visibility.Collapsed;
					item.vSkillLv2 = Visibility.Collapsed;
				}
			}
			SkillCounter.TotalAttack = TotalAtt;
			this.DeckIsEnd = true;
			this.DeckLoadingEnd();
		}
		public void Reload()
		{
			if (this.DeckIsEnd) this.DeckLoadingEnd();
			if (this.ListIsEnd) this.LoadingEnd();
		}
		public int WeaponLvLoad(int Id, int order)
		{
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			if (!Directory.Exists(Path.Combine(MainFolder, "Bin")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Bin"));

			var binPath = Path.Combine(MainFolder, "Bin", "WeaponSkills.bin");
			if (File.Exists(binPath))
			{
				var items = new Dictionary<int, SkillLvTable>();

				var bytes = File.ReadAllBytes(binPath);
				using (var memoryStream = new MemoryStream(bytes))
				using (var reader = new BinaryReader(memoryStream))
				{
					while (memoryStream.Position < memoryStream.Length)
					{
						int paramID = reader.ReadInt32();
						var item = new SkillLvTable
						{
							SkillLv1 = reader.ReadInt32(),
							SkillLv2 = reader.ReadInt32(),
						};
						items.Add(paramID, item);
					}
					memoryStream.Dispose();
					memoryStream.Close();
					reader.Dispose();
					reader.Close();
				}
				if (items.ContainsKey(Id))
				{
					if (order == 1) return items[Id].SkillLv1;
					else return items[Id].SkillLv2;
				}
			}
			return 1;
		}
		public void WeaponLvSave(int Id, int order, int Lv)
		{
			if (!this.DeckIsEnd && !this.ListIsEnd) return;
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			if (!Directory.Exists(Path.Combine(MainFolder, "Bin")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Bin"));

			var binPath = Path.Combine(MainFolder, "Bin", "WeaponSkills.bin");

			var items = new Dictionary<int, SkillLvTable>();

			if (File.Exists(binPath))
			{
				var bytes = File.ReadAllBytes(binPath);
				using (var memoryStream = new MemoryStream(bytes))
				using (var reader = new BinaryReader(memoryStream))
				{
					while (memoryStream.Position < memoryStream.Length)
					{
						int paramID = reader.ReadInt32();
						var item = new SkillLvTable
						{
							SkillLv1 = reader.ReadInt32(),
							SkillLv2 = reader.ReadInt32(),
						};
						items.Add(paramID, item);
					}
					memoryStream.Dispose();
					memoryStream.Close();
					reader.Dispose();
					reader.Close();
				}
				//파일 읽기 종료

				if (items.ContainsKey(Id))//키를 가지고 있으면 해당 키에 해당하는 레벨을 조정
				{
					if (order == 1) items[Id].SkillLv1 = Lv;
					else items[Id].SkillLv2 = Lv;
				}
				else
				{
					var temp = new SkillLvTable();

					if (order == 1)
					{
						temp.SkillLv1 = Lv;
					}
					else
					{
						temp.SkillLv2 = Lv;
					}
					items.Add(Id, temp);
				}
			}
			else
			{
				var temp = new SkillLvTable();

				if (order == 1)
				{
					temp.SkillLv1 = Lv;
					if (temp.SkillLv2 < 1 || temp.SkillLv2 > 10) temp.SkillLv2 = 1;
				}
				else
				{
					temp.SkillLv2 = Lv;
					if (temp.SkillLv1 < 1 || temp.SkillLv1 > 10) temp.SkillLv1 = 1;
				}

				items.Add(Id, temp);
			}

			using (var fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write, FileShare.None))
			using (var writer = new BinaryWriter(fileStream))
			{
				foreach (var item in items)
				{
					writer.Write(item.Key);
					writer.Write(item.Value.SkillLv1);
					writer.Write(item.Value.SkillLv2);
				}
				fileStream.Dispose();
				fileStream.Close();
				writer.Dispose();
				writer.Close();
			}
		}
		#endregion
	}
	public class SkillLvTable
	{
		public int SkillLv1 { get; set; }
		public int SkillLv2 { get; set; }
	}
	public class Skills
	{
		public int TotalAttack { get; set; }
		public int NovelWeaponCount { get; set; }

		//normal
		public int Small { get; set; }
		public int Middle { get; set; }
		public int Large { get; set; }

		//unknown
		public int UnknownS { get; set; }
		public int UnknownM { get; set; }
		public int UnknownL { get; set; }

		//magna
		public int MagnaS { get; set; }
		public int MagnaM { get; set; }
		public int MagnaL { get; set; }

		//strength
		public int StrS { get; set; }
		public int StrM { get; set; }
		public int StrL { get; set; }

		public int NormalSkillLvCount { get; set; }
		public int MagnaSkillLvCount { get; set; }
		public int UnknownSkillLvCount { get; set; }
		public int StrangthSkillCount { get; set; }
	}
	public class NpcInfo
	{
		public int Masterid { get; set; }
		public string name { get; set; }
		public int attack { get; set; }
		public int CalcAtt { get; set; }
	}
	public class WeaponInfo
	{
		public int MasterId { get; set; }
		public int ParamId { get; set; }
		public int attribute { get; set; }
		public string ItemName { get; set; }
		public string SkillName1 { get; set; }
		public string SkillDetail1 { get; set; }
		public string SkillName2 { get; set; }
		public string SkillDetail2 { get; set; }
		public string Element { get; set; }
		public string Kind { get; set; }
		public int SkillType1 { get; set; }
		public int SkillType2 { get; set; }
		//public int Attribute { get; set; }
		public bool is_used { get; set; }

		private int _SkillLv1;
		public int SkillLv1
		{
			get { return this._SkillLv1; }
			set
			{
				if (this._SkillLv1 == value) return;
				this._SkillLv1 = value;
				if (this._SkillLv1 < 1 || this._SkillLv1 > 10) this._SkillLv1 = 1;
				GrandcypherClient.Current.WeaponHooker.Reload();
				GrandcypherClient.Current.WeaponHooker.WeaponLvSave(this.ParamId, 1, this._SkillLv1);
			}
		}
		public Visibility vSkillLv1 { get; set; }
		private int _SkillLv2;
		public int SkillLv2
		{
			get { return this._SkillLv2; }
			set
			{
				if (this._SkillLv2 == value) return;
				this._SkillLv2 = value;
				if (this._SkillLv2 < 1 || this._SkillLv2 > 10) this._SkillLv2 = 1;
				GrandcypherClient.Current.WeaponHooker.Reload();
				GrandcypherClient.Current.WeaponHooker.WeaponLvSave(this.ParamId, 2, this._SkillLv2);
			}
		}
		public Visibility vSkillLv2 { get; set; }
		//param

	}
}
