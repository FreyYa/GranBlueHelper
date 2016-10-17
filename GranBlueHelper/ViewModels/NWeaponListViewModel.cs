using Grandcypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.ViewModels
{
	public class NWeaponListViewModel
	{
		public void SaveList()
		{
			GrandcypherClient.Current.WeaponHooker.EnableListEdit = true;
		}
	}
}
