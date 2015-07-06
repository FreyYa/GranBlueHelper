using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher.Raw
{
	public class WeaponInfo
	{
		public param param { get; set; }
		public master master { get; set; }
		public bool is_used { get; set; }
	}
	public class param
	{
		public int id { get; set; }
		public int status { get; set; }
		public bool is_user_level { get; set; }
		public int quality { get; set; }
		public int evolution { get; set; }
		public int is_locked { get; set; }
	}
	public class master
	{
		public int id { get; set; }
		public int rarity { get; set; }
		public int sold_price { get; set; }
		public int max_evolution_level { get; set; }
	}
}
