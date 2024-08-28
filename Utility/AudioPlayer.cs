using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Redbean.Utility
{
	public class AudioPlayer
	{
		private IEnumerable<AudioSource> audioSources => 
			AppLifeCycle.AudioSystem.GetComponents<AudioSource>();

		private readonly bool isTemporary;

		public AudioSource AudioSource;
		
		public AudioPlayer()
		{
			AudioSource = audioSources.FirstOrDefault(_ => !_.isPlaying);
			if (AudioSource)
				return;

			AudioSource = AppLifeCycle.AudioSystem.AddComponent<AudioSource>();
			isTemporary = true;
		}

		public async void Play(AudioClip clip)
		{
			await PlayAudioSource(clip);
			
			if (isTemporary)
				Object.Destroy(AudioSource);
		}
		
		private async Task PlayAudioSource(AudioClip clip)
		{
			AudioSource.clip = clip;
			AudioSource.Play();

			while (AudioSource && AudioSource.isPlaying)
				await Task.Yield();

			if (!AudioSource)
				return;
			
			AudioSource.clip = null;
		}
	}
}