using Grandcypher.Models;
using Livet;

namespace Grandcypher
{
	public class GrandcypherClient : NotificationObject
	{
		#region singleton

		private static GrandcypherClient current = new GrandcypherClient();

		public static GrandcypherClient Current
		{
			get { return current; }
		}

		#endregion

		public delegate void EventHandler();
		public EventHandler MessageSend;
		public string AppMessage { get; private set; }

		public GrandcypherProxy Proxy { get; private set; }
		public GlobalKeyCore GlobalKeyCore { get; set; }
		public GreetHooker GreetHooker { get; private set; }
		public ScenarioHooker ScenarioHooker { get; private set; }
		public Updater Updater { get; private set; }
		public Translations Translations { get; private set; }
		public WeaponHooker WeaponHooker { get; private set; }
		//public NoticeHooker NoticeHooker { get; private set; }
		public EnhancementHooker EnhancementHooker { get; private set; }
		public BookmakerHooker BookmakerHooker { get; private set; }
		public TreasureHooker TreasureHooker { get; private set; }

		private GrandcypherClient()
		{
			this.Initialieze();
		}

		public void Initialieze()
		{
			var proxy = this.Proxy ?? (this.Proxy = new GrandcypherProxy());
			this.GlobalKeyCore = new GlobalKeyCore();
			this.GreetHooker = new GreetHooker();
			this.ScenarioHooker = new ScenarioHooker();
			this.Updater = new Updater();
			this.Translations = new Translations();
			this.WeaponHooker = new WeaponHooker();
			//this.NoticeHooker = new NoticeHooker();
			this.EnhancementHooker = new EnhancementHooker();
			this.BookmakerHooker = new BookmakerHooker();
			this.TreasureHooker = new TreasureHooker();
		}
		public void PostMan(string str)
		{
			this.AppMessage = str;
			this.MessageSend();
		}
	}
}
