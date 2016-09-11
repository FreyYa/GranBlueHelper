using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher
{
	public class ResultHooker
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler EndBattle;
		#endregion
		public void SessionReader(Session oS)
		{
			if (oS.PathAndQuery.Contains("result/content/index") && oS.oResponse.MIMEType == "application/json")
				ResultHook(oS);
			if (oS.PathAndQuery.Contains("resultmulti/content/index") && oS.oResponse.MIMEType == "application/json")
				ResultHook(oS);
		}
		/// <summary>
		/// 전투 종료를 감지
		/// </summary>
		/// <param name="oS"></param>
		private void ResultHook(Session oS)
		{
			this.EndBattle();
		}
	}
}
