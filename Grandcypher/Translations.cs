using Grandcypher.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Grandcypher
{
	public class Translations : NotificationObject
	{
		private XDocument JPTRs;
		private XDocument Scenarios;
		private XDocument WeaponLists;
		private XDocument WeaponSkills;
		private XDocument TenLists;
		private XDocument BulletLists;
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


		#region JPTRsVersion

		private string _JPTRsVersion;

		public string JPTRsVersion
		{
			get { return _JPTRsVersion; }
			set
			{
				if (_JPTRsVersion != value)
				{
					_JPTRsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SkillListVersion

		private string _SkillListVersion;

		public string SkillListVersion
		{
			get { return _SkillListVersion; }
			set
			{
				if (_SkillListVersion != value)
				{
					_SkillListVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region WeaponListVersion

		private string _WeaponListVersion;

		public string WeaponListVersion
		{
			get { return _JPTRsVersion; }
			set
			{
				if (_WeaponListVersion != value)
				{
					_WeaponListVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TenListVersion

		private string _TenListVersion;

		public string TenListVersion
		{
			get { return _TenListVersion; }
			set
			{
				if (_TenListVersion != value)
				{
					_TenListVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BulletListVersion

		private string _BulletListVersion;

		public string BulletListVersion
		{
			get { return _BulletListVersion; }
			set
			{
				if (_BulletListVersion != value)
				{
					_BulletListVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		internal Translations()
		{
			try
			{
				if (File.Exists(Path.Combine(MainFolder, "XMLs", "JPTRs.xml"))) JPTRs = XDocument.Load(Path.Combine(MainFolder, "XMLs", "JPTRs.xml"));
				if (File.Exists(Path.Combine(MainFolder, "XMLs", "WeaponList.xml"))) WeaponLists = XDocument.Load(Path.Combine(MainFolder, "XMLs", "WeaponList.xml"));
				if (File.Exists(Path.Combine(MainFolder, "XMLs", "SkillList.xml"))) WeaponSkills = XDocument.Load(Path.Combine(MainFolder, "XMLs", "SkillList.xml"));
				if (File.Exists(Path.Combine(MainFolder, "XMLs", "TenList.xml"))) TenLists = XDocument.Load(Path.Combine(MainFolder, "XMLs", "TenList.xml"));
				if (File.Exists(Path.Combine(MainFolder, "XMLs", "BulletList.xml"))) BulletLists = XDocument.Load(Path.Combine(MainFolder, "XMLs", "BulletList.xml"));

				GetVersions();
			}
			catch { }
		}
		private void GetVersions()
		{
			if (JPTRs != null)
			{
				if (JPTRs.Root.Attribute("Version") != null) JPTRsVersion = JPTRs.Root.Attribute("Version").Value;
				else JPTRsVersion = "알 수 없음";
			}
			else
				JPTRsVersion = "없음";

			if (WeaponLists != null)
			{
				if (WeaponLists.Root.Attribute("Version") != null) WeaponListVersion = WeaponLists.Root.Attribute("Version").Value;
				else WeaponListVersion = "알 수 없음";
			}
			else
				WeaponListVersion = "없음";

			if (WeaponSkills != null)
			{
				if (WeaponSkills.Root.Attribute("Version") != null) SkillListVersion = WeaponSkills.Root.Attribute("Version").Value;
				else SkillListVersion = "알 수 없음";
			}
			if (TenLists != null)
			{
				if (TenLists.Root.Attribute("Version") != null) TenListVersion = TenLists.Root.Attribute("Version").Value;
				else TenListVersion = "알 수 없음";
			}
			if (BulletLists != null)
			{
				if (BulletLists.Root.Attribute("Version") != null) BulletListVersion = BulletLists.Root.Attribute("Version").Value;
				else BulletListVersion = "알 수 없음";
			}
			else
				BulletListVersion = "없음";
		}
		private IEnumerable<XElement> GetTranslationList(TranslationType type, TranslateKind sitetype = TranslateKind.Google)
		{
			if (TranslationType.PartTranslate == type)
			{
				if (JPTRs != null)
				{
					if (GrandcypherClient.Current.Updater.JPTRsUpdate)
					{
						this.JPTRs = XDocument.Load(Path.Combine(MainFolder, "XMLs", "JPTRs.xml"));
						GrandcypherClient.Current.Updater.JPTRsUpdate = false;
					}
					return JPTRs.Descendants("JPTR");
				}
				return null;
			}
			else if (type == TranslationType.ScenarioDetail)
			{
				string siteKind = "Google";
				if (sitetype == TranslateKind.Naver) siteKind = "Naver";
				if (File.Exists(Path.Combine(MainFolder, "XMLs", "Scenarios", siteKind, GrandcypherClient.Current.ScenarioHooker.PathName + ".xml")))
					Scenarios = XDocument.Load(Path.Combine(MainFolder, "XMLs", "Scenarios", siteKind, GrandcypherClient.Current.ScenarioHooker.PathName + ".xml"));
				if (Scenarios != null)
				{
					if (GrandcypherClient.Current.Updater.JPTRsUpdate)
					{
						this.Scenarios = XDocument.Load(Path.Combine(MainFolder, "XMLs", "Scenarios", siteKind, GrandcypherClient.Current.ScenarioHooker.PathName + ".xml"));
						GrandcypherClient.Current.Updater.JPTRsUpdate = false;
					}
					return Scenarios.Descendants("Scenario");
				}
				return null;
			}
			else if (type == TranslationType.FirstSkillName || type == TranslationType.LastSkillName || type == TranslationType.WeaponList || type == TranslationType.FirstSkillDetail || type == TranslationType.LastSkillDetail || TranslationType.WeaponType == type || TranslationType.Element == type)
			{
				if (WeaponLists != null)
				{
					if (GrandcypherClient.Current.Updater.WeaponListUpdate)
					{
						this.WeaponLists = XDocument.Load(Path.Combine(MainFolder, "XMLs", "WeaponList.xml"));
						GrandcypherClient.Current.Updater.WeaponListUpdate = false;
					}
					return WeaponLists.Descendants("Weapon");
				}
				return null;
			}
			else if (TranslationType.SkillDetails == type)
			{
				if (WeaponSkills != null)
				{
					if (GrandcypherClient.Current.Updater.JPTRsUpdate)
					{
						this.WeaponSkills = XDocument.Load(Path.Combine(MainFolder, "XMLs", "SkillList.xml"));
						GrandcypherClient.Current.Updater.SkillListUpdate = false;
					}
					return WeaponSkills.Descendants("Skill");
				}
				return null;
			}
			else if (TranslationType.TenTreasure == type)
			{
				if (TenLists != null)
				{
					if (GrandcypherClient.Current.Updater.TenListUpdate)
					{
						this.TenLists = XDocument.Load(Path.Combine(MainFolder, "XMLs", "TenList.xml"));
						GrandcypherClient.Current.Updater.TenListUpdate = false;
					}
					return TenLists.Descendants("Treasure");
				}
				return null;
			}
			else if (TranslationType.BulletMake == type)
			{
				if (BulletLists != null)
				{
					if (GrandcypherClient.Current.Updater.BulletListUpdate)
					{
						this.BulletLists = XDocument.Load(Path.Combine(MainFolder, "XMLs", "BulletList.xml"));
						GrandcypherClient.Current.Updater.BulletListUpdate = false;
					}
					return BulletLists.Descendants("Bullet");
				}
				return null;
			}
			else return null;
		}
		public List<string> GetSkillList()
		{
			List<string> temp = new List<string>();
			IEnumerable<XElement> TranslationList = GetTranslationList(TranslationType.SkillDetails);
			foreach (var item in TranslationList)
			{
				temp.Add(item.Element("Detail").Value);
			}

			temp.Sort(delegate (string x, string y)
			{
				return x.CompareTo(y);
			});
			return temp;
		}
		public List<TenTreasureInfo> GetTreasureList()
		{
			List<TenTreasureInfo> templist = new List<TenTreasureInfo>();
			IEnumerable<XElement> TranslationList = GetTranslationList(TranslationType.TenTreasure);
			foreach (var item in TranslationList)
			{
				TenTreasureInfo temp = new TenTreasureInfo();

				temp.Name = ElementOutput(item, "ItemName");
				temp.Proto = Convert.ToInt32(ElementOutput(item, "Proto"));
				temp.Rust = Convert.ToInt32(ElementOutput(item, "Rust"));
				temp.Element = Convert.ToInt32(ElementOutput(item, "Element"));
				temp.First = Convert.ToInt32(ElementOutput(item, "First"));
				temp.Second = Convert.ToInt32(ElementOutput(item, "Second"));
				temp.Third = Convert.ToInt32(ElementOutput(item, "Third"));
				temp.Fourth = Convert.ToInt32(ElementOutput(item, "Fourth"));
				temp.Fifth = Convert.ToInt32(ElementOutput(item, "Fifth"));
				temp.Sixth = Convert.ToInt32(ElementOutput(item, "Sixth"));
				temp.ElementID = Convert.ToInt32(ElementOutput(item, "ElementID"));
				temp.Origin = Convert.ToInt32(ElementOutput(item, "Origin"));

				templist.Add(temp);
			}

			return templist;
		}
		public List<Bullet> GetBulletList()
		{
			List<Bullet> templist = new List<Bullet>();
			IEnumerable<XElement> TranslationList = GetTranslationList(TranslationType.BulletMake);
			foreach (var item in TranslationList)
			{
				Bullet temp = new Bullet();
				temp.MaterialList = new List<TreasureInfo>();

				temp.Name = ElementOutput(item, "Name");
				temp.TrName = ElementOutput(item, "TrName");
				temp.MasterID = Convert.ToInt32(ElementOutput(item, "ID"));
				temp.BulletKind = Convert.ToInt32(ElementOutput(item, "BulletKind"));
				temp.Rank = Convert.ToInt32(ElementOutput(item, "Rank"));

				int Count = 1;
				bool Checker = true;

				while (Checker)
				{
					//이름작성
					string Materialstr = "Material";
					var strtemp = Materialstr + Count.ToString();

					TreasureInfo material = new TreasureInfo();

					if (item.Element(strtemp) != null)
					{
						material.Name = item.Element(strtemp).Value;
						strtemp = "MaterialCount" + Count.ToString();
						material.max = Convert.ToInt32(item.Element(strtemp).Value);

						temp.MaterialList.Add(material);
						Count++;
					}
					else
					{
						break;
					}

				}//while구문 종료

				templist.Add(temp);
			}

			return templist;
		}
		private string ElementOutput(XElement item, string ElementName)
		{
			if (item.Element(ElementName) != null)
				return item.Element(ElementName).Value;
			else return "0";
		}
		public string GetSkillInfo(string SkillDetail, bool IsDetailFind = false, bool IsDataFind = false)
		{
			var start = "Detail";
			var Target = "KrName";
			if (IsDetailFind)
			{
				start = "KrName";
				Target = "Detail";
			}
			else if (IsDataFind)
			{
				start = "KrName";
				Target = "SkillData";
			}

			IEnumerable<XElement> TranslationList = GetTranslationList(TranslationType.SkillDetails);



			foreach (var item in TranslationList)
			{
				if (item.Element(start).Value == SkillDetail) return item.Element(Target).Value;
			}

			return string.Empty;
		}
		public string GetTranslation(TranslationType type, string JPString, TranslateKind sitekind, int MasterId = -1)
		{
			try
			{
				IEnumerable<XElement> TranslationList = GetTranslationList(type, sitekind);

				string JPChildElement = "JPstring";
				string TRChildElement = "TRstring";
				string IDElement = "ID";



				if (MasterId < 0)
				{
					if (type == TranslationType.ScenarioDetail)
					{
						JPChildElement = "JPDtail";
						TRChildElement = "TrDtail";

						IEnumerable<XElement> OldFoundTranslation = TranslationList.Where(b => b.Attribute(JPChildElement).Value.Equals(JPString));

						foreach (XElement el in OldFoundTranslation)
						{
							return el.Attribute(TRChildElement).Value;
						}
					}
					else if (type == TranslationType.FirstSkillName || type == TranslationType.LastSkillName)
					{
						JPChildElement = "JpName";
						TRChildElement = "KrName";

						IEnumerable<XElement> OldFoundTranslation = TranslationList.Where(b => b.Attribute(JPChildElement).Value.Equals(JPString));

						foreach (XElement el in OldFoundTranslation)
						{
							return el.Attribute(TRChildElement).Value;
						}
					}
					else
					{
						IEnumerable<XElement> OldFoundTranslation = TranslationList.Where(b => b.Element(JPChildElement).Value.Equals(JPString));//string 비교검색
						foreach (XElement el in OldFoundTranslation)
						{
							return el.Element(TRChildElement).Value;
						}
					}

#if DEBUG
					Debug.WriteLine(string.Format("Can't find Translation: {0,-20} {1}", JPString, MasterId));
#endif
				}
				else
				{
					switch (type)
					{
						case TranslationType.WeaponList:
							TRChildElement = "Name";
							break;
						case TranslationType.FirstSkillName:
							TRChildElement = "Skill1";
							break;
						case TranslationType.FirstSkillDetail:
							TRChildElement = "Skill1Detail";
							break;
						case TranslationType.LastSkillName:
							TRChildElement = "Skill2";
							break;
						case TranslationType.LastSkillDetail:
							TRChildElement = "Skill2Detail";
							break;
						case TranslationType.Element:
							TRChildElement = "Element";
							break;
						case TranslationType.WeaponType:
							TRChildElement = "Kind";
							break;
					}

					IEnumerable<XElement> FoundTranslation = TranslationList.Where(b => b.Element(IDElement).Value.Equals(MasterId.ToString()));//아이템 ID검색

					foreach (XElement el in FoundTranslation)
					{
						if (MasterId >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == MasterId)
							return el.Element(TRChildElement).Value;
					}
				}

			}
			catch
			{
				return "";
			}

			return JPString;
		}
		public string ReplaceTranslation(string JPString, TranslationType type = TranslationType.PartTranslate)
		{
			string stemp = JPString;
			try
			{
				string JPChildElement = "JPstring";
				string TRChildElement = "TRstring";

				IEnumerable<XElement> TranslationList = GetTranslationList(type);

				foreach (var item in TranslationList)
				{
					if (stemp.Contains(item.Element(JPChildElement).Value))
					{
						stemp = stemp.Replace(item.Element(JPChildElement).Value, item.Element(TRChildElement).Value);
					}

				}
				return stemp;
			}
			catch { }
			return JPString;
		}
		public void WriteFile(List<Scenario> context, TranslateKind type)
		{
			XmlDocument NewXmlDoc = new XmlDocument();
			NewXmlDoc.AppendChild(NewXmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
			XmlNode Source = NewXmlDoc.CreateElement("", "Scenarios", "");
			NewXmlDoc.AppendChild(Source);

			if (!Directory.Exists(Path.Combine(MainFolder, "XMLs")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs"));
			if (!Directory.Exists(Path.Combine(MainFolder, "XMLs", "Scenarios")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs", "Scenarios"));
			if (!Directory.Exists(Path.Combine(MainFolder, "XMLs", "Scenarios", "Google")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs", "Scenarios", "Google"));
			if (!Directory.Exists(Path.Combine(MainFolder, "XMLs", "Scenarios", "Naver")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs", "Scenarios", "Naver"));

			if (File.Exists(Path.Combine(MainFolder, "XMLs", "Scenarios", context[0].PathName + ".xml")))
			{
				File.Copy(Path.Combine(MainFolder, "XMLs", "Scenarios", context[0].PathName + ".xml"), Path.Combine(MainFolder, "XMLs", "Scenarios", context[0].PathName + ".xml.bak"), true);
				File.Delete(Path.Combine(MainFolder, "XMLs", "Scenarios", context[0].PathName + ".xml"));
			}

			NewXmlDoc.Save(Path.Combine(MainFolder, "XMLs", "Scenarios", context[0].PathName + ".xml"));

			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.Load(Path.Combine(MainFolder, "XMLs", "Scenarios", context[0].PathName + ".xml"));
			foreach (var item in context)
			{
				XmlNode FristNode = XmlDoc.DocumentElement;

				XmlElement root = XmlDoc.CreateElement("Scenario");
				root.SetAttribute("id", item.index.ToString());
				root.SetAttribute("JPName", item.Name);
				root.SetAttribute("TrName", item.TrName);
				root.SetAttribute("JPDtail", item.context);
				root.SetAttribute("TrDtail", item.TrContext);

				FristNode.AppendChild(root);
			}

			if (type == TranslateKind.Google)
				XmlDoc.Save(Path.Combine(MainFolder, "XMLs", "Scenarios", "Google", context[0].PathName + ".xml"));
			else if (type == TranslateKind.Naver)
				XmlDoc.Save(Path.Combine(MainFolder, "XMLs", "Scenarios", "Naver", context[0].PathName + ".xml"));
		}
		protected XmlNode CreateNode(XmlDocument xmlDoc, string name, string innerXml)
		{
			XmlNode node = xmlDoc.CreateElement(string.Empty, name, string.Empty);
			node.InnerXml = innerXml;
			return node;
		}
		public enum TranslationType
		{
			App,
			ScenarioDetail,
			PartTranslate,
			WeaponList,
			FirstSkillName,
			FirstSkillDetail,
			LastSkillName,
			LastSkillDetail,
			Element,
			WeaponType,
			SkillDetails,
			TenTreasure,
			BulletMake,
		}
	}
}