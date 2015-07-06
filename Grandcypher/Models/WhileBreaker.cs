using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher.Models
{
	public class GlobalKeyCore : NotificationObject
	{
		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		public EventHandler ScreenShot;

		#endregion
		public GlobalKeyCore()
		{

		}
		public void TakeScreenShot()
		{
			this.ScreenShot();
		}

	}
}
