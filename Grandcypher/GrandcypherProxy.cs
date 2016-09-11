using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fiddler;
using Livet;
using Newtonsoft.Json.Linq;

namespace Grandcypher
{
	/// <summary>
	/// json 파싱 : http://globalbiz.tistory.com/88
	/// </summary>
	public partial class GrandcypherProxy : NotificationObject
	{

		#region IsStarted

		private bool _IsStarted;

		public bool IsStarted
		{
			get { return this._IsStarted; }
			set
			{
				if (this._IsStarted != value)
				{
					this._IsStarted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		public GrandcypherProxy()
		{
			FiddlerApplication.AfterSessionComplete += delegate (Fiddler.Session oS)
			{
#if DEBUG
				if (oS.uriContains("gbf") || oS.uriContains("game.granbluefantasy"))
				{
					if (!oS.PathAndQuery.StartsWith("/assets") && !oS.PathAndQuery.StartsWith("/security"))
					{
						Console.WriteLine("--------------------------------------------------------------------");
						var temp = oS.PathAndQuery.Split('?');
						Console.WriteLine(temp[0]);
						Console.WriteLine(oS.oResponse.MIMEType);
					}
					if (!oS.PathAndQuery.StartsWith("/security"))
					{
						Console.WriteLine("--------------------------------------------------------------------");
						var temp = oS.PathAndQuery.Split('?');
						Console.WriteLine(temp[0]);
						Console.WriteLine(oS.oResponse.MIMEType);
					}
				}
#endif
				if ((oS.uriContains("gbf") || oS.uriContains("game.granbluefantasy")) && !oS.uriContains("/security") && !oS.uriContains("/assets"))
				{
					GrandcypherClient.Current.GreetHooker.SessionReader(oS);
					GrandcypherClient.Current.ScenarioHooker.SessionReader(oS);
					//GrandcypherClient.Current.WeaponHooker.SessionReader(oS);
					//GrandcypherClient.Current.NoticeHooker.SessionReader(oS);
					GrandcypherClient.Current.EnhancementHooker.SessionReader(oS);
					GrandcypherClient.Current.BookmakerHooker.SessionReader(oS);
					GrandcypherClient.Current.TreasureHooker.SessionReader(oS);
					GrandcypherClient.Current.BulletMaster.SessionReader(oS);
				}
			};
		}
		public void StartUp(int portnum)
		{
			IsStarted = true;
			if (portnum < 0 || portnum > 65535)
			{
				GrandcypherClient.Current.RaiseError(ErrorKind.PortError);
				FiddlerApplication.Startup(37564, true, false, false);
			}
			else FiddlerApplication.Startup(portnum, true, false, false);
		}
		public void Quit()
		{
			IsStarted = false;
			FiddlerApplication.Shutdown();
		}
	}
}
