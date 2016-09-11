using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GranBlueHelper.Models.Notifier
{
	public class TimeLord
	{
		#region singleton

		private static TimeLord current = new TimeLord();
		private int BeforeHour { get; set; }

		public static TimeLord Current
		{
			get { return current; }
		}

		#endregion

		static Timer Mastertimer;

		public TimeLord()
		{
			current = this;
		}
		public void Init()
		{
			BeforeHour = DateTime.Now.Hour;

			TimerCallback Mastertc = new TimerCallback(MasterCounter);
			Mastertimer = new Timer(Mastertc, null, 0, 1000);
		}
		private void MasterCounter(object stateinfo)
		{
			int now = DateTime.Now.Hour;
			if (BeforeHour != now)
			{
				MainNotifier.Current.Show("정각알림", now + "시입니다!!", () => App.ViewModelRoot.Activate());
			}
			BeforeHour = now;
		}
	}
}
