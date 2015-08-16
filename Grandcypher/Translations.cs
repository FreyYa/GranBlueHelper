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

		internal Translations()
		{
			try
			{
				if (File.Exists(Path.Combine(MainFolder, "Translations", "JPTRs.xml"))) JPTRs = XDocument.Load(Path.Combine(MainFolder, "Translations", "JPTRs.xml"));
				if (File.Exists(Path.Combine(MainFolder, "Translations", "WeaponList.xml"))) WeaponLists = XDocument.Load(Path.Combine(MainFolder, "Translations", "WeaponList.xml"));
				if (File.Exists(Path.Combine(MainFolder, "Translations", "SkillList.xml"))) WeaponSkills = XDocument.Load(Path.Combine(MainFolder, "Translations", "SkillList.xml"));

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
			else
				SkillListVersion = "없음";
		}
		private IEnumerable<XElement> GetTranslationList(TranslationType type, TranslateKind sitetype = TranslateKind.Google)
		{
			if (TranslationType.PartTranslate == type)
			{
				if (JPTRs != null)
				{
					if (GrandcypherClient.Current.Updater.JPTRsUpdate)
					{
						this.JPTRs = XDocument.Load(Path.Combine(MainFolder, "Translations", "JPTRs.xml"));
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
				if (File.Exists(Path.Combine(MainFolder, "Translations", "Scenarios", siteKind, GrandcypherClient.Current.ScenarioHooker.PathName + ".xml")))
					Scenarios = XDocument.Load(Path.Combine(MainFolder, "Translations", "Scenarios", siteKind, GrandcypherClient.Current.ScenarioHooker.PathName + ".xml"));
				if (Scenarios != null)
				{
					if (GrandcypherClient.Current.Updater.JPTRsUpdate)
					{
						this.Scenarios = XDocument.Load(Path.Combine(MainFolder, "Translations", "Scenarios", siteKind, GrandcypherClient.Current.ScenarioHooker.PathName + ".xml"));
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
						this.WeaponLists = XDocument.Load(Path.Combine(MainFolder, "Translations", "WeaponList.xml"));
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
						this.WeaponSkills = XDocument.Load(Path.Combine(MainFolder, "Translations", "SkillList.xml"));
						GrandcypherClient.Current.Updater.SkillListUpdate = false;
					}
					return WeaponSkills.Descendants("Skill");
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
		public string GetSkillInfo(string SkillDetail, bool IsDetailFind = false)
		{
			var start = "Detail";
			var Target = "KrName";
			if (IsDetailFind)
			{
				start = "KrName";
				Target = "Detail";
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
			catch { }

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

			if (!Directory.Exists(Path.Combine(MainFolder, "Translations")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Translations"));
			if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "Scenarios")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "Scenarios"));
			if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "Scenarios", "Google")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "Scenarios", "Google"));
			if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "Scenarios", "Naver")))
				Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "Scenarios", "Naver"));

			if (File.Exists(Path.Combine(MainFolder, "Translations", "Scenarios", context[0].PathName + ".xml")))
			{
				File.Copy(Path.Combine(MainFolder, "Translations", "Scenarios", context[0].PathName + ".xml"), Path.Combine(MainFolder, "Translations", "Scenarios", context[0].PathName + ".xml.bak"), true);
				File.Delete(Path.Combine(MainFolder, "Translations", "Scenarios", context[0].PathName + ".xml"));
			}

			NewXmlDoc.Save(Path.Combine(MainFolder, "Translations", "Scenarios", context[0].PathName + ".xml"));

			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.Load(Path.Combine(MainFolder, "Translations", "Scenarios", context[0].PathName + ".xml"));
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
				XmlDoc.Save(Path.Combine(MainFolder, "Translations", "Scenarios", "Google", context[0].PathName + ".xml"));
			else if (type == TranslateKind.Naver)
				XmlDoc.Save(Path.Combine(MainFolder, "Translations", "Scenarios", "Naver", context[0].PathName + ".xml"));
		}
		protected XmlNode CreateNode(XmlDocument xmlDoc, string name, string innerXml)
		{
			XmlNode node = xmlDoc.CreateElement(string.Empty, name, string.Empty);
			node.InnerXml = innerXml;
			return node;
		}
		public enum TranslationType
		{
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
		}
	}
}