using Grandcypher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Grandcypher
{
	public class Updater
	{
		public bool JPTRsUpdate { get; set; }
		public bool WeaponListUpdate { get; set; }

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

		private int XmlFileWizard(string MainFolder, string Xmlname)
		{
			int ReturnValue;
			try
			{
				if (File.Exists(Path.Combine(MainFolder, "Translations", Xmlname)))
				{
					if (File.Exists(Path.Combine(MainFolder, "Translations", "Old", Xmlname + ".old")))
						File.Delete(Path.Combine(MainFolder, "Translations", "Old", Xmlname + ".old"));
					File.Move(Path.Combine(MainFolder, "Translations", Xmlname), Path.Combine(MainFolder, "Translations", "Old", Xmlname + ".old"));
				}
				File.Move(Path.Combine(MainFolder, "Translations", "tmp", Xmlname), Path.Combine(MainFolder, "Translations", Xmlname));
				ReturnValue = 1;
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
					if (!Directory.Exists(Path.Combine(MainFolder, "Translations"))) Directory.CreateDirectory(Path.Combine(MainFolder, "Translations"));
					if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "tmp"))) Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "tmp"));
					if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "Old")))
						Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "Old"));

					if (IsOnlineVersionGreater(TranslationsRef.JPTRsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "JPTRs.xml", Path.Combine(MainFolder, "Translations", "tmp", "JPTRs.xml"));
						ReturnValue = XmlFileWizard(MainFolder, "JPTRs.xml");
					}
				}
				catch
				{
					// Failed to download files.
					return -1;
				}
				if (Directory.Exists(Path.Combine(MainFolder, "Translations", "tmp")))
					Directory.Delete(Path.Combine(MainFolder, "Translations", "tmp"), true);

				return ReturnValue;
			}
		}

		/// <summary>
		/// Uses the downloaded Version XML document to return a specific version number as a string.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="bGetURL">If true, returns the URL of the online file instead of the version.</param>
		/// <returns>String value of either the Version or URL to the file.</returns>
		public string GetOnlineVersion(bool bGetURL = false)
		{
			if (VersionXML == null)
				return "";

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = !bGetURL ? "Version" : "URL";


			return Versions.Where(x => x.Element("Name").Value.Equals("JPTRs")).FirstOrDefault().Element(ElementName).Value;
			//return "";
		}

		/// <summary>
		/// Conditional function to determine whether the supplied version is greater than the one found online.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="LocalVersionString">Version string of the local file to check against</param>
		/// <returns></returns>
		public bool IsOnlineVersionGreater(string LocalVersionString)
		{
			if (VersionXML == null)
				return true;

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = "Version";
			if (LocalVersionString == "알 수 없음") return false;
			else if (LocalVersionString == "없음") return false;
			Version LocalVersion = new Version(LocalVersionString);


			return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("JPTRs")).FirstOrDefault().Element(ElementName).Value)) < 0;

			//return false;
		}

	}
}