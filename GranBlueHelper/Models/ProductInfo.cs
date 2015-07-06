﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace GranBlueHelper
{
	public class ProductInfo
	{
		private readonly Assembly assembly = Assembly.GetExecutingAssembly();
		private string _Title;
		private string _Description;
		private string _Company;
		private string _Product;
		private string _Copyright;
		private string _Trademark;
		private Version _Version;
		private string _VersionString;
		private IReadOnlyCollection<Library> _Libraries;

		public string Title
		{
			get { return this._Title ?? (this._Title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyTitleAttribute))).Title); }
		}

		public string Description
		{
			get { return this._Description ?? (this._Description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyDescriptionAttribute))).Description); }
		}

		public string Company
		{
			get { return this._Company ?? (this._Company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyCompanyAttribute))).Company); }
		}

		public string Product
		{
			get { return this._Product ?? (this._Product = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyProductAttribute))).Product); }
		}

		public string Copyright
		{
			get { return this._Copyright ?? (this._Copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyCopyrightAttribute))).Copyright); }
		}

		public string Trademark
		{
			get { return this._Trademark ?? (this._Trademark = ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyTrademarkAttribute))).Trademark); }
		}

		public Version Version
		{
			get { return this._Version ?? (this._Version = this.assembly.GetName().Version); }
		}

		public string VersionString
		{
			get { return this._VersionString ?? (this._VersionString = string.Format("{0}{1}{2}", this.Version.ToString(3), this.IsBetaRelease ? " β" : "", this.Version.Revision == 0 ? "" : " rev." + this.Version.Revision)); }
		}

		public bool IsBetaRelease
		{
			get { return false; }
		}


		public IReadOnlyCollection<Library> Libraries
		{
			get
			{
				return this._Libraries ?? (this._Libraries = new List<Library>
				{
					new Library("Reactive Extensions", new Uri("http://rx.codeplex.com/")),
					new Library("Interactive Extensions", new Uri("http://rx.codeplex.com/")),
					new Library("Windows API Code Pack", new Uri("http://archive.msdn.microsoft.com/WindowsAPICodePack")),
					new Library("Livet", new Uri("http://ugaya40.net/livet")),
					new Library("FiddlerCore", new Uri("http://fiddler2.com/fiddlercore")),
					new Library("Json.Net ", new Uri("http://www.newtonsoft.com/json")),
					new Library("MetroRadiance ", new Uri("https://github.com/grabacr07/MetroRadiance")),
					new Library("Desktop.Metro ", new Uri("https://github.com/Grabacr07/KanColleViewer/tree/master/Grabacr07.Desktop.Metro")),

				});
			}
		}
	}

	public class Library
	{
		public string Name { get; private set; }
		public Uri Url { get; private set; }

		public Library(string name, Uri url)
		{
			this.Name = name;
			this.Url = url;
		}
	}
}
