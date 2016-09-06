using Grandcypher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using TranslationType = Grandcypher.Translations.TranslationType;

namespace Grandcypher
{
	public class Updater
	{
		public bool JPTRsUpdate { get; set; }
		public bool SkillListUpdate { get; set; }
		public bool WeaponListUpdate { get; set; }
		public bool TenListUpdate { get; set; }
		public bool BulletListUpdate { get; set; }
		public bool ItemListUpdate { get; set; }
		/// <summary>
		/// 업데이트 상태를 구별한다.
		/// bool값을 조정하며 이는 업데이트 후 바로 퀘스트 로드가 적용되지 않는 문제점을 자체 해결하기 위해 도입한것임.
		/// </summary>
		/// <param name="ReturnVal">-1,0,1을 받는다. 사실상 별 의미 없는거같기도</param>
		/// <param name="type">업데이트 된 항목이 정확히 무엇인지 구별한다</param>
		/// <returns></returns>
		public void UpdateState(int ReturnVal, TranslationType type)
		{
			if (ReturnVal == 1)
			{
				switch (type)
				{
					case TranslationType.PartTranslate:
						this.JPTRsUpdate = true;
						break;
					case TranslationType.SkillDetails:
						this.SkillListUpdate = true;
						break;
					case TranslationType.WeaponList:
						this.WeaponListUpdate = true;
						break;
					case TranslationType.TenTreasure:
						this.TenListUpdate = true;
						break;
					case TranslationType.BulletMake:
						this.BulletListUpdate = true;
						break;
					case TranslationType.ItemIdx:
						this.ItemListUpdate = true;
						break;
				}
			}
		}

		private XDocument VersionXML;

		/// <summary>
		/// Loads the version XML file from a remote URL. This houses all current online version info.
		/// </summary>
		/// <param name="UpdateURL">String URL to the version XML file.</param>
		/// <returns>True: Successful, False: Failed</returns>
		public bool LoadVersion(string UpdateURL)
		{
			try
			{
				VersionXML = XDocument.Load(UpdateURL);

				if (VersionXML == null)
					return false;
			}
			catch
			{
				// Couldn't download xml file?
				return false;
			}

			return true;
		}

		private int XmlFileWizard(string MainFolder, string Xmlname, TranslationType type)
		{
			int ReturnValue;
			try
			{
				if (File.Exists(Path.Combine(MainFolder, "XMLs", Xmlname)))
				{
					if (File.Exists(Path.Combine(MainFolder, "XMLs", "Old", Xmlname + ".old")))
						File.Delete(Path.Combine(MainFolder, "XMLs", "Old", Xmlname + ".old"));
					File.Move(Path.Combine(MainFolder, "XMLs", Xmlname), Path.Combine(MainFolder, "XMLs", "Old", Xmlname + ".old"));
				}
				File.Move(Path.Combine(MainFolder, "XMLs", "tmp", Xmlname), Path.Combine(MainFolder, "XMLs", Xmlname));
				ReturnValue = 1;
				UpdateState(ReturnValue, type);
			}
			catch
			{
				ReturnValue = -1;
			}
			return ReturnValue;
		}

		/// <summary>
		/// Updates any translation files that differ from that found online.
		/// </summary>
		/// <param name="BaseTranslationURL">String URL folder that contains all the translation XML files.</param>
		/// <param name="Culture">Language version to download</param>
		/// <param name="TranslationsRef">Link to the translation engine to obtain current translation versions.</param>
		/// <returns>Returns a state code depending on how it ran. [-1: Error, 0: Nothing to update, 1: Update Successful]</returns>
		public int UpdateTranslations(string BaseTranslationURL, Translations TranslationsRef)
		{
			using (WebClient Client = new WebClient())
			{
				int ReturnValue = 0;
				string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

				try
				{
					if (!Directory.Exists(Path.Combine(MainFolder, "XMLs"))) Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs"));
					if (!Directory.Exists(Path.Combine(MainFolder, "XMLs", "tmp"))) Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs", "tmp"));
					if (!Directory.Exists(Path.Combine(MainFolder, "XMLs", "Old")))
						Directory.CreateDirectory(Path.Combine(MainFolder, "XMLs", "Old"));

					// In every one of these we download it to a temp folder, check if the file works, then move it over.
					if (IsOnlineVersionGreater(TranslationType.PartTranslate, TranslationsRef.JPTRsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "JPTRs.xml", Path.Combine(MainFolder, "XMLs", "tmp", "JPTRs.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "JPTRs.xml", TranslationType.PartTranslate);
					}

					if (IsOnlineVersionGreater(TranslationType.SkillDetails, TranslationsRef.SkillListVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "SkillList.xml", Path.Combine(MainFolder, "XMLs", "tmp", "SkillList.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "SkillList.xml", TranslationType.SkillDetails);
					}

					if (IsOnlineVersionGreater(TranslationType.WeaponList, TranslationsRef.WeaponListVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "WeaponList.xml", Path.Combine(MainFolder, "XMLs", "tmp", "WeaponList.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "WeaponList.xml", TranslationType.WeaponList);
					}

					if (IsOnlineVersionGreater(TranslationType.TenTreasure, TranslationsRef.TenListVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "TenList.xml", Path.Combine(MainFolder, "XMLs", "tmp", "TenList.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "TenList.xml", TranslationType.TenTreasure);
					}

					if (IsOnlineVersionGreater(TranslationType.BulletMake, TranslationsRef.BulletListVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "BulletList.xml", Path.Combine(MainFolder, "XMLs", "tmp", "BulletList.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "BulletList.xml", TranslationType.BulletMake);
					}

					if (IsOnlineVersionGreater(TranslationType.ItemIdx, TranslationsRef.BulletListVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "ItemList.xml", Path.Combine(MainFolder, "XMLs", "tmp", "ItemList.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "ItemList.xml", TranslationType.ItemIdx);
					}
				}
				catch
				{
					// Failed to download files.
					return -1;
				}
				if (Directory.Exists(Path.Combine(MainFolder, "XMLs", "tmp")))
					Directory.Delete(Path.Combine(MainFolder, "XMLs", "tmp"), true);

				return ReturnValue;
			}
		}

		/// <summary>
		/// Uses the downloaded Version XML document to return a specific version number as a string.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="bGetURL">If true, returns the URL of the online file instead of the version.</param>
		/// <returns>String value of either the Version or URL to the file.</returns>
		public string GetOnlineVersion(TranslationType Type, bool bGetURL = false)
		{
			if (VersionXML == null)
				return "";

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = !bGetURL ? "Version" : "URL";

			switch (Type)
			{
				case TranslationType.App:
					return Versions.Where(x => x.Element("Name").Value.Equals("App")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.PartTranslate:
					return Versions.Where(x => x.Element("Name").Value.Equals("JPTRs")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.SkillDetails:
					return Versions.Where(x => x.Element("Name").Value.Equals("Skills")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.WeaponList:
					return Versions.Where(x => x.Element("Name").Value.Equals("Weapons")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.TenTreasure:
					return Versions.Where(x => x.Element("Name").Value.Equals("Treasures")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.BulletMake:
					return Versions.Where(x => x.Element("Name").Value.Equals("Bullets")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.ItemIdx:
					return Versions.Where(x => x.Element("Name").Value.Equals("Items")).FirstOrDefault().Element(ElementName).Value;

			}
			return "";
		}

		/// <summary>
		/// Conditional function to determine whether the supplied version is greater than the one found online.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="LocalVersionString">Version string of the local file to check against</param>
		/// <returns></returns>
		public bool IsOnlineVersionGreater(TranslationType Type, string LocalVersionString)
		{
			if (VersionXML == null)
				return true;

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = "Version";
			if (LocalVersionString == "알 수 없음") return false;
			else if (LocalVersionString == "없음") return false;
			Version LocalVersion = new Version(LocalVersionString);


			switch (Type)
			{
				case TranslationType.SkillDetails:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Skills")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.WeaponList:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Weapons")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.PartTranslate:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("JPTRs")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.TenTreasure:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Treasures")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.BulletMake:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Bullets")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.ItemIdx:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Items")).FirstOrDefault().Element(ElementName).Value)) < 0;

			}

			return false;
		}

	}
}