using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Redbean
{
	public class AudioPlayer
	{
		private readonly bool isTemporary;

		public AudioSource AudioSource;
		
		public AudioPlayer()
		{
			AudioSource = AudioMono.Audio.GetAudioSources().FirstOrDefault(_ => !_.isPlaying);
			if (AudioSource)
				return;

			AudioSource = AudioMono.Audio.AddAudioSource();
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
	
	public class AudioMono : MonoBehaviour
	{
		public static AudioMono Audio;
		
		public static void OnInitialize()
		{
			var go = new GameObject("[Audio System]", 
			                             typeof(AudioSource),
			                             typeof(AudioSource),
			                             typeof(AudioSource),
			                             typeof(AudioSource),
			                             typeof(AudioMono));
			go.transform.SetParent(AppLifeCycle.Transform);
			
			Audio = go.GetComponent<AudioMono>();
		}

		public AudioSource[] GetAudioSources() => 
			gameObject.GetComponents<AudioSource>();
		
		public AudioSource AddAudioSource() => 
			gameObject.AddComponent<AudioSource>();
	}
}