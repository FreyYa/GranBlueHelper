using GranBlueHelper.Models;
using Grandcypher;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.ViewModels
{
	public class CalcTenViewModel : ViewModel
	{
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

		#region 십천중 계산 설정

		private bool _Proto;
		public bool Proto
		{
			get { return this._Proto; }
			set
			{
				if (this._Proto == value) return;
				this._Proto = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Proto = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Rust;
		public bool Rust
		{
			get { return this._Rust; }
			set
			{
				if (this._Rust == value) return;
				this._Rust = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Rust = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Element;
		public bool Element
		{
			get { return this._Element; }
			set
			{
				if (this._Element == value) return;
				this._Element = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Element = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _First;
		public bool First
		{
			get { return this._First; }
			set
			{
				if (this._First == value) return;
				this._First = value;
				this.RemoveCompleteTreasure();
				Settings.Current.First = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Second;
		public bool Second
		{
			get { return this._Second; }
			set
			{
				if (this._Second == value) return;
				this._Second = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Second = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Third;
		public bool Third
		{
			get { return this._Third; }
			set
			{
				if (this._Third == value) return;
				this._Third = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Third = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Fourth;
		public bool Fourth
		{
			get { return this._Fourth; }
			set
			{
				if (this._Fourth == value) return;
				this._Fourth = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Fourth = value;

				this.RaisePropertyChanged();
			}
		}
		private bool _Fifth;
		public bool Fifth
		{
			get { return this._Fifth; }
			set
			{
				if (this._Fifth == value) return;
				this._Fifth = value;
				this.RemoveCompleteTreasure();
				Settings.Current.Fifth = value;

				this.RaisePropertyChanged();
			}
		}

		#endregion

		public CalcTenViewModel()
		{
			this.Read();
		}
		private void Read()
		{

			GrandcypherClient.Current.TreasureHooker.TreasureReadEnd += () =>
			{
				this.RemoveCompleteTreasure();
			};
		}
		private void RemoveCompleteTreasure()
		{
			if (GrandcypherClient.Current.TreasureHooker.CurrentTreasureList == null) return;
			if (GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Count < 1) return;

			List<TreasureInfo> temp = new List<TreasureInfo>(GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Values);
			List<TreasureInfo> result = new List<TreasureInfo>();
			


			List<TenTreasureInfo> targetList = new List<TenTreasureInfo>(GrandcypherClient.Current.Translations.GetTreasureList());

			foreach (var item in targetList)
			{
				var search = temp.Where(x => x.Name == item.Name).ToList();
				if (search.Count > 0)
				{
					var target = search[0];

					target.max = 0;//max값을 초기화

					//max값에 필요 아이템값을 모두 합산
					if (!Proto)
						target.max += item.Proto;
					if (!Rust)
						target.max += item.Rust;
					if (!Element)
						target.max += item.Element;
					if (!First)
						target.max += item.First;
					if (!Second)
						target.max += item.Second;
					if (!Third)
						target.max += item.Third;
					if (!Fourth)
						target.max += item.Fourth;
					if (!Fifth)
						target.max += item.Fifth;

					target.max += item.Sixth;

					target.result = target.max - target.count;

					if (target.result > 0)
						result.Add(target);
				}
			}
			this.TreasureList = new List<TreasureInfo>(result);
		}
	}
}
