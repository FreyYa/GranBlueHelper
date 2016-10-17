using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher.Raw
{
	public class Weapon
	{
		public Weapon()
		{
			this.param = new Param();
			this.master = new Master();
		}
		public bool IsExclude { get; set; }
		public bool IsUsed { get; set; }
		public Master master { get; set; }
		public Param param { get; set; }
	}
	public class Master
	{
		public int archaic { get; set; }
		public int default_open { get; set; }
		public int id { get; set; }
		public dynamic is_group { get; set; }
		public int max_evolution_level { get; set; }
		public int rarity { get; set; }
		public int release_max_evolution_level { get; set; }
		public int skill1_display {  get; set; }
		public string skill1_image {  get; set; }
		public int skill2_display {  get; set; }
		public string skill2_image {  get; set; }
		public int sold_price { get; set; }
	}
	public class Param
	{
		public Bullet bullet { get; set; }
		public int evolution { get; set; }
		public int id { get; set; }
		public int is_locked { get; set; }
		public bool is_user_level { get; set; }
		public int quality { get; set; }
		public int SkillLv { get; set; }
		public int AttStatus { get; set; }
		public int HPStatus { get; set; }
	}
	public class Bullet
	{
		public int max_set_count { get; set; }
		public int this_set_count { get; set; }
	}
}