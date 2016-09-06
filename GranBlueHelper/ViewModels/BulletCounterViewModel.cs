using GranBlueHelper.Models;
using Grandcypher;
using Grandcypher.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.ViewModels
{
	public class BulletCounterViewModel : ViewModel
	{
		#region 목록


		private List<BulletControler> _CustomList;
		public List<BulletControler> CustomList
		{
			get { return this._CustomList; }
			set
			{
				this._CustomList = value;
				CalcTreasure();
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region 트레저 목록
		private List<TreasureInfo> _TreasureList;
		public List<TreasureInfo> TreasureList
		{
			get { return this._TreasureList; }
			set
			{
				if (this._TreasureList == value) return;
				this._TreasureList = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		private int idx { get; set; }


		public BulletCounterViewModel()
		{
			this.CustomList = new List<BulletControler>();
			TreasureList = new List<TreasureInfo>();
			idx = 0;
			BulletControler.Changed += () =>
			{
				this.CalcTreasure();
			};
			GrandcypherClient.Current.BulletMaster.ItemReadEnd += () =>
			{
				this.CalcTreasure();
			};
		}
		public void AddBullet()
		{
			idx++;
			BulletControler temp_con = new BulletControler(idx);
			this.CustomList.Add(temp_con);

			var temp = this.CustomList;
			this.CustomList = new List<BulletControler>(temp);
		}
		public void RemoveBullet()
		{
			if (idx < 1) return;
			this.CustomList.RemoveAt(idx - 1);

			var temp = this.CustomList;
			this.CustomList = new List<BulletControler>(temp);
			idx--;
			if (this.CustomList.Count == 0)
				this.TreasureList = new List<TreasureInfo>();
		}
		private void CalcTreasure()
		{
			if (this.CustomList.Count < 1 || GrandcypherClient.Current.TreasureHooker.CurrentTreasureList == null) return;

			this.TreasureList = new List<TreasureInfo>();
			List<TreasureInfo> temp_lst = new List<TreasureInfo>();

			foreach (var item in this.CustomList)
			{
				var customtemp = item.GetMaterialList();
				temp_lst = temp_lst.Concat(customtemp).ToList();
			}
			foreach (var item in temp_lst)
			{
				TreasureInfo treasure_temp = new TreasureInfo();
				if (TreasureList.Any(x => x.ItemID == item.ItemID))
				{
					TreasureList.Find(x => x.ItemID == item.ItemID).max += item.max;
				}
				else
				{
					treasure_temp.Name = item.Name;
					treasure_temp.max = item.max;
					treasure_temp.ItemID = item.ItemID;
					TreasureList.Add(treasure_temp);
				}
			}
			List<TreasureInfo> calcList = new List<TreasureInfo>();
			//GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Values= 현재 트래저 목록
			//TreasureList= 설정에 따라 불러온 트래저 목록
			foreach (var current in GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Values)
			{
				foreach (var target in TreasureList)
				{
					if (target.ItemID == current.ItemID)
					{
						TreasureInfo temp_t = new TreasureInfo();

						temp_t.Name = current.Name;
						temp_t.result = target.max - current.count;
						temp_t.ItemID = current.ItemID;

						if (temp_t.result > 0) calcList.Add(temp_t);
					}
				}
			}
			List<TreasureInfo> modified_list = new List<TreasureInfo>(calcList);
			foreach (var item in TreasureList)
			{
				if (GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Values.Where(x => x.ItemID == item.ItemID).FirstOrDefault() == default(TreasureInfo))
				{
					TreasureInfo temp_t = new TreasureInfo();

					temp_t.Name = item.Name;
					temp_t.result = item.max;
					temp_t.ItemID = item.ItemID;
					if (temp_t.result > 0) modified_list.Add(temp_t);
				}
			}
			this.TreasureList = new List<TreasureInfo>(modified_list);
		}
	}
}
