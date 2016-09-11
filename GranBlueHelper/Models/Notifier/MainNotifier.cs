using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GranBlueHelper.Models.Notifier
{
	public class MainNotifier
	{
		#region singleton

		private static MainNotifier current = new MainNotifier();

		public static MainNotifier Current
		{
			get { return current; }
		}

		#endregion
		public MainNotifier()
		{
			current = this;
		}
		CustomSound sound = new CustomSound();
		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;
		public void Dispose()
		{
			this.notifyIcon.Dispose();
		}
		public bool IsNotiInit
		{
			get
			{
				if (this.notifyIcon == null) return false;
				else return true;
			}
			
		}
		public void Initialize()
		{
			const string iconUri = "pack://application:,,,/GrandcypherGear;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
				return;

			var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
			if (streamResourceInfo == null)
				return;

			using (var stream = streamResourceInfo.Stream)
			{
				this.notifyIcon = new NotifyIcon
				{
					Text = App.ProductInfo.Title,
					Icon = new Icon(stream),
					Visible = true,
				};
			}
		}
		public void Show(string header, string body, Action activated, Action<Exception> failed = null)
		{
			if (this.notifyIcon == null)
				return;

			if (activated != null)
			{
				this.notifyIcon.BalloonTipClicked -= this.activatedAction;

				this.activatedAction = (sender, args) => activated();
				this.notifyIcon.BalloonTipClicked += this.activatedAction;
			}
			if (Settings.Current.NotiOn) sound.SoundOutput(header, false);
			notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
		}
	}
}
