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
		public static Dictionary<string, int> ElementTable = new Dictionary<string, int>
		{
			{ "화",1 }, {"수",2 },{"토",3 },{"풍",4 },{"광",5 },{"암",6 }
		};
		public List<string> ElementKind { get; set; }
		private string _SelectedElement;
		public string SelectedElement
		{
			get { return this._SelectedElement; }
			set
			{
				if (this._SelectedElement == value) return;
				this._SelectedElement = value;
				Settings.Current.ElementSetting = value;
				this.RemoveCompleteTreasure();

				this.RaisePropertyChanged();
			}
		}
		public CalcTenViewModel()
		{
			this.ElementKind = new List<string>(ElementTable.Keys.ToList());
			if (Settings.Current.ElementSetting == null) Settings.Current.ElementSetting = "화";
			this.SelectedElement = Settings.Current.ElementSetting;

			this.Proto = Settings.Current.Proto;
			this.Rust = Settings.Current.Rust;
			this.Element = Settings.Current.Element;
			this.First = Settings.Current.First;
			this.Second = Settings.Current.Second;
			this.Third = Settings.Current.Third;
			this.Fourth = Settings.Current.Fourth;
			this.Fifth = Settings.Current.Fifth;

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
			List<TenTreasureInfo> ElementList = targetList.Where(x => x.ElementID != 0).ToList();
			List<TenTreasureInfo> AnotherList = targetList.Where(x => x.ElementID == 0).ToList();

			var Origin = ElementList.Where(x => x.Origin == 1).ToList();
			var Magna = ElementList.Where(x => x.Origin == 2).ToList();
			List<TenTreasureInfo> ProtoList = new List<TenTreasureInfo>();


			for (int i = 0; i < Origin.Count; i++)
			{
				if (Origin[i].ElementID == ElementTable[this.SelectedElement])
				{
					Origin[i].Element += 3;
					Origin[i].Second += 3;
					Origin[i].Fourth += 3;
				}
			}

			for (int i = 0; i < Magna.Count; i++)
			{
				if (Magna[i].ElementID == ElementTable[this.SelectedElement])
					Magna[i].Fifth += 60;
			}

			ProtoList = ElementList.Where(x => x.Proto > 0).ToList();

			switch (ElementTable[this.SelectedElement])
			{
				case 1:
					ElementList = ElementList.Where(x => x.ElementID == 1 && x.Origin == 0).ToList();
					break;
				case 2:
					ElementList = ElementList.Where(x => x.ElementID == 2 && x.Origin == 0).ToList();
					break;
				case 3:
					ElementList = ElementList.Where(x => x.ElementID == 3 && x.Origin == 0).ToList();
					break;
				case 4:
					ElementList = ElementList.Where(x => x.ElementID == 4 && x.Origin == 0).ToList();
					break;
				case 5:
					ElementList = ElementList.Where(x => x.ElementID == 5 && x.Origin == 0).ToList();
					break;
				case 6:
					ElementList = ElementList.Where(x => x.ElementID == 6 && x.Origin == 0).ToList();
					break;
			}
			targetList = new List<TenTreasureInfo>(AnotherList.Concat(ElementList).Concat(Magna).Concat(Origin).ToList());
			if (ElementTable[this.SelectedElement] != 5)
			{
				List<TenTreasureInfo> outputList = new List<TenTreasureInfo>();
				foreach (var item in ProtoList)
				{
					TenTreasureInfo tempoutput = new TenTreasureInfo
					{
						Name = item.Name,
						Proto = item.Proto
					};
					outputList.Add(tempoutput);
				}
				targetList = new List<TenTreasureInfo>(outputList.Concat(targetList).ToList());
			}
			//temp=현재 보유하고 있는 아이템목록
			//targetlist=정리된 십천중 필요목록
			targetList = new List<TenTreasureInfo>(GrandcypherClient.Current.Translations.SetItemIdxList(targetList));
			foreach (var item in targetList)
			{
				var search = temp.Where(x => x.ItemID == item.idx).ToList();
				TreasureInfo target = new TreasureInfo();

				//아이템 보유목록에 필요목록에 있는 아이템이 존재하는 경우
				if (search.Count > 0)
				{
					target = search[0];
				}
				else//아이템이 비활성화인 경우 기초값으로 생성
				{
					target = new TreasureInfo
					{
						Name = item.Name,
						count = 0,
						ItemID = -1,
					};
				}
				if (target.Name == null || target.Name == string.Empty) continue;
				target.max = 0;//max값을 초기화

				//max값에 필요 아이템값을 모두 합산
				if (!Proto && item.Proto > 0)
					target.max += item.Proto;
				if (!Rust && item.Rust > 0)
					target.max += item.Rust;
				if (!Element && item.Element > 0)
					target.max += item.Element;
				if (!First && item.First > 0)
					target.max += item.First;
				if (!Second && item.Second > 0)
					target.max += item.Second;
				if (!Third && item.Third > 0)
					target.max += item.Third;
				if (!Fourth && item.Fourth > 0)
					target.max += item.Fourth;
				if (!Fifth && item.Fifth > 0)
					target.max += item.Fifth;

				target.max += item.Sixth;
				target.result = target.max - target.count;

				if (target.result > 0)
					result.Add(target);
			}
			this.TreasureList = new List<TreasureInfo>(result);
		}
	}
}
