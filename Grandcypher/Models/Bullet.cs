using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher.Models
{
	public class Bullet
	{
		public int MasterID { get; set; }
		public string Name { get; set; }
		public string TrName { get; set; }
		public string FullName
		{
			get { return this.TrName + "[" + this.Name + "]"; }
		}
		public int BulletKind { get; set; }
		public int Rank { get; set; }
		public List<TreasureInfo> MaterialList { get; set; }
	}
}
