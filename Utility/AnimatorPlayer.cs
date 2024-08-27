using System.Threading.Tasks;
using UnityEngine;

namespace Redbean.Utility
{
	public class AnimatorPlayer
	{
		private readonly Animator animator;
		
		public AnimatorPlayer(Animator animator)
		{
			this.animator = animator;
		}

		public async Task PlayAsync(string clip, int index = 0)
		{
			animator.Play(clip, index);

			OnAnimatorStarted?.Invoke();
			
			while (animator.GetCurrentAnimatorStateInfo(index).normalizedTime < 1.0f)
				await Task.Yield();
			
			OnAnimatorEnded?.Invoke();
		}

		public delegate void onAnimatorStarted();
		public event onAnimatorStarted OnAnimatorStarted;
		
		public delegate void onAnimatorEnded();
		public event onAnimatorEnded OnAnimatorEnded;
	}
}