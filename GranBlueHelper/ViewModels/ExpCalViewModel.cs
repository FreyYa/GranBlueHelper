using Grandcypher;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranBlueHelper.ViewModels
{
	public class ExpCalViewModel : ViewModel
	{
		public static int[] EquipExpTable = new int[]
		{
			0,0,10,22,36,52,70,90,112,136,162,
			190,220,255,295,340,390,450,520,600,690,
			790,910,1050,1210,1390,1590,1810,2050,2310,2590,
			2890,3210,3550,3910,4290,4690,5110,5550,6010,6490,
			6990,7510,8050,8610,9190,9790,10410,11050,11710,12390,
			13090,13810,14550,15310,16090,16890,17710,18550,19410,20290,
			21190,22110,23050,24010,24990,25990,27010,28050,29110,30190,
			31290,32410,33550,34710,35890,37090,38310,39550,40810,42090,
			43390,44710,46050,47410,48790,50190,51610,53050,54510,55990,
			57490,59010,60550,62110,63690,65290,66910,68550,70210,71890
		};
		public static int[] CharExpTable = new int[]
		{
			0,0,30,100,200,320,460,620,800,1000,1220,
			1460,1720,2000,2300,2650,3050,3500,4000,4550,5150,
			5800,6500,7300,8200,9200,10300,11500,12800,14200,15700,
			17300,19000,20800,22700,24700,26800,29000,31400,34000,36800,
			39800,43000,46400,50000,53800,57800,62000,66400,71000,75800,
			80800,86050,91550,97300,103300,109550,116050,122800,129800,137050,
			144550,152350,160450,168850,177550,186550,196050,206050,216550,227550,
			239050,251050,263550,276550,290050,304050,318550,333550,349050,365050,
			415050,435050,456050,478050,501050,525050,550050,576050,603050,703050,
			853050,1053050,1303050,1603050,1953050,2353050,2803050,3303050,3803050,4803050,
		};
		public int[] ExpTable { get; set; }

		#region CurrentLevel

		private int _CurrentLevel;

		public int CurrentLevel
		{
			get { return this._CurrentLevel; }
			set
			{
				if (this._CurrentLevel != value && value >= 1 && value <= 100)
				{
					this._CurrentLevel = value;
					this.TargetLevel = Math.Max(this.TargetLevel, Math.Min(value + 1, this.MaxLevel));
					this.RaisePropertyChanged();
					this.UpdateExpData();
				}
			}
		}

		#endregion

		#region TargetLevel

		private int _TargetLevel;

		public int TargetLevel
		{
			get { return this._TargetLevel; }
			set
			{
				if (this._TargetLevel != value && value >= 1 && value <= 100)
				{
					this._TargetLevel = value;
					this.TargetExp = ExpTable[value];
					this.CurrentLevel = Math.Min(this.CurrentLevel, Math.Max(value - 1, 1));
					this.RaisePropertyChanged();
					this.UpdateExpData();
				}
			}
		}

		#endregion

		#region CurrentExp

		private int _CurrentExp;

		public int CurrentExp
		{
			get { return this._CurrentExp; }
			private set
			{
				if (this._CurrentExp != value)
				{
					this._CurrentExp = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TargetExp

		private int _TargetExp;

		public int TargetExp
		{
			get { return this._TargetExp; }
			private set
			{
				if (this._TargetExp != value)
				{
					this._TargetExp = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RemainingExp

		private int _RemainingExp;

		public int RemainingExp
		{
			get { return this._RemainingExp; }
			private set
			{
				if (this._RemainingExp != value)
				{
					this._RemainingExp = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region AngelWeapon

		private int _AngelWeapon;

		public int AngelWeapon
		{
			get { return this._AngelWeapon; }
			private set
			{
				if (this._AngelWeapon != value)
				{
					this._AngelWeapon = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ArchAngelWeapon

		private int _ArchAngelWeapon;

		public int ArchAngelWeapon
		{
			get { return this._ArchAngelWeapon; }
			private set
			{
				if (this._ArchAngelWeapon != value)
				{
					this._ArchAngelWeapon = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsWeapon

		private bool _IsWeapon;

		public bool IsWeapon
		{
			get { return this._IsWeapon; }
			private set
			{
				if (this._IsWeapon != value)
				{
					this._IsWeapon = value;
					if (value) this.ExpTable = EquipExpTable;
					this.RaisePropertyChanged();
					if (this.expInfo != null) this.UpdateExpData();
				}
			}
		}

		#endregion

		#region IsChar

		private bool _IsChar;

		public bool IsChar
		{
			get { return this._IsChar; }
			private set
			{
				if (this._IsChar != value)
				{
					this._IsChar = value;
					if (value) this.ExpTable = CharExpTable;
					this.RaisePropertyChanged();
					if (this.expInfo != null) this.UpdateExpData();
				}
			}
		}

		#endregion

		#region IsMultiply

		private bool _IsMultiply;

		public bool IsMultiply
		{
			get { return this._IsMultiply; }
			private set
			{
				if (this._IsMultiply != value)
				{
					this._IsMultiply = value;
					this.RaisePropertyChanged();
					if (this.expInfo != null) this.UpdateExpData();
				}
			}
		}

		#endregion
		//
		public ExpInfo expInfo { get; set; }
		private int MaxLevel { get; set; }


		public ExpCalViewModel()
		{
			this.Read();
		}
		private void UpdateExpData()
		{
			this.expInfo = GrandcypherClient.Current.EnhancementHooker.expInfo;
			if (this.expInfo.max_level <= 100) this.MaxLevel = this.expInfo.max_level;
			else this.MaxLevel = 100;

			this.CurrentExp = this.expInfo.exp;
			this.CurrentLevel = this.expInfo.level;

			this.RemainingExp = ExpTable[TargetLevel] - this.CurrentExp;
			if (IsMultiply) this.AngelWeapon = Convert.ToInt32(Math.Ceiling((double)RemainingExp / (100d * 1.5d)));
			else this.AngelWeapon = Convert.ToInt32(Math.Ceiling((double)RemainingExp / 100d));

			if (IsMultiply) this.ArchAngelWeapon = Convert.ToInt32(Math.Ceiling((double)RemainingExp / (500d * 1.5d)));
			else this.ArchAngelWeapon = Convert.ToInt32(Math.Ceiling((double)RemainingExp / 500d));
		}
		private void Read()
		{
			GrandcypherClient.Current.EnhancementHooker.WeaponReadStart += () =>
			{
				this.IsWeapon = true;
				this.IsChar = false;
			};
			GrandcypherClient.Current.EnhancementHooker.WeaponReadEnd += () =>
			{
				this.UpdateExpData();
			};
			GrandcypherClient.Current.EnhancementHooker.CharReadStart += () =>
			{
				this.IsChar = true;
				this.IsWeapon = false;
			};
			GrandcypherClient.Current.EnhancementHooker.CharReadEnd += () =>
			{
				this.UpdateExpData();
			};
		}
	}
}
