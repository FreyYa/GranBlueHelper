using Grandcypher;
using Grandcypher.Raw;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GranBlueHelper.ViewModels
{
	public class MessageViewViewModel : ViewModel
	{
		#region 메시지 리스트
		private List<UserIf> _GreetList;
		public List<UserIf> GreetList
		{
			get { return this._GreetList; }
			set
			{
				if (this._GreetList != value)
				{
					this._GreetList = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private Visibility _GreetEnable;
		public Visibility GreetEnable
		{
			get { return this._GreetEnable; }
			set
			{
				if (!Equals(this._GreetEnable, value))
				{
					this._GreetEnable = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion
		#region 로딩화면

		private Visibility _LoadingScreen;
		public Visibility LoadingScreen
		{
			get { return this._LoadingScreen; }
			set
			{
				if (!Equals(this._LoadingScreen, value))
				{
					this._LoadingScreen = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private string _Progress;
		public string Progress
		{
			get { return this._Progress; }
			set
			{
				if (!Equals(this._Progress, value))
				{
					this._Progress = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public MessageViewViewModel()
		{
			this.GreetEnable = Visibility.Collapsed;
			this.LoadingScreen = Visibility.Collapsed;
			Read();
		}
		private void Read()
		{
			GrandcypherClient.Current.GreetHooker.ReadStart += () =>
			{
				this.LoadingScreen = Visibility.Visible;
			};
			GrandcypherClient.Current.GreetHooker.TranslatieEnd += () =>
			{
				this.GreetList = new List<UserIf>(GrandcypherClient.Current.GreetHooker.GreetList);
				this.LoadingScreen = Visibility.Collapsed;
				this.GreetEnable = Visibility.Visible;
			};
			GrandcypherClient.Current.GreetHooker.ProgressBar += () =>
			{
				string temp = GrandcypherClient.Current.GreetHooker.ProgressStatus.Max.ToString() + "개중 " +
					GrandcypherClient.Current.GreetHooker.ProgressStatus.Current.ToString() + "개 번역완료";
				GrandcypherClient.Current.PostMan(temp);
				this.Progress = temp;
			};
		}
	}
}
