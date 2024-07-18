using UnityEngine;

namespace Redbean.MVP
{
	public interface IPresenter : IExtension
	{
		/// <summary>
		/// 연결되어 있는 게임 오브젝트 호출
		/// </summary>
		GameObject GetGameObject();
		
		/// <summary>
		/// View와 Presenter 연결
		/// </summary>
		void BindView(IView view);
		
		/// <summary>
		/// Presenter 생성 시 호출되는 함수
		/// </summary>
		void Setup();

		/// <summary>
		/// Presenter 파괴 시 호출되는 함수
		/// </summary>
		void Teardown();
	}
}