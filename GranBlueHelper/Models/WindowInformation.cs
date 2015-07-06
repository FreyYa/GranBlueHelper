using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.Models
{
	public class RectItem
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	}
	public class HourMin
	{
		public int Hour { get; set; }
		public int Minute { get; set; }
		public bool IsOverDay { get; set; }
		public bool IsPCShutDown { get; set; }
	}
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}
	public class WindowSize
	{
		public int left { get; set; }
		public int top { get; set; }
		public int right { get; set; }
		public int bottom { get; set; }
	}
}
