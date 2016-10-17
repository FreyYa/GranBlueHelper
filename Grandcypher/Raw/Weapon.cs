using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grandcypher.Raw
{
	public class Weapon
	{
		public Weapon()
		{
			this.param = new Param();
			this.master = new Master();
		}
		public bool IsExclude { get; set; }
		public bool IsUsed { get; set; }
		public Master master { get; set; }
		public Param param { get; set; }
	}
	public class Master
	{
		public Master()
		{
			this.Skill1 = new Skill();
			this.Skill2 = new Skill();
		}
		public int archaic { get; set; }
		public int default_open { get; set; }
		public int id { get; set; }
		public dynamic is_group { get; set; }
		public int max_evolution_level { get; set; }
		public int rarity { get; set; }
		public int release_max_evolution_level { get; set; }
		public Skill Skill1 { get; set; }
		public string skill1_image
		{
			get { return this.Skill1.RawData; }
			set
			{
				this.Skill1.SetData(value);
			}
		}
		public Skill Skill2 { get; set; }
		public string skill2_image
		{
			get { return this.Skill1.RawData; }
			set
			{
				this.Skill2.SetData(value);
			}
		}
		public int sold_price { get; set; }
	}
	public class Skill
	{
		public void SetData(string value)
		{
			if (value == string.Empty) return;

			this.RawData = value;
			var temp = this.RawData.Split('_');
			//스킬의 종류를 판별
			if (value.Contains("skill_baha"))//바하무트
				this.SkillType |= SkillType.바하무트;
			else if (value.Contains("skill_god"))//신위
				this.SkillType |= SkillType.신위;
			else if (value.Contains("skill_whole"))//군신
				this.SkillType |= SkillType.군신;
			else if (value.Contains("skill_backwater"))//배수
				this.SkillType |= SkillType.배수;
			else if (value.Contains("skill_ta"))//삼수
				this.SkillType |= SkillType.삼수;
			else if (value.Contains("skill_job_weapon"))//직업무기
				this.SkillType |= SkillType.직업무기;
			else if (value.Contains("skill_tresure"))
				this.SkillType |= SkillType.트레저;
			else if (value.Contains("skill_material"))
				this.SkillType |= SkillType.스킬강화소재;

			int tempint;
			if(temp.Count()>3)
			{
				if (temp[2] == "m")//마그나
					this.SkillType |= SkillType.마그나;
				else if (temp[2] == "a")//언노운
					this.SkillType |= SkillType.언노운;
				else if (!this.SkillType.HasFlag(SkillType.직업무기) && int.TryParse(temp[2], out tempint))
					this.SkillType |= SkillType.일반;
				else
					this.SkillType |= SkillType.지원하지않는타입;
			}
			else
				this.SkillType |= SkillType.지원하지않는타입;

			if (this.SkillType == SkillType.지원하지않는타입) return;

			if (value.Contains("_atk_"))
				this.SkillType |= SkillType.공인;
			else if (value.Contains("_hp_"))
				this.SkillType |= SkillType.수호;


			if (!this.SkillType.HasFlag(SkillType.바하무트))
			{
				//속성,단계 판별
				if (temp.Count() > 1)
				{
					if (this.SkillType.HasFlag(SkillType.마그나) || this.SkillType.HasFlag(SkillType.언노운))
					{
						this.Attribute = (Attribute)Convert.ToInt32(temp[3]);
						if (!this.SkillType.HasFlag(SkillType.신위))
						{
							switch (Convert.ToInt32(temp[4]))
							{
								case 1:
									this.SkillPhase |= SkillPhase.소;
									break;
								case 2:
									this.SkillPhase |= SkillPhase.중;
									break;
								case 3:
									this.SkillPhase |= SkillPhase.대;
									break;
							}
						}
						if (temp.Count() > 5)//2단계 스킬
							this.SkillPhase |= SkillPhase.단계2;
					}
					else if (this.SkillType.HasFlag(SkillType.일반))
					{
						this.Attribute = (Attribute)Convert.ToInt32(temp[2]);
						if (!this.SkillType.HasFlag(SkillType.신위))
						{
							switch (Convert.ToInt32(temp[3]))
							{
								case 1:
									this.SkillPhase |= SkillPhase.소;
									break;
								case 2:
									this.SkillPhase |= SkillPhase.중;
									break;
								case 3:
									this.SkillPhase |= SkillPhase.대;
									break;
							}
						}
						if (temp.Count() > 4)//2단계 스킬
							this.SkillPhase |= SkillPhase.단계2;
					}
				}

			}
		}
		public Attribute Attribute { get; private set; }
		public SkillType SkillType { get; private set; }
		public SkillPhase SkillPhase { get; private set; }
		//skill1_image
		public string RawData { get; private set; }
		//skill1_display
		public string Display { get; set; }
	}
	public class Param
	{
		public Bullet bullet { get; set; }
		public int evolution { get; set; }
		public int id { get; set; }
		public int is_locked { get; set; }
		public bool is_user_level { get; set; }
		public int quality { get; set; }
		public int SkillLv { get; set; }
		public int AttStatus { get; set; }
		public int HPStatus { get; set; }
	}
	public class Bullet
	{
		public int max_set_count { get; set; }
		public int this_set_count { get; set; }
	}
	/// <summary>
	/// 스킬 단계를 표시
	/// http://sjpison.tistory.com/1
	/// </summary>
	[Flags]
	public enum SkillPhase
	{
		없음 = 0,
		소 = 1,
		중 = 1<<1,
		대 = 1 << 2,
		단계2 = 1 << 3,
	}
	public enum Attribute
	{
		화 = 1,
		수 = 2,
		토 = 3,
		풍 = 4,
		광 = 5,
		암 = 6,
	}
	[Flags]
	public enum SkillType
	{
		지원하지않는타입 = 0,
		마그나 = 1,
		일반 = 1 << 1,
		언노운 = 1 << 2,
		바하무트 = 1 << 3,
		신위 = 1 << 4,
		배수 = 1 << 5,
		군신 = 1 << 6,
		삼수 = 1 << 7,
		공인 = 1 << 8,
		수호 = 1 << 9,
		직업무기 = 1 << 10,
		트레저 = 1 << 11,
		스킬강화소재 = 1 << 12,
	}
}