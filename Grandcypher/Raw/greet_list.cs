using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grandcypher.Raw
{
	public class greet_list
	{
		public int count { get; set; }
		public int first { get; set; }
		public int last { get; set; }
		public int next { get; set; }
		public int prev { get; set; }
		public Detail list { get; set; }
	}
	public class Detail
	{
		public List<UserIf> UserIf { get; set; }
	}
	public class UserIf
	{
		public bool delete_flg { get; set; }
		public string from_greet_time { get; set; }
		public UserComment UserComment { get; set; }
		public string from_user_image { get; set; }
		public string from_user_level { get; set; }
		public string from_user_name { get; set; }
		public string from_user_url { get; set; }
		public int id { get; set; }
		public int platform_id { get; set; }
	}
	public class UserComment
	{
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		public bool is_stamp { get; set; }
		public string text { get; set; }
		public string TrText { get; set; }
		public string StampUrl { get; set; }
		public string StampFileName { get; set; }
		public ImageSource LocalImage
		{
			get
			{
				if (this.StampFileName != null)
				{
					if (File.Exists(Path.Combine(MainFolder,"Stamps", this.StampFileName)))
						return new BitmapImage(new Uri(Path.Combine(MainFolder,"Stamps", this.StampFileName),UriKind.Absolute));
					else return null;
				}
				else return null;
			}
		}
	}
}
