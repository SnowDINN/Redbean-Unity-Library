using System.Threading.Tasks;
using UnityEngine;

namespace Redbean.Utility
{
	public class AnimationPlayer
	{
		private readonly Animation animation;
		
		public AnimationPlayer(Animation animation)
		{
			this.animation = animation;
		}

		public async Task PlayAsync(string clip)
		{
			animation.Play(clip);

			OnAnimationStarted?.Invoke();
			
			while (animation.isPlaying)
				await Task.Yield();
			
			OnAnimationEnded?.Invoke();
		}

		public delegate void onAnimationStarted();
		public event onAnimationStarted OnAnimationStarted;
		
		public delegate void onAnimationEnded();
		public event onAnimationEnded OnAnimationEnded;
	}
}