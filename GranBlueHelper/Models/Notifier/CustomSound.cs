using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppSettings= GranBlueHelper.Properties.Settings;

namespace GranBlueHelper.Models.Notifier
{
	public class CustomSound
	{

		private BlockAlignReductionStream BlockStream = null;
		private DirectSoundOut SoundOut = null;
		string Main_folder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		public void SoundOutput(string header, bool IsWin8)
		{
			try
			{
				DisposeWave();//알림이 동시에 여러개가 울릴 경우 소리가 겹치는 문제를 방지
				if (!Directory.Exists(Path.Combine(Main_folder, "Sounds")))
					Directory.CreateDirectory(Path.Combine(Main_folder, "Sounds"));
				List<string> FileList = Directory.GetFiles(Path.Combine(Main_folder, "Sounds"), "*.wav", SearchOption.AllDirectories)
					.Concat(Directory.GetFiles(Path.Combine(Main_folder, "Sounds"), "*.mp3", SearchOption.AllDirectories)).ToList();//mp3와 wav를 검색하여 추가
				string Audiofile = string.Empty;
				if (FileList.Count > 0)
				{
					Random Rnd = new Random();
					Audiofile = FileList[Rnd.Next(0, FileList.Count)];

					float Volume = AppSettings.Default.CustomSoundVolume > 0 ? (float)AppSettings.Default.CustomSoundVolume / 100 : 0;
					if (Path.GetExtension(Audiofile).ToLower() == ".wav")//wav인지 채크
					{
						WaveStream pcm = new WaveChannel32(new WaveFileReader(Audiofile), Volume, 0);
						BlockStream = new BlockAlignReductionStream(pcm);
					}
					else if (Path.GetExtension(Audiofile).ToLower() == ".mp3")//mp3인 경우
					{
						WaveStream pcm = new WaveChannel32(new Mp3FileReader(Audiofile), Volume, 0);
						BlockStream = new BlockAlignReductionStream(pcm);
					}
					SoundOut = new DirectSoundOut();
					SoundOut.Init(BlockStream);
					SoundOut.Play();
				}
				else
				{
					System.Media.SystemSounds.Beep.Play();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				System.Media.SystemSounds.Beep.Play();
			}
		}
		private void DisposeWave()
		{
			if (SoundOut != null)
			{
				if (SoundOut.PlaybackState == NAudio.Wave.PlaybackState.Playing) SoundOut.Stop();
				SoundOut.Dispose();
				SoundOut = null;
			}
			if (BlockStream != null)
			{
				BlockStream.Dispose();
				BlockStream = null;
			}
		}

	}
}