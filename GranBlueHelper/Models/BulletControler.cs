using Grandcypher;
using Grandcypher.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.Models
{
	public class BulletControler : ViewModel
	{
		/// <summary>
		/// 총알 컨트롤러의 ID입니다. 다른 컨트롤러와의 구별을 위해 사용합니다.
		/// </summary>
		private int Idx { get; set; }

		#region EventHandler
		/// <summary>
		/// 이벤트 핸들러
		/// </summary>
		public delegate void EventHandler();
		static public EventHandler Changed;
		#endregion

		/// <summary>
		/// Bullet 종류 Table
		/// </summary>
		public static Dictionary<string, int> BulletCatTable = new Dictionary<string, int>
		{
			{ "패러벨럼",1 }, {"라이플",2 },{"카트리지",3 },{"에테리얼",4 }
		};


		/// <summary>
		/// Bullet 데이터 리스트
		/// </summary>
		public List<Bullet> MasterBullet { get; set; }


		/// <summary>
		/// Bullet 종류 목록
		/// </summary>
		public IEnumerable<string> BulletCatList { get { return BulletCatTable.Keys; } }

		#region Title
		private string _Title;
		public string Title
		{
			get { return this._Title; }
			set
			{
				if (this._Title == value) return;
				this._Title = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region CurrentStatus
		private int _CurrentStatus;
		public int CurrentStatus
		{
			get { return this._CurrentStatus; }
			set
			{
				if (this._CurrentStatus == value) return;
				this._CurrentStatus = value;
				Changed();
				this.RaisePropertyChanged();
			}
		}


		private List<int> _CurrentStatusList;
		public List<int> CurrentStatusList
		{
			get { return this._CurrentStatusList; }
			set
			{
				this._CurrentStatusList = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Bullet Cat Selected
		private string _BulletCat;
		public string BulletCat
		{
			get { return this._BulletCat; }
			set
			{
				if (this._BulletCat == value || value == null) return;
				this._BulletCat = value;

				int idx = BulletCatTable[this._BulletCat];
				switch (idx)
				{
					case 1:
						this.MasterBullet = new List<Bullet>(GrandcypherClient.Current.Translations.GetBulletList().Where(x => x.BulletKind == 1));
						break;
					case 2:
						this.MasterBullet = new List<Bullet>(GrandcypherClient.Current.Translations.GetBulletList().Where(x => x.BulletKind == 2));
						break;
					case 3:
						this.MasterBullet = new List<Bullet>(GrandcypherClient.Current.Translations.GetBulletList().Where(x => x.BulletKind == 3));
						break;
					case 4:
						this.MasterBullet = new List<Bullet>(GrandcypherClient.Current.Translations.GetBulletList().Where(x => x.BulletKind == 4));
						break;
					default:
						break;
				}

				FirstStrList = new List<string>();
				SecondStrList = new List<string>();
				FirstStrList = new List<string>(ConvertString(this.MasterBullet.Where(x => x.Rank == 1)));
				this.First = FirstStrList.FirstOrDefault();

				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region String List
		private List<string> _FirstStrList;
		public List<string> FirstStrList
		{
			get { return this._FirstStrList; }
			set
			{
				this._FirstStrList = value;
				this.RaisePropertyChanged();
			}
		}


		private List<string> _SecondStrList;
		public List<string> SecondStrList
		{
			get { return this._SecondStrList; }
			set
			{
				this._SecondStrList = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region String
		private string _First;
		public string First
		{
			get { return this._First; }
			set
			{
				if (this._First == value || value == null) return;
				this._First = value;
				var index = this.MasterBullet.Where(x => x.FullName == value).FirstOrDefault().MasterID;
				this.SecondStrList = new List<string>(ConvertString(this.MasterBullet.Where(x => x.MasterID == index)));
				this.Second = this.SecondStrList.FirstOrDefault();

				this.RaisePropertyChanged();
			}
		}

		private string _Second;
		public string Second
		{
			get { return this._Second; }
			set
			{
				if (this._Second == value || value == null) return;
				this._Second = value;
#if DEBUG
				Debug.WriteLine("-------------------------------------------------");
				Debug.WriteLine(this._Second);
				foreach (var item in this.MasterBullet.Where(x => x.FullName == this._Second).FirstOrDefault().MaterialList)
				{
					Debug.WriteLine(item.Name + " : " + item.max);
				}

#endif
				ChangeTitle();
				//UpdateCurrentStatusList();
				this.RaisePropertyChanged();
			}
		}
		#endregion

		/// <summary>
		/// 총알 목록 관리 클래스
		/// </summary>
		public BulletControler(int index = -1)
		{
			this.Idx = index;
			this.BulletCat = this.BulletCatList.FirstOrDefault();
		}
		/// <summary>
		/// 총알 단계 목록을 생성합니다. 현재 쓰이지 않습니다.
		/// </summary>
		private void UpdateCurrentStatusList()
		{
			if (this.Second == null) return;

			var master_id = this.MasterBullet.Where(x => x.FullName == this.Second).FirstOrDefault().MasterID;
			var max_rank = this.MasterBullet.Where(x => x.FullName == this.Second).FirstOrDefault().Rank;

			List<int> output = new List<int>();
			output.Add(0);
			foreach (var item in this.MasterBullet)
			{
				if (item.MasterID == master_id && item.Rank < max_rank)
					output.Add(item.Rank);
			}
			//output.RemoveAt(output.Count - 1);

			this.CurrentStatusList = new List<int>(output);
			this.CurrentStatus = this.CurrentStatusList.FirstOrDefault();
		}
		/// <summary>
		/// 선택된 총알이 변경될 때 총알의 텍스트를 변경합니다.
		/// </summary>
		private void ChangeTitle()
		{
			if (Second == null || Idx < 0) this.Title = "";
			else this.Title = "#" + Idx.ToString() + " " + Second;
			Changed();
		}
		/// <summary>
		/// 트레저 목록을 가져옵니다.
		/// </summary>
		/// <returns></returns>
		public List<TreasureInfo> GetMaterialList()
		{
			if (this.Second == null) return new List<TreasureInfo>();

			List<TreasureInfo> output = new List<TreasureInfo>();
			foreach (var selcted_item in this.MasterBullet.Where(x => x.FullName == this.Second).FirstOrDefault().MaterialList)
			{
				TreasureInfo temp = new TreasureInfo();
				if (this.CurrentStatus == 0)
				{
					temp.Name = selcted_item.Name;
					temp.max = selcted_item.max;
				}
				else
				{
					int calc_max = selcted_item.max;
					var idx = this.MasterBullet.Where(x => x.FullName == this.Second).FirstOrDefault().MasterID;

					List<Bullet> temp_lst = new List<Bullet>(this.MasterBullet.Where(x => x.MasterID == idx));
					for (int i = 1; i <= this.CurrentStatus; i++)
					{
						List<TreasureInfo> mat_temp = new List<TreasureInfo>(temp_lst.Where(x => x.Rank == i).FirstOrDefault().MaterialList);

						foreach (var item in mat_temp)
						{
							if (selcted_item.Name == item.Name)
								calc_max -= item.max;
						}
					}

					temp.Name = selcted_item.Name;
					temp.max = calc_max;
				}
				output.Add(temp);
			}
			return output;
		}
		/// <summary>
		/// 목록 안의 FullName을 string List 형태로 출력해줍니다.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private List<string> ConvertString(IEnumerable<Bullet> list)
		{
			List<string> temp = new List<string>();
			foreach (var item in list)
			{
				temp.Add(item.FullName);
			}
			return temp;
		}

	}

}
