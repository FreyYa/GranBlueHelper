using Grandcypher;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.ViewModels
{
	public class CalcTenViewModel:ViewModel
	{
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
		public CalcTenViewModel()
		{
			this.Read();
		}
		private void Read()
		{

			GrandcypherClient.Current.TreasureHooker.TreasureReadEnd += () =>
			{
				this.RemoveCompleteTreasure();
				//this.TreasureList = new List<TreasureInfo>(GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Values);
			};
		}
		private void RemoveCompleteTreasure()
		{
			List<TreasureInfo> temp = new List<TreasureInfo>(GrandcypherClient.Current.TreasureHooker.CurrentTreasureList.Values);

			List<TenTreasureInfo> targetList = new List<TenTreasureInfo>(GrandcypherClient.Current.Translations.GetTreasureList());


		}
	}
}
