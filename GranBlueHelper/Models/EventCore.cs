﻿using GranBlueHelper.Models.Notifier;
using Grandcypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSettings = GranBlueHelper.Properties.Settings;

namespace GranBlueHelper.Models
{
	public class EventCore
	{
		#region singleton
		public static EventCore Current { get; } = new EventCore();
		#endregion
		public void Start()
		{
			GrandcypherClient.Current.PortError += () =>
			{
				Settings.Current.portNum = Convert.ToInt32(AppSettings.Default.LocalProxyPort);
			};
			GrandcypherClient.Current.ResultHooker.EndBattle += () =>
			{
				MainNotifier.Current.Show("전투종료알림", "전투가 종료되었습니다", () => App.ViewModelRoot.Activate());
			};
			GrandcypherClient.Current.WeaponHooker.SkillError += () =>
			{
				MainNotifier.Current.Show("무기 목록 설정", "무기 목록 정렬이 스킬레벨로 설정되어있지 않습니다.", () => App.ViewModelRoot.Activate());
			};
			GrandcypherClient.Current.WeaponHooker.AtkError += () =>
			{
				MainNotifier.Current.Show("무기 목록 설정", "무기 목록 정렬이 공격력으로 설정되어있지 않습니다.", () => App.ViewModelRoot.Activate());
			};
			GrandcypherClient.Current.WeaponHooker.HPError += () =>
			{
				MainNotifier.Current.Show("무기 목록 설정", "무기 목록 정렬이 HP로 설정되어있지 않습니다.", () => App.ViewModelRoot.Activate());
			};
			GrandcypherClient.Current.WeaponHooker.FinishRead += () =>
			{
				MainNotifier.Current.Show("무기 목록 설정", "총 " + GrandcypherClient.Current.WeaponHooker.WeaponList.Count + "개의 무기정보(" + GrandcypherClient.Current.WeaponHooker.CurrentMode.ToString() + ")를 불러오는데 성공했습니다.", () => App.ViewModelRoot.Activate());
			};
		}
	}
}
