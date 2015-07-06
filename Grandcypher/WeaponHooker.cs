using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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
		public WeaponHooker()
		{

		}
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
		private void ListDetail(Session oS)
		{
			this.WeaponListLoad();

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
				WeaponInfo temp = new WeaponInfo();

				temp.MasterId = tempIndex.id;
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
			this.LoadingEnd();
		}
		/// <summary>
		/// 덱 편성 화면
		/// </summary>
		/// <param name="oS"></param>
		private void DeckDetail(Session oS)
		{
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
				//if (mainweaponAttribute != deck.Attribute)
				//	deck.SkillType1 = 3;
				if (deck.SkillDetail1.Contains("공격력") && !deck.SkillDetail1.Contains("HP상승"))
					deck.SkillType1 = 1;
				else if (deck.SkillDetail1.Contains("HP상승") && !deck.SkillDetail1.Contains("공격력"))
					deck.SkillType1 = 2;


				//if (mainweaponAttribute != deck.Attribute)
				//	deck.SkillType2 = 3;
				if (deck.SkillDetail2.Contains("공격력") && !deck.SkillDetail2.Contains("HP상승"))
					deck.SkillType2 = 1;
				else if (deck.SkillDetail2.Contains("HP상승") && !deck.SkillDetail2.Contains("공격력"))
					deck.SkillType2 = 2;

				if (i == 1)
				{
					MainWeapon = deck;
					//mainweaponAttribute = deck.Attribute;
				}
				else WeaponLists.Add(deck);
				this.ProgressStatus.Current++;
				this.ProgressBar();
			}

			List<WeaponInfo> CollectedWeapon = WeaponLists.Where(x =>
					(x.SkillDetail1.Contains("공격력 상승") || x.SkillDetail2.Contains("공격력 상승")))
					.ToList();//공격력 상승이라는 키워드를 가진 모든 무기를 로드(마그나 제외)
			CollectedWeapon = CollectedWeapon.Where(x => !x.SkillName1.Contains("방진") && !x.SkillName2.Contains("방진"))
				.ToList();

			List<WeaponInfo> MagnaWeapon = WeaponLists.Where(x =>
					(x.SkillDetail1.Contains("공격력 상승") || x.SkillDetail2.Contains("공격력 상승")))
					.ToList();//마그나 공인 무기를 별도로 분류
			MagnaWeapon = MagnaWeapon.Where(x => x.SkillName1.Contains("방진") || x.SkillName2.Contains("방진"))
				.ToList();

			//List<WeaponInfo> BahaWeapon = WeaponLists.Where(x =>
			//		x.SkillDetail1.Contains("족의 공격력") ||
			//		x.SkillDetail2.Contains("족의 공격력"))
			//		.ToList();//바하무트 무기의 구분
			List<WeaponInfo> UnknownWeapon = CollectedWeapon.Where(x => x.SkillName1.Contains("ATK") || x.SkillName2.Contains("ATK")).ToList();
			UnknownWeapon = UnknownWeapon.Where(x => x.attribute == MasterAttribute).ToList();

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
			};
			int TotalAtt = 0;
			if (MainWeapon.SkillDetail1.Contains("小") || MainWeapon.SkillDetail2.Contains("小"))
			{
				if (MainWeapon.SkillName1.Contains("방진") || MainWeapon.SkillName2.Contains("방진")) SkillCounter.MagnaS++;
				else if (MainWeapon.SkillName1.Contains("ATK") || MainWeapon.SkillName2.Contains("ATK")) SkillCounter.UnknownS++;
				else SkillCounter.Small++;
			}
			else if (MainWeapon.SkillDetail1.Contains("中") || MainWeapon.SkillDetail2.Contains("中"))
			{
				if (MainWeapon.SkillName1.Contains("방진") || MainWeapon.SkillName2.Contains("방진")) SkillCounter.MagnaM++;
				else if (MainWeapon.SkillName1.Contains("ATK") || MainWeapon.SkillName2.Contains("ATK")) SkillCounter.UnknownM++;
				else SkillCounter.Middle++;
			}
			else if (MainWeapon.SkillDetail1.Contains("大") || MainWeapon.SkillDetail2.Contains("大"))
			{
				if (MainWeapon.SkillName1.Contains("방진") || MainWeapon.SkillName2.Contains("방진")) SkillCounter.MagnaL++;
				else if (MainWeapon.SkillName1.Contains("ATK") || MainWeapon.SkillName2.Contains("ATK")) SkillCounter.UnknownL++;
				else SkillCounter.Large++;
			}
			if (test.deck.pc.param != null) TotalAtt = test.deck.pc.param.attack;
			if (MainWeapon.SkillName1.Contains("단련의 공인")) SkillCounter.NovelWeaponCount++;
			foreach (var item in WeaponLists)
			{
				if (item.SkillName1.Contains("단련의 공인"))
				{
					SkillCounter.NovelWeaponCount++;
				}
			}
			SkillCounter.TotalAttack = TotalAtt;
			this.DeckLoadingEnd();
		}
	}
	public class Skills
	{
		public int Small { get; set; }
		public int MagnaS { get; set; }
		public int UnknownS { get; set; }
		public int Middle { get; set; }
		public int MagnaM { get; set; }
		public int UnknownM { get; set; }
		public int Large { get; set; }
		public int MagnaL { get; set; }
		public int UnknownL { get; set; }
		public int TotalAttack { get; set; }
		public int NovelWeaponCount { get; set; }
	}
	public class NpcInfo
	{
		public int Masterid { get; set; }
		public string name { get; set; }
		public int attack { get; set; }
	}
	public class WeaponInfo
	{
		public int MasterId { get; set; }
		public int ParamId { get; set; }
		public int attribute { get; set; }
		public string ItemName { get; set; }
		public string SkillName1 { get; set; }
		public int SkillLv1 { get; set; }
		public string SkillDetail1 { get; set; }
		public string SkillName2 { get; set; }
		public int SkillLv2 { get; set; }
		public string SkillDetail2 { get; set; }
		public string Element { get; set; }
		public string Kind { get; set; }
		public int SkillType1 { get; set; }
		public int SkillType2 { get; set; }
		//public int Attribute { get; set; }
		public bool is_used { get; set; }

		//param

	}
}
