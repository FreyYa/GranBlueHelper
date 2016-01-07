﻿using Fiddler;
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
		public Dictionary<int, WeaponInfo> MasterBinList { get; set; }
		public List<WeaponInfo> WeaponLists { get; set; }
		public List<NpcInfo> NPCList { get; set; }
		public WeaponInfo MainWeapon { get; set; }
		public Skills SkillCounter { get; set; }
		public List<string> SkillList { get; set; }

		public bool IsConcilioExist { get; private set; }
		public bool IsVisExist { get; private set; }

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
			if (oS.PathAndQuery.StartsWith("/npc/enhancement_materials") && oS.oResponse.MIMEType == "application/json")
				ListDetail(oS);
			if (oS.PathAndQuery.StartsWith("/weapon/weapon") && oS.oResponse.MIMEType == "application/json")
				WeaponDetail(oS, false);
			if (oS.PathAndQuery.StartsWith("/weapon/weapon_base") && oS.oResponse.MIMEType == "application/json")
				WeaponDetail(oS, false);
			if (oS.PathAndQuery.StartsWith("/enhancement_weapon/enhancement") && oS.oResponse.MIMEType == "application/json")
				WeaponDetail(oS);
		}
		/// <summary>
		/// 무기 정보에 접근할때 자동적으로 무기의 레벨을 저장한다.
		/// </summary>
		/// <param name="oS"></param>
		private void WeaponDetail(Session oS, bool IsEnhance = true)
		{
			JObject jsonFull = JObject.Parse(oS.GetResponseBodyAsString()) as JObject;
			dynamic temp = jsonFull;
			if (!IsEnhance)
			{
				dynamic master = temp.master;
				dynamic param = temp.param;
				if (temp.id != null)
				{
					int SkillLv = param.skill_level;
					int MasterId = master.id;
					if (SkillLv <= 1) return;
					if (MasterId == 1039900000 || MasterId == 1029900000) return;
					int id = temp.id;
					dynamic skill1 = temp.skill1;
					dynamic skill2 = temp.skill2;

					if (skill1.comment != null) this.WeaponLvSave(id, 1, SkillLv);
					if (skill2.comment != null) this.WeaponLvSave(id, 2, SkillLv);
				}
			}
			else
			{
				dynamic detail = temp.detail;
				JObject NewInfo = (JObject)temp["new"];
				if (detail.id != null)
				{
					int ParamId = detail.id;
					int MasterId = detail.master.id;
					int NewSkillLv = (int)NewInfo["skill_level"];

					if (NewSkillLv <= 1) return;
					if (MasterId == 1039900000 || MasterId == 1029900000) return;

					dynamic skill1 = detail.skill1;
					dynamic skill2 = detail.skill2;

					if (skill1.comment != null) this.WeaponLvSave(ParamId, 1, NewSkillLv);
					if (skill2.comment != null) this.WeaponLvSave(ParamId, 2, NewSkillLv);
				}
			}
		}
		/// <summary>
		/// 기본 무기 리스트. 강화/리스트/창고가 포함됨
		/// </summary>
		/// <param name="oS"></param>
		#region List
		private void ListDetail(Session oS)
		{
			//this.WeaponListLoad();
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

			//this.ProgressBar();

			for (int i = 0; i < weaponList.Count; i++)
			{
				dynamic tempIndex = weaponList[i].master;
				dynamic tempparam = weaponList[i].param;
				WeaponInfo temp = new WeaponInfo();

				temp.MasterId = tempIndex.id;

				temp.ItemName = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponList, "", TranslateKind.Google, temp.MasterId);
				if (temp.ItemName != string.Empty)
				{
					temp = this.InputSkillInfo(temp);
					temp.Element = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.Element, "", TranslateKind.Google, temp.MasterId);
					temp.Kind = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponType, "", TranslateKind.Google, temp.MasterId);
				}
				else
				{
					int msid = temp.MasterId;
					temp = MasterInfoLoad(temp.MasterId);

					if (temp.SkillName1 != string.Empty) temp.SkillDetail1 = GrandcypherClient.Current.Translations.GetSkillInfo(temp.SkillName1, true);
					else temp.SkillDetail1 = string.Empty;
					if (temp.SkillName2 != string.Empty) temp.SkillDetail2 = GrandcypherClient.Current.Translations.GetSkillInfo(temp.SkillName2, true);
					else temp.SkillDetail2 = string.Empty;
				}
				temp.is_used = weaponList[i].is_used;
				temp.ParamId = tempparam.id;//무기 스킬레벨등을 저장하고 구별하기 위한 부분

				temp.SkillLv1 = WeaponLvLoad(temp.ParamId, 1);
				temp.SkillLv2 = WeaponLvLoad(temp.ParamId, 2);

				if (temp.SkillName1 == string.Empty || temp.SkillName1 == null) temp.vSkillLv1 = Visibility.Collapsed;
				else temp.vSkillLv1 = Visibility.Visible;
				if (temp.SkillName2 == string.Empty || temp.SkillName2 == null) temp.vSkillLv2 = Visibility.Collapsed;
				else temp.vSkillLv2 = Visibility.Visible;

				WeaponLists.Add(temp);
				this.ProgressStatus.Current++;
				//this.ProgressBar();
			}
			this.ListIsEnd = true;
			//this.LoadingEnd();
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

			//this.WeaponListLoad();
			int MasterAttribute = 0;

			this.SkillCounter = new Skills();

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
				//this.ProgressBar();
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
				deck.ItemName = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponList, "", TranslateKind.Google, deck.MasterId);
				if (deck.ItemName == string.Empty)
				{
					int msid = deck.MasterId;
					deck = MasterInfoLoad(deck.MasterId);
					deck.MasterId = msid;
					deck.IsManual = true;
					deck.vSkillLv1 = Visibility.Collapsed;
					deck.vSkillLv2 = Visibility.Collapsed;

					if (deck.SkillName1 != string.Empty) deck.SkillDetail1 = GrandcypherClient.Current.Translations.GetSkillInfo(deck.SkillName1, true);
					else deck.SkillDetail1 = string.Empty;
					if (deck.SkillName2 != string.Empty) deck.SkillDetail2 = GrandcypherClient.Current.Translations.GetSkillInfo(deck.SkillName2, true);
					else deck.SkillDetail2 = string.Empty;
				}
				else
				{
					deck = this.InputSkillInfo(deck);
					deck.attribute = (int)master["attribute"];
					deck.Kind = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.WeaponType, "", TranslateKind.Google, deck.MasterId);
				}

				deck.ParamId = (int)param["id"];//무기 스킬레벨등을 저장하고 구별하기 위한 부분


				if (i == 1)
				{
					MainWeapon = deck;
				}
				else WeaponLists.Add(deck);

				this.ProgressStatus.Current++;
				//this.ProgressBar();
			}


			for (int i = 0; i < WeaponLists.Count; i++)
			{
				if (WeaponLists[i].SkillAttribute1 == MasterAttribute
					|| WeaponLists[i].SkillAttribute1 >= 7)
					this.SumAtt(WeaponLists[i].GeneralType1, WeaponLists[i]);
				if (WeaponLists[i].SkillAttribute2 == MasterAttribute
					|| WeaponLists[i].SkillAttribute2 >= 7)
					this.SumAtt(WeaponLists[i].GeneralType2, WeaponLists[i]);
			}

			//this.DeckLoadingEnd();
		}
		/// <summary>
		/// 무기 정보를 받아 무기의 스킬배수를 계산
		/// 바하무트 무기 계산과 언노운 무기 계산에 오류 발생.
		/// </summary>
		/// <param name="General"></param>
		/// <param name="weapon"></param>
		/// <param name="IsSecond"></param>
		private void SumAtt(int General, WeaponInfo weapon, bool IsSecond = false)
		{
			if (!IsSecond)
			{
				switch (General)
				{
					case 1://공격
						switch (weapon.AttackType1)
						{
							case 1://일반
								this.SkillCounter.Noramal += this.RaiseSkillLevel(weapon.Skill_Rank1, weapon.SkillLv1, weapon.Is_Double1);
								break;
							case 2://언노운
								this.SkillCounter.Unknown += this.RaiseSkillLevel(weapon.Skill_Rank1, weapon.SkillLv1, weapon.Is_Double1);
								break;
							case 3://스트렝스
								this.SkillCounter.Str += this.RaiseSkillLevel(weapon.Skill_Rank1, weapon.SkillLv1, weapon.Is_Double1);
								break;
							case 4://세이빙
								this.SkillCounter.Saving += this.RaiseSkillLevel(weapon.Skill_Rank1, weapon.SkillLv1, weapon.Is_Double1);
								break;
							case 5://마그나
								this.SkillCounter.Magna += this.RaiseSkillLevel(weapon.Skill_Rank1, weapon.SkillLv1, weapon.Is_Double1);
								break;
							case 6://절대치 상승
								this.SkillCounter.staticAtt += weapon.Skill_Rank1;
								break;
							case 7://vis
								if (weapon.SkillLv1 == 10) this.SkillCounter.Baha += 30;
								else
								{
									this.SkillCounter.Baha += 20;//기본
									this.SkillCounter.Baha += weapon.SkillLv1;//스킬레벨
								}
								break;
							case 8://con
								if (weapon.SkillLv1 == 10) this.SkillCounter.Baha += 15;
								else
								{
									this.SkillCounter.Baha += 10;//기본
									this.SkillCounter.Baha += weapon.SkillLv1 * 0.5;//스킬레벨 * 0.5
								}
								break;
						}
						break;
					case 3://배수
						break;
				}
			}
			else
			{
				switch (General)
				{
					case 1://공격
						switch (weapon.AttackType2)
						{
							case 1://일반
								this.SkillCounter.Noramal += this.RaiseSkillLevel(weapon.Skill_Rank2, weapon.SkillLv2, weapon.Is_Double2);
								break;
							case 2://언노운
								this.SkillCounter.Unknown += this.RaiseSkillLevel(weapon.Skill_Rank2, weapon.SkillLv2, weapon.Is_Double2);
								break;
							case 3://스트렝스
								this.SkillCounter.Str += this.RaiseSkillLevel(weapon.Skill_Rank2, weapon.SkillLv2, weapon.Is_Double2);
								break;
							case 4://세이빙
								this.SkillCounter.Saving += this.RaiseSkillLevel(weapon.Skill_Rank2, weapon.SkillLv2, weapon.Is_Double2);
								break;
							case 5://마그나
								this.SkillCounter.Magna += this.RaiseSkillLevel(weapon.Skill_Rank2, weapon.SkillLv2, weapon.Is_Double2);
								break;
							case 6://절대치 상승
								this.SkillCounter.staticAtt += weapon.Skill_Rank2;
								break;
							case 7://vis
								if (weapon.SkillLv2 == 10) this.SkillCounter.Baha += 30;
								else
								{
									this.SkillCounter.Baha += 20;
									this.SkillCounter.Baha += weapon.SkillLv2;
								}
								break;
							case 8://con
								if (weapon.SkillLv2 == 10) this.SkillCounter.Baha += 15;
								else
								{
									this.SkillCounter.Baha += 10;
									this.SkillCounter.Baha += weapon.SkillLv2 * 0.5;
								}
								break;
						}
						break;
					case 3://배수
						break;
				}
			}
		}
		private double RaiseSkillLevel(int Rank, int SkillLv, bool IsDouble)
		{
			double ans = 0;
			switch (Rank)//기본값 설정
			{
				case 1:
					ans = 0;
					break;
				case 2:
					ans = 2;
					break;
				case 3:
					if (IsDouble) ans = 6;
					else ans = 5;
					break;
			}
			if (SkillLv < 11)//스킬레벨이 10이하인 경우엔 모든 증가폭이 1이다
			{
				return ans + SkillLv;
			}
			else//스킬레벨이 11이상인 경우엔 증가폭이 다르게 나타난다
			{
				ans += 10;
				switch (Rank)//추가치를 더한다.
				{
					case 1:
						return ans += SkillLv * 0.4;
					case 2:
						return ans += SkillLv * 0.5;
					case 3:
						if (IsDouble)
							return ans += SkillLv * 0.8;
						else
							return ans += SkillLv * 0.6;
				}
			}
			return 0;
		}
		public void Reload()
		{
			if (this.DeckIsEnd) this.DeckLoadingEnd();
			if (this.ListIsEnd) this.LoadingEnd();
		}
		/// <summary>
		/// 마스터 리스트를 불러온다
		/// </summary>
		public void MasterInfoListLoad()
		{
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			var binPath = Path.Combine(MainFolder, "Data", "MasterWeapon.bin");

			var items = new Dictionary<int, WeaponInfo>();

			if (File.Exists(binPath))
			{
				var bytes = File.ReadAllBytes(binPath);
				using (var memoryStream = new MemoryStream(bytes))
				using (var reader = new BinaryReader(memoryStream))
				{
					while (memoryStream.Position < memoryStream.Length)
					{
						int paramID = reader.ReadInt32();
						var item = new WeaponInfo
						{
							attribute = reader.ReadInt32(),
							WeaponType = reader.ReadInt32(),
							ItemName = reader.ReadString(),
							SkillName1 = reader.ReadString(),
							SkillName2 = reader.ReadString(),
						};
						items.Add(paramID, item);
					}
					memoryStream.Dispose();
					memoryStream.Close();
					reader.Dispose();
					reader.Close();
				}
				//파일 읽기 종료
			}
			this.MasterBinList = items;
		}
		/// <summary>
		/// 어플리케이션 구동시 불러온 마스터 리스트를 기반으로 데이터를 검색해서 자동으로 세팅.
		/// </summary>
		/// <param name="MasterId"></param>
		/// <returns></returns>
		private WeaponInfo MasterInfoLoad(int MasterId)
		{
			if (this.MasterBinList == null)
			{
				return new WeaponInfo//스킬 리스트에 직접 접근하지 못하므로 일단 스킬명은 String.Empty로 설정
				{
					MasterId = MasterId,
					IsManual = true,
					attribute = 1,
					WeaponType = 1,
					ItemName = MasterId.ToString(),
					SkillName1 = string.Empty,
					SkillName2 = string.Empty,
					vSkillLv1 = Visibility.Collapsed,
					vSkillLv2 = Visibility.Collapsed,
				};
			}
			if (this.MasterBinList.ContainsKey(MasterId))
			{
				return this.MasterBinList[MasterId];
			}
			else//해당하는 마스터ID가 없는경우 빈 데이터를 출력한다.
			{
				return new WeaponInfo//스킬 리스트에 직접 접근하지 못하므로 일단 스킬명은 String.Empty로 설정
				{
					MasterId = MasterId,
					IsManual = true,
					attribute = 1,
					WeaponType = 1,
					ItemName = MasterId.ToString(),
					SkillName1 = string.Empty,
					SkillName2 = string.Empty,
					vSkillLv1 = Visibility.Collapsed,
					vSkillLv2 = Visibility.Collapsed,
				};
			}
		}
		/// <summary>
		/// UI에서 수동으로 설정한 데이터를 bin파일에 저장한다.
		/// </summary>
		/// <param name="data"></param>
		public void MasterInfoSave(WeaponInfo data)
		{
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			var binPath = Path.Combine(MainFolder, "Data", "MasterWeapon.bin");
			if (!Directory.Exists(Path.Combine(MainFolder, "Data"))) Directory.CreateDirectory(Path.Combine(MainFolder, "Data"));

			var items = new Dictionary<int, WeaponInfo>(this.MasterBinList);

			if (File.Exists(binPath))//파일이 있는경우
			{
				if (items.ContainsKey(data.MasterId))//키를 가지고 있으면 해당 키에 해당하는 값을 덮어쓰기
				{
					items[data.MasterId] = data;
				}
				else//키가 없으면 추가
				{
					items.Add(data.MasterId, data);
				}
			}
			else//파일이 없는경우
			{
				items.Add(data.MasterId, data);
			}

			using (var fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write, FileShare.None))
			using (var writer = new BinaryWriter(fileStream))
			{
				foreach (var item in items)
				{
					writer.Write(item.Key);
					writer.Write(item.Value.attribute);
					writer.Write(item.Value.WeaponType);
					writer.Write(item.Value.ItemName);
					writer.Write(item.Value.SkillName1);
					writer.Write(item.Value.SkillName2);
				}
				fileStream.Dispose();
				fileStream.Close();
				writer.Dispose();
				writer.Close();
			}
			MasterInfoListLoad();
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
					if (temp.SkillLv2 < 1 || temp.SkillLv2 > 15) temp.SkillLv2 = 1;
				}
				else
				{
					temp.SkillLv2 = Lv;
					if (temp.SkillLv1 < 1 || temp.SkillLv1 > 15) temp.SkillLv1 = 1;
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

		/// <summary>
		/// -1: 카테고리화 되지 않는 무기들이 포함
		//1: 공격력 상승 카테고리의 무기가 모두 여기에 포함
		//2: HP 상승 카테고리의 무기가 모두 여기에 포함
		//3. 배수 카테고리의 무기가 모두 여기에 포함
		//
		//0. 미분류.기본적으로 계산에 포함되지않는다
		//1. 일반공인.바하무기도 여기에 포함
		//2. 언노운.신데렐라 콜라보 무기만 여기에 포함된다
		//3. 스트렝스.테일즈 콜라보 무기만 여기에 포함된다
		//4. 세이빙 어택.스트리트 파이터 무기만 여기에 포함된다
		//5. 마그나.마그나 드랍 무기만 여기에 포함된다
		//6. 절대 공격력 상승 무기가 여기에 추가
		//7. 바하무트 웨폰 위스
		//8. 바하무트 웨폰 콘킬리오
		//
		//1. 화
		//2. 수
		//3. 풍
		//4. 토
		//5. 광
		//6. 암
		//7. 이벤트무기중 메인 무기의 속성을 따라가는 경우
		//8. 속성무관
		//
		//0. 기본.최대 스킬레벨은 10이다
		//1. 최종상한무기.최대 스킬레벨은 15다
		//
		//0. 미분류.공인이 아닌 무기는 모두 여기에 포함
		//1. 소
		//2. 중
		//3. 대
		//4. 위스
		//5. 콘킬리오
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		private WeaponInfo InputSkillInfo(WeaponInfo info)
		{
			WeaponInfo temp = new WeaponInfo();
			temp = info;

			var skill_name1 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.FirstSkillName, temp.ItemName, 0, temp.MasterId);
			var skill_name2 = GrandcypherClient.Current.Translations.GetTranslation(Translations.TranslationType.LastSkillDetail, temp.ItemName, 0, temp.MasterId);
			var spl_data1 = GrandcypherClient.Current.Translations.GetSkillInfo(skill_name1, false, true).Split(';');
			var spl_data2 = GrandcypherClient.Current.Translations.GetSkillInfo(skill_name2, false, true).Split(';');

			temp.SkillName1 = skill_name1;
			temp.SkillName2 = skill_name2;
			temp.SkillDetail1 = GrandcypherClient.Current.Translations.GetSkillInfo(skill_name1, true);
			temp.SkillDetail2 = GrandcypherClient.Current.Translations.GetSkillInfo(skill_name2, true);

			temp.SkillLv1 = WeaponLvLoad(temp.ParamId,1);
			temp.SkillLv2 = temp.SkillLv1;


			List<int> data = new List<int>();
			for (int i = 0; i < spl_data1.Count(); i++)
			{
				if (spl_data1[i] != string.Empty) data.Add(Convert.ToInt32(spl_data1[i]));
			}
			for (int i = 0; i < data.Count; i++)
			{
				switch (i)
				{
					case 0:
						if (info.SkillName1 != string.Empty)
						{
							info.GeneralType1 = data[i];
							info.vSkillLv1 = Visibility.Visible;
						}
						if (info.SkillName2 != string.Empty)
						{
							info.GeneralType2 = data[i];
							info.vSkillLv2 = Visibility.Visible;
						}
						break;
					case 1:
						if (info.SkillName1 != string.Empty) info.AttackType1 = data[i];
						if (info.SkillName2 != string.Empty) info.AttackType2 = data[i];
						break;
					case 2:
						if (info.SkillName1 != string.Empty) info.SkillAttribute1 = data[i];
						if (info.SkillName2 != string.Empty) info.SkillAttribute2 = data[i];
						break;
					case 3:
						if (info.SkillName1 != string.Empty) info.Is_Unlimited1 = Convert.ToBoolean(data[i]);
						if (info.SkillName2 != string.Empty) info.Is_Unlimited2 = Convert.ToBoolean(data[i]);
						break;
					case 4:
						if (info.SkillName1 != string.Empty) info.Skill_Rank1 = data[i];
						if (info.SkillName2 != string.Empty) info.Skill_Rank2 = data[i];
						break;
					case 5:
						if (info.SkillName1 != string.Empty) info.Is_Double1 = Convert.ToBoolean(data[i]);
						if (info.SkillName2 != string.Empty) info.Is_Double2 = Convert.ToBoolean(data[i]);
						break;
				}
			}


			return temp;
		}
	}
	public class SkillLvTable
	{
		public int SkillLv1 { get; set; }
		public int SkillLv2 { get; set; }
	}
	public class Skills
	{
		public double TotalAttack { get; set; }

		//normal
		public double Noramal { get; set; }

		public double Baha { get; set; }

		//unknown
		public double Unknown { get; set; }

		//magna
		public double Magna { get; set; }

		//strength
		public double Str { get; set; }

		//saving
		public double Saving { get; set; }

		public double staticAtt { get; set; }
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
		#region attribute List
		private static Dictionary<int, string> AttributeTable = new Dictionary<int, string>
		{
			{1,"화" }, {2,"수"}, {3,"토"}, {4,"풍"}, {5,"빛"}, {6,"암"},
		};
		#endregion

		#region WeaponType List
		private static Dictionary<int, string> WeaponTypeTable = new Dictionary<int, string>
		{
			{1,"검"}, {2,"단검" }, {3,"창" }, {4,"도끼"}, {5,"지팡이"}, {6,"총"}, {7,"권갑"}, {8,"활"}, {9,"악기"}, {10,"도" }, {11,"소재" },
		};
		#endregion

		public int MasterId { get; set; }
		public int ParamId { get; set; }

		private int _attribute;
		public int attribute
		{
			get { return this._attribute; }
			set
			{
				if (this._attribute == value) return;
				this._attribute = value;
				this.Element = AttributeTable[value];
			}
		}
		private int _WeaponType;
		public int WeaponType
		{
			get { return this._WeaponType; }
			set
			{
				if (this._WeaponType == value) return;
				this._WeaponType = value;
				this.Kind = WeaponTypeTable[value];
			}
		}

		public string ItemName { get; set; }
		public string Element { get; set; }
		public string Kind { get; set; }
		public bool is_used { get; set; }
		public bool IsManual { get; set; }

		public string SkillName1 { get; set; }
		public string SkillDetail1 { get; set; }
		public int GeneralType1 { get; set; }
		public int AttackType1 { get; set; }
		public int SkillAttribute1 { get; set; }
		public bool Is_Unlimited1 { get; set; }
		public int Skill_Rank1 { get; set; }
		public bool Is_Double1 { get; set; }

		#region Skill Level 1
		private int _SkillLv1;
		public int SkillLv1
		{
			get { return this._SkillLv1; }
			set
			{
				if (this._SkillLv1 == value) return;
				this._SkillLv1 = value;
				if (this._SkillLv1 < 1 || this._SkillLv1 > 15) this._SkillLv1 = 1;
				GrandcypherClient.Current.WeaponHooker.Reload();
				if (this.ParamId != 0)
				{
					GrandcypherClient.Current.WeaponHooker.WeaponLvSave(this.ParamId, 1, this._SkillLv1);
					if (vSkillLv2 == Visibility.Visible)
					{
						SkillLv2 = value;
						if (this.SkillLv2 < 1 || this.SkillLv2 > 15) this.SkillLv2 = 1;
						GrandcypherClient.Current.WeaponHooker.Reload();

						GrandcypherClient.Current.WeaponHooker.WeaponLvSave(this.ParamId, 2, value);
					}
				}
			}
		}
		public Visibility vSkillLv1 { get; set; }
		#endregion

		public string SkillName2 { get; set; }
		public string SkillDetail2 { get; set; }
		public int GeneralType2 { get; set; }
		public int AttackType2 { get; set; }
		public int SkillAttribute2 { get; set; }
		public bool Is_Unlimited2 { get; set; }
		public int Skill_Rank2 { get; set; }
		public bool Is_Double2 { get; set; }

		#region Skill Level 2
		public int SkillLv2 { get; set; }
		public Visibility vSkillLv2 { get; set; }
		#endregion

		//param

	}
}
